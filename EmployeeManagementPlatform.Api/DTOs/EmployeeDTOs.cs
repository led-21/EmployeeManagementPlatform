namespace EmployeeManagementPlatform.Api.DTOs;

public record EmployeeResponseDto(
    int Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string Position,
    string Department,
    decimal Salary,
    DateTime HireDate,
    bool Active,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateEmployeeDto(
    string FirstName,
    string LastName,
    string Email,
    string Position,
    string Department,
    decimal Salary,
    DateTime HireDate
);

public record UpdateEmployeeDto(
    string FirstName,
    string LastName,
    string Email,
    string Position,
    string Department,
    decimal Salary,
    DateTime HireDate,
    bool Active
);

public record EmployeeQueryParams(
    string? Search = null,
    string? Department = null,
    bool? Active = null,
    string? SortBy = "name",
    string? SortOrder = "asc",
    int Page = 1,
    int PageSize = 10
);

public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage
)
{
    public static PagedResult<T> Create(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
    {
        var totalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;
        return new PagedResult<T>(
            items,
            totalCount,
            page,
            pageSize,
            totalPages,
            page > 1,
            page < totalPages
        );
    }
}

public record DashboardSummaryDto(
    int TotalEmployees,
    int ActiveEmployees,
    int InactiveEmployees,
    int TotalDepartments,
    IReadOnlyList<DepartmentCountDto> DepartmentCounts
);

public record DepartmentCountDto(
    string Department,
    int Count
);
