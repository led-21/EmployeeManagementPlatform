using Microsoft.EntityFrameworkCore;
using EmployeeManagementPlatform.Api.Data;
using EmployeeManagementPlatform.Api.DTOs;
using EmployeeManagementPlatform.Api.Interfaces;
using EmployeeManagementPlatform.Api.Models;

namespace EmployeeManagementPlatform.Api.Services;

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _context;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(AppDbContext context, ILogger<EmployeeService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<EmployeeResponseDto>> GetAllAsync(EmployeeQueryParams query, CancellationToken ct = default)
    {
        var dbQuery = _context.Employees.AsNoTracking().AsQueryable();

        // Search across name, email, position
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = $"%{query.Search.Trim()}%";
            dbQuery = dbQuery.Where(e =>
                EF.Functions.Like(e.FirstName, term) ||
                EF.Functions.Like(e.LastName, term) ||
                EF.Functions.Like(e.Email, term) ||
                EF.Functions.Like(e.Position, term));
        }

        // Filter by department
        if (!string.IsNullOrWhiteSpace(query.Department))
        {
            var dept = query.Department.Trim();
            dbQuery = dbQuery.Where(e => e.Department == dept);
        }

        // Filter by active status
        if (query.Active.HasValue)
        {
            dbQuery = dbQuery.Where(e => e.Active == query.Active.Value);
        }

        var totalCount = await dbQuery.CountAsync(ct);

        // Sorting
        var isDesc = string.Equals(query.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);
        dbQuery = (query.SortBy?.ToLowerInvariant()) switch
        {
            "name" => isDesc
                ? dbQuery.OrderByDescending(e => e.LastName).ThenByDescending(e => e.FirstName)
                : dbQuery.OrderBy(e => e.LastName).ThenBy(e => e.FirstName),
            "firstname" => isDesc ? dbQuery.OrderByDescending(e => e.FirstName) : dbQuery.OrderBy(e => e.FirstName),
            "department" => isDesc ? dbQuery.OrderByDescending(e => e.Department) : dbQuery.OrderBy(e => e.Department),
            "position" => isDesc ? dbQuery.OrderByDescending(e => e.Position) : dbQuery.OrderBy(e => e.Position),
            "salary" => isDesc ? dbQuery.OrderByDescending(e => e.Salary) : dbQuery.OrderBy(e => e.Salary),
            "hiredate" => isDesc ? dbQuery.OrderByDescending(e => e.HireDate) : dbQuery.OrderBy(e => e.HireDate),
            "createdat" => isDesc ? dbQuery.OrderByDescending(e => e.CreatedAt) : dbQuery.OrderBy(e => e.CreatedAt),
            _ => isDesc ? dbQuery.OrderByDescending(e => e.Id) : dbQuery.OrderBy(e => e.Id)
        };

        // Pagination
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 10 : (query.PageSize > 100 ? 100 : query.PageSize);

        var items = await dbQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => MapToDto(e))
            .ToListAsync(ct);

        return PagedResult<EmployeeResponseDto>.Create(items, totalCount, page, pageSize);
    }

    public async Task<EmployeeResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var employee = await _context.Employees.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, ct);

        return employee is null ? null : MapToDto(employee);
    }

    public async Task<EmployeeResponseDto> CreateAsync(CreateEmployeeDto dto, CancellationToken ct = default)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
        var emailExists = await _context.Employees.AnyAsync(e => e.Email.ToLower() == normalizedEmail, ct);
        if (emailExists)
        {
            throw new InvalidOperationException($"An employee with email '{dto.Email}' already exists.");
        }

        var employee = new Employee
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = normalizedEmail,
            Position = dto.Position.Trim(),
            Department = dto.Department.Trim(),
            Salary = dto.Salary,
            HireDate = DateTime.SpecifyKind(dto.HireDate.Date, DateTimeKind.Utc),
            Active = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Employees.AddAsync(employee, ct);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Created new employee ID {EmployeeId} ({Email})", employee.Id, employee.Email);
        return MapToDto(employee);
    }

    public async Task<EmployeeResponseDto?> UpdateAsync(int id, UpdateEmployeeDto dto, CancellationToken ct = default)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (employee is null)
        {
            return null;
        }

        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
        var emailConflict = await _context.Employees.AnyAsync(e => e.Email.ToLower() == normalizedEmail && e.Id != id, ct);
        if (emailConflict)
        {
            throw new InvalidOperationException($"An employee with email '{dto.Email}' already exists.");
        }

        employee.FirstName = dto.FirstName.Trim();
        employee.LastName = dto.LastName.Trim();
        employee.Email = normalizedEmail;
        employee.Position = dto.Position.Trim();
        employee.Department = dto.Department.Trim();
        employee.Salary = dto.Salary;
        employee.HireDate = DateTime.SpecifyKind(dto.HireDate.Date, DateTimeKind.Utc);
        employee.Active = dto.Active;
        employee.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("Updated employee ID {EmployeeId}", employee.Id);

        return MapToDto(employee);
    }

    public async Task<bool> ToggleActiveStatusAsync(int id, CancellationToken ct = default)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (employee is null)
        {
            return false;
        }

        employee.Active = !employee.Active;
        employee.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("Toggled active status for employee ID {EmployeeId} to {Status}", employee.Id, employee.Active);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (employee is null)
        {
            return false;
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Deleted employee ID {EmployeeId}", id);
        return true;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken ct = default)
    {
        var total = await _context.Employees.CountAsync(ct);
        var active = await _context.Employees.CountAsync(e => e.Active, ct);
        var inactive = total - active;

        var departments = await _context.Employees
            .Select(e => e.Department)
            .ToListAsync(ct);

        var deptCounts = departments
            .GroupBy(d => d)
            .Select(g => new DepartmentCountDto(g.Key, g.Count()))
            .OrderByDescending(d => d.Count)
            .ToList();

        return new DashboardSummaryDto(
            total,
            active,
            inactive,
            deptCounts.Count,
            deptCounts
        );
    }

    public async Task<IReadOnlyList<string>> GetDepartmentsAsync(CancellationToken ct = default)
    {
        return await _context.Employees
            .Select(e => e.Department)
            .Distinct()
            .OrderBy(d => d)
            .ToListAsync(ct);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null, CancellationToken ct = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        var query = _context.Employees.AsQueryable();
        if (excludeId.HasValue)
        {
            query = query.Where(e => e.Id != excludeId.Value);
        }

        return !await query.AnyAsync(e => e.Email.ToLower() == normalized, ct);
    }

    private static EmployeeResponseDto MapToDto(Employee e) =>
        new(
            e.Id,
            e.FirstName,
            e.LastName,
            e.FullName,
            e.Email,
            e.Position,
            e.Department,
            e.Salary,
            e.HireDate,
            e.Active,
            e.CreatedAt,
            e.UpdatedAt
        );
}
