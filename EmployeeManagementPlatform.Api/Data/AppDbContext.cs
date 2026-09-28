using Microsoft.EntityFrameworkCore;
using EmployeeManagementPlatform.Api.Models;

namespace EmployeeManagementPlatform.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Position).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Department).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Department);
            entity.Property(e => e.Salary).HasPrecision(18, 2);
            entity.Property(e => e.HireDate).IsRequired();
            entity.Property(e => e.Active).IsRequired().HasDefaultValue(true);
            entity.HasIndex(e => e.Active);
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        // Seed initial synthetic employees for development & portfolio showcase
        modelBuilder.Entity<Employee>().HasData(
            new Employee
            {
                Id = 1,
                FirstName = "Sarah",
                LastName = "Connor",
                Email = "sarah.connor@example.com",
                Position = "VP of Engineering",
                Department = "Engineering",
                Salary = 95000m,
                HireDate = new DateTime(2021, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                Active = true,
                CreatedAt = new DateTime(2021, 3, 15, 0, 0, 0, DateTimeKind.Utc)
            },
            new Employee
            {
                Id = 2,
                FirstName = "Alex",
                LastName = "Rivera",
                Email = "alex.rivera@example.com",
                Position = "Senior Full-Stack Developer",
                Department = "Engineering",
                Salary = 78000m,
                HireDate = new DateTime(2022, 1, 10, 0, 0, 0, DateTimeKind.Utc),
                Active = true,
                CreatedAt = new DateTime(2022, 1, 10, 0, 0, 0, DateTimeKind.Utc)
            },
            new Employee
            {
                Id = 3,
                FirstName = "Elena",
                LastName = "Rostova",
                Email = "elena.rostova@example.com",
                Position = "Lead Product Manager",
                Department = "Product",
                Salary = 82000m,
                HireDate = new DateTime(2021, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                Active = true,
                CreatedAt = new DateTime(2021, 7, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Employee
            {
                Id = 4,
                FirstName = "Marcus",
                LastName = "Vance",
                Email = "marcus.vance@example.com",
                Position = "UI/UX Designer",
                Department = "Design",
                Salary = 65000m,
                HireDate = new DateTime(2022, 6, 20, 0, 0, 0, DateTimeKind.Utc),
                Active = true,
                CreatedAt = new DateTime(2022, 6, 20, 0, 0, 0, DateTimeKind.Utc)
            },
            new Employee
            {
                Id = 5,
                FirstName = "Amina",
                LastName = "Diallo",
                Email = "amina.diallo@example.com",
                Position = "HR Director",
                Department = "Human Resources",
                Salary = 75000m,
                HireDate = new DateTime(2020, 11, 5, 0, 0, 0, DateTimeKind.Utc),
                Active = true,
                CreatedAt = new DateTime(2020, 11, 5, 0, 0, 0, DateTimeKind.Utc)
            },
            new Employee
            {
                Id = 6,
                FirstName = "David",
                LastName = "Kim",
                Email = "david.kim@example.com",
                Position = "DevOps Engineer",
                Department = "Engineering",
                Salary = 74000m,
                HireDate = new DateTime(2023, 2, 14, 0, 0, 0, DateTimeKind.Utc),
                Active = true,
                CreatedAt = new DateTime(2023, 2, 14, 0, 0, 0, DateTimeKind.Utc)
            },
            new Employee
            {
                Id = 7,
                FirstName = "Sofia",
                LastName = "Martins",
                Email = "sofia.martins@example.com",
                Position = "Financial Analyst",
                Department = "Finance",
                Salary = 62000m,
                HireDate = new DateTime(2023, 5, 2, 0, 0, 0, DateTimeKind.Utc),
                Active = true,
                CreatedAt = new DateTime(2023, 5, 2, 0, 0, 0, DateTimeKind.Utc)
            },
            new Employee
            {
                Id = 8,
                FirstName = "Liam",
                LastName = "Chen",
                Email = "liam.chen@example.com",
                Position = "Operations Specialist",
                Department = "Operations",
                Salary = 55000m,
                HireDate = new DateTime(2023, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                Active = false,
                CreatedAt = new DateTime(2023, 9, 18, 0, 0, 0, DateTimeKind.Utc)
            },
            new Employee
            {
                Id = 9,
                FirstName = "Olivia",
                LastName = "Taylor",
                Email = "olivia.taylor@example.com",
                Position = "Marketing Manager",
                Department = "Marketing",
                Salary = 68000m,
                HireDate = new DateTime(2022, 8, 12, 0, 0, 0, DateTimeKind.Utc),
                Active = true,
                CreatedAt = new DateTime(2022, 8, 12, 0, 0, 0, DateTimeKind.Utc)
            },
            new Employee
            {
                Id = 10,
                FirstName = "James",
                LastName = "Wilson",
                Email = "james.wilson@example.com",
                Position = "QA Automation Engineer",
                Department = "Engineering",
                Salary = 63000m,
                HireDate = new DateTime(2024, 1, 8, 0, 0, 0, DateTimeKind.Utc),
                Active = true,
                CreatedAt = new DateTime(2024, 1, 8, 0, 0, 0, DateTimeKind.Utc)
            },
            new Employee
            {
                Id = 11,
                FirstName = "Lucia",
                LastName = "Gomez",
                Email = "lucia.gomez@example.com",
                Position = "Backend Developer",
                Department = "Engineering",
                Salary = 71000m,
                HireDate = new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                Active = true,
                CreatedAt = new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Employee
            {
                Id = 12,
                FirstName = "Ethan",
                LastName = "Hunt",
                Email = "ethan.hunt@example.com",
                Position = "Security Specialist",
                Department = "Operations",
                Salary = 80000m,
                HireDate = new DateTime(2022, 4, 15, 0, 0, 0, DateTimeKind.Utc),
                Active = true,
                CreatedAt = new DateTime(2022, 4, 15, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
