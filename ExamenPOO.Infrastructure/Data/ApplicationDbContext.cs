using Microsoft.EntityFrameworkCore;
using ExamenPOO.Core.Entities;

namespace ExamenPOO.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<SemesterEnrollment> SemesterEnrollments { get; set; }
    public DbSet<EnrolledCourse> EnrolledCourses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Student configuration
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // Configure Value Objects
            entity.Property(e => e.StudentNumber)
                .HasConversion(
                    v => v.Value,
                    v => new ExamenPOO.Core.ValueObjects.StudentNumber(v))
                .IsRequired()
                .HasMaxLength(20);
            
            entity.HasIndex(e => e.StudentNumber).IsUnique();
            
            entity.Property(e => e.Email)
                .HasConversion(
                    v => v.Value,
                    v => new ExamenPOO.Core.ValueObjects.Email(v))
                .IsRequired()
                .HasMaxLength(100);
            
            entity.HasIndex(e => e.Email).IsUnique();
                
            entity.Property(e => e.PasswordHash)
                .IsRequired();
                
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.Career)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.CreatedAt)
                .IsRequired();
                
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
        });

        // Course configuration
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CourseCode).IsUnique();
            
            entity.Property(e => e.CourseCode)
                .IsRequired()
                .HasMaxLength(20);
                
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.Description)
                .HasMaxLength(500);
                
            entity.Property(e => e.Credits)
                .IsRequired();
                
            entity.Property(e => e.Department)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.CreatedAt)
                .IsRequired();
                
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
        });

        // SemesterEnrollment configuration
        modelBuilder.Entity<SemesterEnrollment>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Semester)
                .IsRequired()
                .HasMaxLength(20);
                
            entity.Property(e => e.Year)
                .IsRequired();
                
            entity.Property(e => e.MaxCredits)
                .IsRequired()
                .HasDefaultValue(21);
                
            entity.Property(e => e.EnrollmentDate)
                .IsRequired();

            // Foreign key relationship
            entity.HasOne(se => se.Student)
                .WithMany(s => s.SemesterEnrollments)
                .HasForeignKey(se => se.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint: One enrollment per student per semester/year
            entity.HasIndex(e => new { e.StudentId, e.Semester, e.Year }).IsUnique();
        });

        // EnrolledCourse configuration
        modelBuilder.Entity<EnrolledCourse>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.EnrollmentDate)
                .IsRequired();

            // Foreign key relationships
            entity.HasOne(ec => ec.SemesterEnrollment)
                .WithMany(se => se.EnrolledCourses)
                .HasForeignKey(ec => ec.SemesterEnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ec => ec.Course)
                .WithMany()
                .HasForeignKey(ec => ec.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique constraint: One course per semester enrollment
            entity.HasIndex(e => new { e.SemesterEnrollmentId, e.CourseId }).IsUnique();
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Fixed date for seed data
        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        // Seed data will be handled via migrations or service layer
        // modelBuilder.Entity<Student>().HasData(...);

        // Seed Courses
        modelBuilder.Entity<Course>().HasData(
            new Course
            {
                Id = 1,
                CourseCode = "CS101",
                Name = "Introduction to Programming",
                Description = "Basic programming concepts using C# and object-oriented programming principles",
                Credits = 4,
                Department = "Computer Science",
                CreatedAt = seedDate,
                IsActive = true
            },
            new Course
            {
                Id = 2,
                CourseCode = "CS201",
                Name = "Data Structures and Algorithms",
                Description = "Fundamental data structures and algorithmic problem-solving techniques",
                Credits = 4,
                Department = "Computer Science",
                CreatedAt = seedDate,
                IsActive = true
            },
            new Course
            {
                Id = 3,
                CourseCode = "CS301",
                Name = "Database Systems",
                Description = "Design and implementation of relational database systems",
                Credits = 3,
                Department = "Computer Science",
                CreatedAt = seedDate,
                IsActive = true
            },
            new Course
            {
                Id = 4,
                CourseCode = "CS401",
                Name = "Software Engineering",
                Description = "Software development methodologies and project management",
                Credits = 4,
                Department = "Computer Science",
                CreatedAt = seedDate,
                IsActive = true
            },
            new Course
            {
                Id = 5,
                CourseCode = "CS501",
                Name = "Web Development",
                Description = "Modern web development using ASP.NET Core and related technologies",
                Credits = 3,
                Department = "Computer Science",
                CreatedAt = seedDate,
                IsActive = true
            },
            new Course
            {
                Id = 6,
                CourseCode = "MATH201",
                Name = "Discrete Mathematics",
                Description = "Mathematical foundations for computer science",
                Credits = 3,
                Department = "Mathematics",
                CreatedAt = seedDate,
                IsActive = true
            }
        );

        // Seed SemesterEnrollments
        modelBuilder.Entity<SemesterEnrollment>().HasData(
            new SemesterEnrollment
            {
                Id = 1,
                StudentId = 1,
                Semester = "Fall",
                Year = 2024,
                MaxCredits = 18,
                EnrollmentDate = seedDate
            },
            new SemesterEnrollment
            {
                Id = 2,
                StudentId = 2,
                Semester = "Fall",
                Year = 2024,
                MaxCredits = 21,
                EnrollmentDate = seedDate
            },
            new SemesterEnrollment
            {
                Id = 3,
                StudentId = 3,
                Semester = "Fall",
                Year = 2024,
                MaxCredits = 15,
                EnrollmentDate = seedDate
            }
        );

        // Seed EnrolledCourses
        modelBuilder.Entity<EnrolledCourse>().HasData(
            // John Doe's enrollments (18 credits total)
            new EnrolledCourse
            {
                Id = 1,
                SemesterEnrollmentId = 1,
                CourseId = 1, // CS101 - 4 credits
                EnrollmentDate = seedDate
            },
            new EnrolledCourse
            {
                Id = 2,
                SemesterEnrollmentId = 1,
                CourseId = 2, // CS201 - 4 credits
                EnrollmentDate = seedDate
            },
            new EnrolledCourse
            {
                Id = 3,
                SemesterEnrollmentId = 1,
                CourseId = 3, // CS301 - 3 credits
                EnrollmentDate = seedDate
            },
            new EnrolledCourse
            {
                Id = 4,
                SemesterEnrollmentId = 1,
                CourseId = 6, // MATH201 - 3 credits
                EnrollmentDate = seedDate
            },
            new EnrolledCourse
            {
                Id = 5,
                SemesterEnrollmentId = 1,
                CourseId = 4, // CS401 - 4 credits
                EnrollmentDate = seedDate
            },
            // Jane Smith's enrollments (14 credits total, room for more)
            new EnrolledCourse
            {
                Id = 6,
                SemesterEnrollmentId = 2,
                CourseId = 1, // CS101 - 4 credits
                EnrollmentDate = seedDate
            },
            new EnrolledCourse
            {
                Id = 7,
                SemesterEnrollmentId = 2,
                CourseId = 3, // CS301 - 3 credits
                EnrollmentDate = seedDate
            },
            new EnrolledCourse
            {
                Id = 8,
                SemesterEnrollmentId = 2,
                CourseId = 5, // CS501 - 3 credits
                EnrollmentDate = seedDate
            },
            new EnrolledCourse
            {
                Id = 9,
                SemesterEnrollmentId = 2,
                CourseId = 4, // CS401 - 4 credits
                EnrollmentDate = seedDate
            },
            // Mike Johnson's enrollments (11 credits total, room for more)
            new EnrolledCourse
            {
                Id = 10,
                SemesterEnrollmentId = 3,
                CourseId = 1, // CS101 - 4 credits
                EnrollmentDate = seedDate
            },
            new EnrolledCourse
            {
                Id = 11,
                SemesterEnrollmentId = 3,
                CourseId = 5, // CS501 - 3 credits
                EnrollmentDate = seedDate
            },
            new EnrolledCourse
            {
                Id = 12,
                SemesterEnrollmentId = 3,
                CourseId = 4, // CS401 - 4 credits
                EnrollmentDate = seedDate
            }
        );
    }
}
