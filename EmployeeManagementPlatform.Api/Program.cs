using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EmployeeManagementPlatform.Api.Data;
using EmployeeManagementPlatform.Api.Endpoints;
using EmployeeManagementPlatform.Api.Interfaces;
using EmployeeManagementPlatform.Api.Services;
using EmployeeManagementPlatform.Api.Validators;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext with SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Default") ?? "Data Source=employees.db";
    options.UseSqlite(connectionString);
});

// Register Domain & Application Services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

// Register FluentValidation Validators
builder.Services.AddValidatorsFromAssemblyContaining<CreateEmployeeDtoValidator>();

// Standard ProblemDetails for consistent RFC 7807 / 9457 errors
builder.Services.AddProblemDetails();

// CORS configuration for Frontend (Vite default is 5173)
const string corsPolicyName = "AllowFrontendDev";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "http://localhost:3000",
                "http://127.0.0.1:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Swagger / OpenAPI documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Employee Management Platform API",
        Version = "v1",
        Description = "Clean RESTful API for managing company employees, departments, and metrics."
    });
});

var app = builder.Build();

// Apply migrations / ensure database is created automatically with seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee Management Platform API v1");
    });
}

app.UseHttpsRedirection();
app.UseCors(corsPolicyName);

// Map Endpoints
app.MapEmployeeEndpoints();

app.Run();
