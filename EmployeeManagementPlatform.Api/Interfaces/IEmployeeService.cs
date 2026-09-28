using EmployeeManagementPlatform.Api.DTOs;

namespace EmployeeManagementPlatform.Api.Interfaces;

public interface IEmployeeService
{
    Task<PagedResult<EmployeeResponseDto>> GetAllAsync(EmployeeQueryParams query, CancellationToken ct = default);
    Task<EmployeeResponseDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<EmployeeResponseDto> CreateAsync(CreateEmployeeDto dto, CancellationToken ct = default);
    Task<EmployeeResponseDto?> UpdateAsync(int id, UpdateEmployeeDto dto, CancellationToken ct = default);
    Task<bool> ToggleActiveStatusAsync(int id, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken ct = default);
    Task<IReadOnlyList<string>> GetDepartmentsAsync(CancellationToken ct = default);
    Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null, CancellationToken ct = default);
}
