using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagementPlatform.Api.DTOs;
using EmployeeManagementPlatform.Api.Interfaces;

namespace EmployeeManagementPlatform.Api.Endpoints;

public static class EmployeeEndpoints
{
    public static void MapEmployeeEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/employees")
            .WithTags("Employees");

        group.MapGet("/", GetAllEmployees)
            .WithName("GetAllEmployees")
            .WithSummary("Get paginated list of employees with search, filtering, and sorting")
            .Produces<PagedResult<EmployeeResponseDto>>(StatusCodes.Status200OK);

        group.MapGet("/summary", GetSummary)
            .WithName("GetDashboardSummary")
            .WithSummary("Get high-level summary metrics (total, active, departments)")
            .Produces<DashboardSummaryDto>(StatusCodes.Status200OK);

        group.MapGet("/departments", GetDepartments)
            .WithName("GetDepartments")
            .WithSummary("Get list of unique departments")
            .Produces<IReadOnlyList<string>>(StatusCodes.Status200OK);

        group.MapGet("/{id:int}", GetEmployeeById)
            .WithName("GetEmployeeById")
            .WithSummary("Get an employee by ID")
            .Produces<EmployeeResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateEmployee)
            .WithName("CreateEmployee")
            .WithSummary("Create a new employee")
            .Produces<EmployeeResponseDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPut("/{id:int}", UpdateEmployee)
            .WithName("UpdateEmployee")
            .WithSummary("Update an existing employee")
            .Produces<EmployeeResponseDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPatch("/{id:int}/status", ToggleStatus)
            .WithName("ToggleEmployeeStatus")
            .WithSummary("Toggle active/inactive status of an employee")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:int}", DeleteEmployee)
            .WithName("DeleteEmployee")
            .WithSummary("Delete an employee by ID")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public static async Task<IResult> GetAllEmployees(
        [AsParameters] EmployeeQueryParams query,
        IEmployeeService service,
        CancellationToken ct)
    {
        var result = await service.GetAllAsync(query, ct);
        return TypedResults.Ok(result);
    }

    public static async Task<IResult> GetSummary(
        IEmployeeService service,
        CancellationToken ct)
    {
        var summary = await service.GetSummaryAsync(ct);
        return TypedResults.Ok(summary);
    }

    public static async Task<IResult> GetDepartments(
        IEmployeeService service,
        CancellationToken ct)
    {
        var departments = await service.GetDepartmentsAsync(ct);
        return TypedResults.Ok(departments);
    }

    public static async Task<IResult> GetEmployeeById(
        int id,
        IEmployeeService service,
        CancellationToken ct)
    {
        var employee = await service.GetByIdAsync(id, ct);
        return employee is not null
            ? TypedResults.Ok(employee)
            : TypedResults.NotFound(new ProblemDetails
            {
                Title = "Employee Not Found",
                Detail = $"Employee with ID {id} was not found.",
                Status = StatusCodes.Status404NotFound
            });
    }

    public static async Task<IResult> CreateEmployee(
        CreateEmployeeDto dto,
        IValidator<CreateEmployeeDto> validator,
        IEmployeeService service,
        CancellationToken ct)
    {
        var validation = await validator.ValidateAsync(dto, ct);
        if (!validation.IsValid)
        {
            return TypedResults.ValidationProblem(validation.ToDictionary());
        }

        try
        {
            var created = await service.CreateAsync(dto, ct);
            return TypedResults.Created($"/api/employees/{created.Id}", created);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.Conflict(new ProblemDetails
            {
                Title = "Duplicate Employee",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    public static async Task<IResult> UpdateEmployee(
        int id,
        UpdateEmployeeDto dto,
        IValidator<UpdateEmployeeDto> validator,
        IEmployeeService service,
        CancellationToken ct)
    {
        var validation = await validator.ValidateAsync(dto, ct);
        if (!validation.IsValid)
        {
            return TypedResults.ValidationProblem(validation.ToDictionary());
        }

        try
        {
            var updated = await service.UpdateAsync(id, dto, ct);
            return updated is not null
                ? TypedResults.Ok(updated)
                : TypedResults.NotFound(new ProblemDetails
                {
                    Title = "Employee Not Found",
                    Detail = $"Employee with ID {id} was not found.",
                    Status = StatusCodes.Status404NotFound
                });
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.Conflict(new ProblemDetails
            {
                Title = "Email Conflict",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    public static async Task<IResult> ToggleStatus(
        int id,
        IEmployeeService service,
        CancellationToken ct)
    {
        var success = await service.ToggleActiveStatusAsync(id, ct);
        return success
            ? TypedResults.Ok(new { message = $"Status for employee {id} updated." })
            : TypedResults.NotFound(new ProblemDetails
            {
                Title = "Employee Not Found",
                Detail = $"Employee with ID {id} was not found.",
                Status = StatusCodes.Status404NotFound
            });
    }

    public static async Task<IResult> DeleteEmployee(
        int id,
        IEmployeeService service,
        CancellationToken ct)
    {
        var deleted = await service.DeleteAsync(id, ct);
        return deleted
            ? TypedResults.NoContent()
            : TypedResults.NotFound(new ProblemDetails
            {
                Title = "Employee Not Found",
                Detail = $"Employee with ID {id} was not found.",
                Status = StatusCodes.Status404NotFound
            });
    }
}
