using EmployeeManagementPlatform.Api.DTOs;
using EmployeeManagementPlatform.Api.Validators;
using Xunit;

namespace EmployeeManagementPlatform.Tests;

public class EmployeeValidationTests
{
    private readonly CreateEmployeeDtoValidator _createValidator = new();
    private readonly UpdateEmployeeDtoValidator _updateValidator = new();

    [Fact]
    public async Task CreateValidator_Passes_WhenDtoIsValid()
    {
        // Arrange
        var dto = new CreateEmployeeDto(
            FirstName: "John",
            LastName: "Doe",
            Email: "john.doe@company.com",
            Position: "Software Developer",
            Department: "Engineering",
            Salary: 60000m,
            HireDate: new DateTime(2023, 1, 15)
        );

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("", "Doe", "First name is required.")]
    [InlineData("John", "", "Last name is required.")]
    public async Task CreateValidator_Fails_WhenNameIsMissing(string firstName, string lastName, string expectedError)
    {
        // Arrange
        var dto = new CreateEmployeeDto(
            FirstName: firstName,
            LastName: lastName,
            Email: "user@company.com",
            Position: "Developer",
            Department: "IT",
            Salary: 50000m,
            HireDate: DateTime.UtcNow.AddMonths(-1)
        );

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains(expectedError));
    }

    [Fact]
    public async Task CreateValidator_Fails_WhenFirstNameMatchesLastName()
    {
        // Arrange
        var dto = new CreateEmployeeDto(
            FirstName: "Smith",
            LastName: "Smith",
            Email: "smith@company.com",
            Position: "Developer",
            Department: "IT",
            Salary: 50000m,
            HireDate: DateTime.UtcNow.AddMonths(-1)
        );

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("identical"));
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("user@")]
    [InlineData("@company.com")]
    public async Task CreateValidator_Fails_WhenEmailIsInvalid(string invalidEmail)
    {
        // Arrange
        var dto = new CreateEmployeeDto(
            FirstName: "John",
            LastName: "Doe",
            Email: invalidEmail,
            Position: "Developer",
            Department: "IT",
            Salary: 50000m,
            HireDate: DateTime.UtcNow.AddMonths(-1)
        );

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateEmployeeDto.Email));
    }

    [Fact]
    public async Task CreateValidator_Fails_WhenSalaryIsNegative()
    {
        // Arrange
        var dto = new CreateEmployeeDto(
            FirstName: "John",
            LastName: "Doe",
            Email: "john.doe@company.com",
            Position: "Developer",
            Department: "IT",
            Salary: -100m,
            HireDate: DateTime.UtcNow.AddMonths(-1)
        );

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateEmployeeDto.Salary));
    }

    [Fact]
    public async Task CreateValidator_Fails_WhenHireDateIsInFuture()
    {
        // Arrange
        var dto = new CreateEmployeeDto(
            FirstName: "John",
            LastName: "Doe",
            Email: "john.doe@company.com",
            Position: "Developer",
            Department: "IT",
            Salary: 50000m,
            HireDate: DateTime.UtcNow.AddDays(5)
        );

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateEmployeeDto.HireDate));
    }
}
