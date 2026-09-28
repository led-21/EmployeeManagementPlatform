using Microsoft.Extensions.Logging.Abstractions;
using EmployeeManagementPlatform.Api.DTOs;
using EmployeeManagementPlatform.Api.Models;
using EmployeeManagementPlatform.Api.Services;
using EmployeeManagementPlatform.Tests.Helpers;
using Xunit;

namespace EmployeeManagementPlatform.Tests;

public class EmployeeServiceTests
{
    private static EmployeeService CreateService(out Api.Data.AppDbContext context)
    {
        context = TestDbContextFactory.Create();
        var logger = NullLogger<EmployeeService>.Instance;
        return new EmployeeService(context, logger);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedEmployees_WithSeedData()
    {
        // Arrange
        var service = CreateService(out _);
        var query = new EmployeeQueryParams(Page: 1, PageSize: 5);

        // Act
        var result = await service.GetAllAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Items.Count);
        Assert.True(result.TotalCount >= 10);
        Assert.Equal(1, result.Page);
        Assert.True(result.HasNextPage);
        Assert.False(result.HasPreviousPage);
    }

    [Fact]
    public async Task GetAllAsync_FiltersByDepartment()
    {
        // Arrange
        var service = CreateService(out _);
        var query = new EmployeeQueryParams(Department: "Engineering", PageSize: 50);

        // Act
        var result = await service.GetAllAsync(query);

        // Assert
        Assert.NotEmpty(result.Items);
        Assert.All(result.Items, e => Assert.Equal("Engineering", e.Department));
    }

    [Fact]
    public async Task GetAllAsync_SearchesByNameOrEmail()
    {
        // Arrange
        var service = CreateService(out _);
        var query = new EmployeeQueryParams(Search: "Sarah");

        // Act
        var result = await service.GetAllAsync(query);

        // Assert
        Assert.Single(result.Items);
        Assert.Equal("Sarah", result.Items[0].FirstName);
        Assert.Equal("Connor", result.Items[0].LastName);
    }

    [Fact]
    public async Task GetAllAsync_FiltersByActiveStatus()
    {
        // Arrange
        var service = CreateService(out _);
        var query = new EmployeeQueryParams(Active: false);

        // Act
        var result = await service.GetAllAsync(query);

        // Assert
        Assert.NotEmpty(result.Items);
        Assert.All(result.Items, e => Assert.False(e.Active));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEmployee_WhenFound()
    {
        // Arrange
        var service = CreateService(out _);

        // Act
        var employee = await service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(employee);
        Assert.Equal(1, employee.Id);
        Assert.Equal("Sarah", employee.FirstName);
        Assert.Equal("sarah.connor@example.com", employee.Email);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        // Arrange
        var service = CreateService(out _);

        // Act
        var employee = await service.GetByIdAsync(9999);

        // Assert
        Assert.Null(employee);
    }

    [Fact]
    public async Task CreateAsync_AddsEmployee_Successfully()
    {
        // Arrange
        var service = CreateService(out var context);
        var dto = new CreateEmployeeDto(
            FirstName: "Grace",
            LastName: "Hopper",
            Email: "grace.hopper@example.com",
            Position: "Principal Systems Architect",
            Department: "Engineering",
            Salary: 99000m,
            HireDate: new DateTime(2023, 6, 1, 0, 0, 0, DateTimeKind.Utc)
        );

        // Act
        var created = await service.CreateAsync(dto);

        // Assert
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal("Grace Hopper", created.FullName);
        Assert.Equal("grace.hopper@example.com", created.Email);
        Assert.True(created.Active);

        var fromDb = await context.Employees.FindAsync(created.Id);
        Assert.NotNull(fromDb);
        Assert.Equal("Grace", fromDb.FirstName);
    }

    [Fact]
    public async Task CreateAsync_ThrowsException_OnDuplicateEmail()
    {
        // Arrange
        var service = CreateService(out _);
        var dto = new CreateEmployeeDto(
            FirstName: "Duplicate",
            LastName: "User",
            Email: "sarah.connor@example.com", // already seeded
            Position: "Engineer",
            Department: "Engineering",
            Salary: 50000m,
            HireDate: new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        );

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(dto));
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingEmployee()
    {
        // Arrange
        var service = CreateService(out _);
        var updateDto = new UpdateEmployeeDto(
            FirstName: "Sarah",
            LastName: "Connor-Updated",
            Email: "sarah.updated@example.com",
            Position: "Chief Technology Officer",
            Department: "Executive",
            Salary: 120000m,
            HireDate: new DateTime(2021, 3, 15, 0, 0, 0, DateTimeKind.Utc),
            Active: true
        );

        // Act
        var updated = await service.UpdateAsync(1, updateDto);

        // Assert
        Assert.NotNull(updated);
        Assert.Equal("Connor-Updated", updated.LastName);
        Assert.Equal("sarah.updated@example.com", updated.Email);
        Assert.Equal("Chief Technology Officer", updated.Position);
        Assert.NotNull(updated.UpdatedAt);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenEmployeeNotFound()
    {
        // Arrange
        var service = CreateService(out _);
        var updateDto = new UpdateEmployeeDto(
            FirstName: "Test",
            LastName: "Test",
            Email: "test@example.com",
            Position: "Role",
            Department: "Dept",
            Salary: 50000m,
            HireDate: DateTime.UtcNow,
            Active: true
        );

        // Act
        var updated = await service.UpdateAsync(9999, updateDto);

        // Assert
        Assert.Null(updated);
    }

    [Fact]
    public async Task ToggleActiveStatusAsync_TogglesEmployeeStatus()
    {
        // Arrange
        var service = CreateService(out _);

        // Act
        var toggled = await service.ToggleActiveStatusAsync(1);
        var employee = await service.GetByIdAsync(1);

        // Assert
        Assert.True(toggled);
        Assert.NotNull(employee);
        Assert.False(employee.Active); // Was true, now false
    }

    [Fact]
    public async Task DeleteAsync_RemovesEmployee()
    {
        // Arrange
        var service = CreateService(out _);

        // Act
        var deleted = await service.DeleteAsync(2);
        var employee = await service.GetByIdAsync(2);

        // Assert
        Assert.True(deleted);
        Assert.Null(employee);
    }

    [Fact]
    public async Task GetSummaryAsync_ReturnsAccurateMetrics()
    {
        // Arrange
        var service = CreateService(out _);

        // Act
        var summary = await service.GetSummaryAsync();

        // Assert
        Assert.NotNull(summary);
        Assert.True(summary.TotalEmployees >= 10);
        Assert.True(summary.ActiveEmployees > 0);
        Assert.True(summary.InactiveEmployees >= 1); // Liam Chen is seeded inactive
        Assert.Equal(summary.TotalEmployees, summary.ActiveEmployees + summary.InactiveEmployees);
        Assert.NotEmpty(summary.DepartmentCounts);
    }

    [Fact]
    public async Task GetDepartmentsAsync_ReturnsDistinctDepartments()
    {
        // Arrange
        var service = CreateService(out _);

        // Act
        var departments = await service.GetDepartmentsAsync();

        // Assert
        Assert.NotNull(departments);
        Assert.Contains("Engineering", departments);
        Assert.Contains("Product", departments);
        Assert.Contains("Finance", departments);
    }
}
