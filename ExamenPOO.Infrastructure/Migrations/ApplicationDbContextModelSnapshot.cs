﻿// <auto-generated />
using System;
using ExamenPOO.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ExamenPOO.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ExamenPOO.Core.Entities.Course", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CourseCode")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("CourseName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreditHours")
                        .HasColumnType("int");

                    b.Property<int>("Credits")
                        .HasColumnType("int");

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Prerequisites")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CourseCode")
                        .IsUnique();

                    b.ToTable("Courses");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CourseCode = "CS101",
                            CourseName = "",
                            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            CreditHours = 0,
                            Credits = 4,
                            Department = "Computer Science",
                            Description = "Basic programming concepts using C# and object-oriented programming principles",
                            IsActive = true,
                            Name = "Introduction to Programming"
                        },
                        new
                        {
                            Id = 2,
                            CourseCode = "CS201",
                            CourseName = "",
                            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            CreditHours = 0,
                            Credits = 4,
                            Department = "Computer Science",
                            Description = "Fundamental data structures and algorithmic problem-solving techniques",
                            IsActive = true,
                            Name = "Data Structures and Algorithms"
                        },
                        new
                        {
                            Id = 3,
                            CourseCode = "CS301",
                            CourseName = "",
                            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            CreditHours = 0,
                            Credits = 3,
                            Department = "Computer Science",
                            Description = "Design and implementation of relational database systems",
                            IsActive = true,
                            Name = "Database Systems"
                        },
                        new
                        {
                            Id = 4,
                            CourseCode = "CS401",
                            CourseName = "",
                            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            CreditHours = 0,
                            Credits = 4,
                            Department = "Computer Science",
                            Description = "Software development methodologies and project management",
                            IsActive = true,
                            Name = "Software Engineering"
                        },
                        new
                        {
                            Id = 5,
                            CourseCode = "CS501",
                            CourseName = "",
                            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            CreditHours = 0,
                            Credits = 3,
                            Department = "Computer Science",
                            Description = "Modern web development using ASP.NET Core and related technologies",
                            IsActive = true,
                            Name = "Web Development"
                        },
                        new
                        {
                            Id = 6,
                            CourseCode = "MATH201",
                            CourseName = "",
                            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            CreditHours = 0,
                            Credits = 3,
                            Department = "Mathematics",
                            Description = "Mathematical foundations for computer science",
                            IsActive = true,
                            Name = "Discrete Mathematics"
                        });
                });

            modelBuilder.Entity("ExamenPOO.Core.Entities.EnrolledCourse", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<int?>("CourseId1")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DropDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("EnrollmentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Grade")
                        .HasMaxLength(2)
                        .HasColumnType("nvarchar(2)");

                    b.Property<DateTime?>("GradeDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDropped")
                        .HasColumnType("bit");

                    b.Property<int>("SemesterEnrollmentId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("CourseId1");

                    b.HasIndex("SemesterEnrollmentId", "CourseId")
                        .IsUnique();

                    b.ToTable("EnrolledCourses");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CourseId = 1,
                            EnrollmentDate = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            IsDropped = false,
                            SemesterEnrollmentId = 1
                        },
                        new
                        {
                            Id = 2,
                            CourseId = 2,
                            EnrollmentDate = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            IsDropped = false,
                            SemesterEnrollmentId = 1
                        },
                        new
                        {
                            Id = 3,
                            CourseId = 3,
                            EnrollmentDate = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            IsDropped = false,
                            SemesterEnrollmentId = 1
                        },
                        new
                        {
                            Id = 4,
                            CourseId = 6,
                            EnrollmentDate = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            IsDropped = false,
                            SemesterEnrollmentId = 1
                        },
                        new
                        {
                            Id = 5,
                            CourseId = 4,
                            EnrollmentDate = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            IsDropped = false,
                            SemesterEnrollmentId = 1
                        },
                        new
                        {
                            Id = 6,
                            CourseId = 1,
                            EnrollmentDate = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            IsDropped = false,
                            SemesterEnrollmentId = 2
                        },
                        new
                        {
                            Id = 7,
                            CourseId = 3,
                            EnrollmentDate = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            IsDropped = false,
                            SemesterEnrollmentId = 2
                        },
                        new
                        {
                            Id = 8,
                            CourseId = 5,
                            EnrollmentDate = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            IsDropped = false,
                            SemesterEnrollmentId = 2
                        },
                        new
                        {
                            Id = 9,
                            CourseId = 4,
                            EnrollmentDate = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            IsDropped = false,
                            SemesterEnrollmentId = 2
                        },
                        new
                        {
                            Id = 10,
                            CourseId = 1,
                            EnrollmentDate = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            IsDropped = false,
                            SemesterEnrollmentId = 3
                        },
                        new
                        {
                            Id = 11,
                            CourseId = 5,
                            EnrollmentDate = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            IsDropped = false,
                            SemesterEnrollmentId = 3
                        },
                        new
                        {
                            Id = 12,
                            CourseId = 4,
                            EnrollmentDate = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            IsDropped = false,
                            SemesterEnrollmentId = 3
                        });
                });

            modelBuilder.Entity("ExamenPOO.Core.Entities.SemesterEnrollment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("EnrollmentDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<int>("MaxCredits")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(21);

                    b.Property<string>("Semester")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("StudentId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("StudentId", "Semester", "Year")
                        .IsUnique();

                    b.ToTable("SemesterEnrollments");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedAt = new DateTime(2025, 7, 3, 18, 54, 34, 276, DateTimeKind.Utc).AddTicks(7336),
                            EndDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            EnrollmentDate = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            IsActive = true,
                            MaxCredits = 18,
                            Semester = "Fall",
                            StartDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            StudentId = 1,
                            Year = 2024
                        },
                        new
                        {
                            Id = 2,
                            CreatedAt = new DateTime(2025, 7, 3, 18, 54, 34, 276, DateTimeKind.Utc).AddTicks(8998),
                            EndDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            EnrollmentDate = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            IsActive = true,
                            MaxCredits = 21,
                            Semester = "Fall",
                            StartDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            StudentId = 2,
                            Year = 2024
                        },
                        new
                        {
                            Id = 3,
                            CreatedAt = new DateTime(2025, 7, 3, 18, 54, 34, 276, DateTimeKind.Utc).AddTicks(9001),
                            EndDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            EnrollmentDate = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            IsActive = true,
                            MaxCredits = 15,
                            Semester = "Fall",
                            StartDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            StudentId = 3,
                            Year = 2024
                        });
                });

            modelBuilder.Entity("ExamenPOO.Core.Entities.Student", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Career")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StudentNumber")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("StudentNumber")
                        .IsUnique();

                    b.ToTable("Students");
                });

            modelBuilder.Entity("ExamenPOO.Core.Entities.EnrolledCourse", b =>
                {
                    b.HasOne("ExamenPOO.Core.Entities.Course", "Course")
                        .WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ExamenPOO.Core.Entities.Course", null)
                        .WithMany("EnrolledCourses")
                        .HasForeignKey("CourseId1");

                    b.HasOne("ExamenPOO.Core.Entities.SemesterEnrollment", "SemesterEnrollment")
                        .WithMany("EnrolledCourses")
                        .HasForeignKey("SemesterEnrollmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("SemesterEnrollment");
                });

            modelBuilder.Entity("ExamenPOO.Core.Entities.SemesterEnrollment", b =>
                {
                    b.HasOne("ExamenPOO.Core.Entities.Student", "Student")
                        .WithMany("SemesterEnrollments")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Student");
                });

            modelBuilder.Entity("ExamenPOO.Core.Entities.Course", b =>
                {
                    b.Navigation("EnrolledCourses");
                });

            modelBuilder.Entity("ExamenPOO.Core.Entities.SemesterEnrollment", b =>
                {
                    b.Navigation("EnrolledCourses");
                });

            modelBuilder.Entity("ExamenPOO.Core.Entities.Student", b =>
                {
                    b.Navigation("SemesterEnrollments");
                });
#pragma warning restore 612, 618
        }
    }
}
