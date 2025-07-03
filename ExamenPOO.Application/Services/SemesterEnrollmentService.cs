using ExamenPOO.Application.DTOs;
using ExamenPOO.Application.Interfaces;
using ExamenPOO.Core.Entities;
using ExamenPOO.Core.Interfaces;

namespace ExamenPOO.Application.Services;

public class SemesterEnrollmentService : ISemesterEnrollmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public SemesterEnrollmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResultDto<SemesterEnrollmentDto>> GetSemesterEnrollmentsAsync(int pageNumber = 1, int pageSize = 10)
    {
        var enrollments = await _unitOfWork.SemesterEnrollments.GetPagedAsync(pageNumber, pageSize);
        var totalCount = await _unitOfWork.SemesterEnrollments.CountAsync();
        
        return new PagedResultDto<SemesterEnrollmentDto>
        {
            Data = enrollments.Select(MapToDto),
            TotalItems = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<SemesterEnrollmentDto?> GetSemesterEnrollmentByIdAsync(int id)
    {
        var enrollment = await _unitOfWork.SemesterEnrollments.GetWithEnrolledCoursesAsync(id);
        return enrollment != null ? MapToDto(enrollment) : null;
    }

    public async Task<SemesterEnrollmentDto?> GetByStudentAndSemesterAsync(int studentId, string semester, int year)
    {
        var enrollment = await _unitOfWork.SemesterEnrollments.GetByStudentAndSemesterAsync(studentId, semester, year);
        return enrollment != null ? MapToDto(enrollment) : null;
    }

    public async Task<SemesterEnrollmentDto> CreateSemesterEnrollmentAsync(CreateSemesterEnrollmentDto createDto)
    {
        // Validate student exists
        var student = await _unitOfWork.Students.GetByIdAsync(createDto.StudentId);
        if (student == null)
            throw new KeyNotFoundException("Student not found");

        // Check if enrollment already exists for this student and semester
        if (await _unitOfWork.SemesterEnrollments.ExistsForStudentAndSemesterAsync(
            createDto.StudentId, createDto.Semester, createDto.Year))
        {
            throw new InvalidOperationException("Student is already enrolled for this semester");
        }

        var enrollment = new SemesterEnrollment
        {
            StudentId = createDto.StudentId,
            Semester = createDto.Semester,
            Year = createDto.Year,
            MaxCredits = createDto.MaxCredits,
            EnrollmentDate = DateTime.UtcNow
        };

        await _unitOfWork.SemesterEnrollments.AddAsync(enrollment);
        await _unitOfWork.SaveChangesAsync();

        // Reload with student data
        var createdEnrollment = await _unitOfWork.SemesterEnrollments.GetWithEnrolledCoursesAsync(enrollment.Id);
        return MapToDto(createdEnrollment!);
    }

    public async Task<SemesterEnrollmentDto> UpdateSemesterEnrollmentAsync(int id, UpdateSemesterEnrollmentDto updateDto)
    {
        var enrollment = await _unitOfWork.SemesterEnrollments.GetWithEnrolledCoursesAsync(id);
        if (enrollment == null)
            throw new KeyNotFoundException("Semester enrollment not found");

        // Check credit limit constraints
        var currentCredits = enrollment.GetTotalCredits();
        if (updateDto.MaxCredits.HasValue && updateDto.MaxCredits.Value < currentCredits)
        {
            throw new InvalidOperationException($"Cannot set max credits below current enrolled credits ({currentCredits})");
        }

        if (updateDto.MaxCredits.HasValue)
            enrollment.MaxCredits = updateDto.MaxCredits.Value;

        await _unitOfWork.SemesterEnrollments.Update(enrollment);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(enrollment);
    }

    public async Task<bool> DeleteSemesterEnrollmentAsync(int id)
    {
        var enrollment = await _unitOfWork.SemesterEnrollments.GetWithEnrolledCoursesAsync(id);
        if (enrollment == null)
            return false;

        // Check if there are enrolled courses
        if (enrollment.EnrolledCourses.Any())
        {
            throw new InvalidOperationException("Cannot delete semester enrollment with enrolled courses. Remove courses first.");
        }

        await _unitOfWork.SemesterEnrollments.Delete(enrollment);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<SemesterEnrollmentDto>> GetByStudentIdAsync(int studentId)
    {
        var enrollments = await _unitOfWork.SemesterEnrollments.GetByStudentIdAsync(studentId);
        return enrollments.Select(MapToDto);
    }

    public async Task<IEnumerable<SemesterEnrollmentDto>> GetBySemesterAsync(string semester, int year)
    {
        var enrollments = await _unitOfWork.SemesterEnrollments.GetBySemesterAsync(semester, year);
        return enrollments.Select(MapToDto);
    }

    public async Task<EnrolledCourseDto> EnrollInCourseAsync(int semesterEnrollmentId, int courseId)
    {
        // Get semester enrollment with courses
        var semesterEnrollment = await _unitOfWork.SemesterEnrollments.GetWithEnrolledCoursesAsync(semesterEnrollmentId);
        if (semesterEnrollment == null)
            throw new KeyNotFoundException("Semester enrollment not found");

        // Get course
        var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
        if (course == null)
            throw new KeyNotFoundException("Course not found");

        if (!course.IsActive)
            throw new InvalidOperationException("Course is not active");

        // Check if already enrolled
        if (await _unitOfWork.EnrolledCourses.IsStudentEnrolledInCourseAsync(semesterEnrollmentId, courseId))
            throw new InvalidOperationException("Student is already enrolled in this course for this semester");

        // Check credit limit
        var currentCredits = semesterEnrollment.GetTotalCredits();
        if (currentCredits + course.Credits > semesterEnrollment.MaxCredits)
        {
            throw new InvalidOperationException($"Enrolling in this course would exceed the credit limit. Current: {currentCredits}, Course: {course.Credits}, Max: {semesterEnrollment.MaxCredits}");
        }

        var enrolledCourse = new EnrolledCourse
        {
            SemesterEnrollmentId = semesterEnrollmentId,
            CourseId = courseId,
            EnrollmentDate = DateTime.UtcNow
        };

        await _unitOfWork.EnrolledCourses.AddAsync(enrolledCourse);
        await _unitOfWork.SaveChangesAsync();

        // Reload with full data
        var createdEnrollment = await _unitOfWork.EnrolledCourses.GetByIdAsync(enrolledCourse.Id);
        return MapEnrolledCourseToDto(createdEnrollment!);
    }

    public async Task<bool> UnenrollFromCourseAsync(int semesterEnrollmentId, int courseId)
    {
        var enrolledCourse = await _unitOfWork.EnrolledCourses.GetByStudentAndCourseAsync(semesterEnrollmentId, courseId);
        if (enrolledCourse == null)
            return false;

        await _unitOfWork.EnrolledCourses.Delete(enrolledCourse);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetTotalCreditsForSemesterAsync(int semesterEnrollmentId)
    {
        return await _unitOfWork.SemesterEnrollments.GetTotalCreditsForSemesterAsync(semesterEnrollmentId);
    }

    public async Task<PagedResultDto<SemesterEnrollmentDto>> GetSemesterEnrollmentsByStudentIdAsync(int studentId, int pageNumber = 1, int pageSize = 10)
    {
        var enrollments = await _unitOfWork.SemesterEnrollments.GetByStudentIdAsync(studentId);
        var pagedEnrollments = enrollments
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
        
        return new PagedResultDto<SemesterEnrollmentDto>
        {
            Data = pagedEnrollments.Select(MapToDto),
            TotalItems = enrollments.Count(),
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<EnrolledCourseDto> EnrollCourseAsync(int semesterEnrollmentId, EnrollCourseDto enrollCourseDto)
    {
        return await EnrollInCourseAsync(semesterEnrollmentId, enrollCourseDto.CourseId);
    }

    public async Task<bool> UnenrollCourseAsync(int semesterEnrollmentId, int courseId)
    {
        return await UnenrollFromCourseAsync(semesterEnrollmentId, courseId);
    }

    public async Task<PagedResultDto<EnrolledCourseDto>> GetEnrolledCoursesAsync(int semesterEnrollmentId, int pageNumber = 1, int pageSize = 10)
    {
        var enrolledCourses = await _unitOfWork.EnrolledCourses.GetBySemesterEnrollmentIdAsync(semesterEnrollmentId);
        var pagedCourses = enrolledCourses
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
        
        return new PagedResultDto<EnrolledCourseDto>
        {
            Data = pagedCourses.Select(MapEnrolledCourseToDto),
            TotalItems = enrolledCourses.Count(),
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    private static SemesterEnrollmentDto MapToDto(SemesterEnrollment enrollment)
    {
        return new SemesterEnrollmentDto
        {
            Id = enrollment.Id,
            StudentId = enrollment.StudentId,
            StudentName = $"{enrollment.Student?.FirstName} {enrollment.Student?.LastName}",
            StudentNumber = enrollment.Student?.StudentNumber ?? "",
            Semester = enrollment.Semester,
            Year = enrollment.Year,
            MaxCredits = enrollment.MaxCredits,
            TotalCredits = enrollment.GetTotalCredits(),
            EnrollmentDate = enrollment.EnrollmentDate,
            EnrolledCourses = enrollment.EnrolledCourses?.Select(MapEnrolledCourseToDto).ToList() ?? new List<EnrolledCourseDto>()
        };
    }

    private static EnrolledCourseDto MapEnrolledCourseToDto(EnrolledCourse enrolledCourse)
    {
        return new EnrolledCourseDto
        {
            Id = enrolledCourse.Id,
            SemesterEnrollmentId = enrolledCourse.SemesterEnrollmentId,
            CourseId = enrolledCourse.CourseId,
            CourseCode = enrolledCourse.Course?.CourseCode ?? "",
            CourseName = enrolledCourse.Course?.Name ?? "",
            Credits = enrolledCourse.Course?.Credits ?? 0,
            Department = enrolledCourse.Course?.Department ?? "",
            EnrollmentDate = enrolledCourse.EnrollmentDate
        };
    }
}
