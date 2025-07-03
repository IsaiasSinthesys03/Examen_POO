using ExamenPOO.Core.Entities;
using ExamenPOO.Core.Services;
using ExamenPOO.Core.ValueObjects;
using ExamenPOO.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExamenPOO.Infrastructure.Services;

public class DatabaseSeederService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHashingService _passwordHashingService;

    public DatabaseSeederService(ApplicationDbContext context, IPasswordHashingService passwordHashingService)
    {
        _context = context;
        _passwordHashingService = passwordHashingService;
    }

    public async Task SeedAsync()
    {
        // Check if students already exist
        if (await _context.Students.AnyAsync())
            return;

        var students = new List<Student>
        {
            new Student(
                new StudentNumber("2024001"),
                new Email("john.doe@university.edu"),
                "John",
                "Doe",
                new DateTime(1995, 5, 15),
                "Computer Science",
                "555-0001",
                "123 Main St"
            ),
            new Student(
                new StudentNumber("2024002"),
                new Email("jane.smith@university.edu"),
                "Jane",
                "Smith",
                new DateTime(1996, 8, 22),
                "Information Systems",
                "555-0002",
                "456 Oak Ave"
            ),
            new Student(
                new StudentNumber("2024003"),
                new Email("mike.johnson@university.edu"),
                "Mike",
                "Johnson",
                new DateTime(1994, 12, 10),
                "Software Engineering",
                "555-0003",
                "789 Pine St"
            )
        };

        // Set passwords for all students (password: "Demo123!")
        foreach (var student in students)
        {
            var hashedPassword = _passwordHashingService.HashPassword("Demo123!");
            student.SetPassword(hashedPassword);
        }

        _context.Students.AddRange(students);
        await _context.SaveChangesAsync();
    }
}
