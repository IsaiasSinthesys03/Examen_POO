using ExamenPOO.Application.DTOs;
using ExamenPOO.Application.Interfaces;
using ExamenPOO.Core.Entities;
using ExamenPOO.Core.Interfaces;

namespace ExamenPOO.Application.Services;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;

    public CourseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResultDto<CourseDto>> GetCoursesAsync(int pageNumber = 1, int pageSize = 10)
    {
        var courses = await _unitOfWork.Courses.GetPagedAsync(pageNumber, pageSize);
        var totalCount = await _unitOfWork.Courses.CountAsync();
        
        return new PagedResultDto<CourseDto>
        {
            Data = courses.Select(MapToDto),
            TotalItems = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<CourseDto?> GetCourseByIdAsync(int id)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(id);
        return course != null ? MapToDto(course) : null;
    }

    public async Task<CourseDto?> GetCourseByCourseCodeAsync(string courseCode)
    {
        var course = await _unitOfWork.Courses.GetByCourseCodeAsync(courseCode);
        return course != null ? MapToDto(course) : null;
    }

    public async Task<CourseDto> CreateCourseAsync(CreateCourseDto createCourseDto)
    {
        // Check if course code already exists
        if (await _unitOfWork.Courses.CourseCodeExistsAsync(createCourseDto.CourseCode))
            throw new InvalidOperationException("Course code already exists");

        var course = new Course
        {
            CourseCode = createCourseDto.CourseCode,
            Name = createCourseDto.Name,
            Description = createCourseDto.Description,
            Credits = createCourseDto.Credits,
            Department = createCourseDto.Department,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _unitOfWork.Courses.AddAsync(course);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(course);
    }

    public async Task<CourseDto> UpdateCourseAsync(int id, UpdateCourseDto updateCourseDto)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(id);
        if (course == null)
            throw new KeyNotFoundException("Course not found");

        // Check if course code is being changed and if new code already exists
        if (course.CourseCode != updateCourseDto.CourseCode)
        {
            if (await _unitOfWork.Courses.CourseCodeExistsAsync(updateCourseDto.CourseCode))
                throw new InvalidOperationException("Course code already exists");
        }

        // Update only non-null properties
        if (!string.IsNullOrEmpty(updateCourseDto.CourseCode))
            course.CourseCode = updateCourseDto.CourseCode;
        if (!string.IsNullOrEmpty(updateCourseDto.Name))
            course.Name = updateCourseDto.Name;
        if (updateCourseDto.Description != null)
            course.Description = updateCourseDto.Description;
        if (updateCourseDto.Credits.HasValue)
            course.Credits = updateCourseDto.Credits.Value;
        if (!string.IsNullOrEmpty(updateCourseDto.Department))
            course.Department = updateCourseDto.Department;
        if (updateCourseDto.IsActive.HasValue)
            course.IsActive = updateCourseDto.IsActive.Value;

        await _unitOfWork.Courses.Update(course);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(course);
    }

    public async Task<bool> DeleteCourseAsync(int id)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(id);
        if (course == null)
            return false;

        // Check if course has active enrollments
        var enrollmentCount = await _unitOfWork.EnrolledCourses.GetEnrollmentCountForCourseAsync(id);
        if (enrollmentCount > 0)
            throw new InvalidOperationException("Cannot delete course with active enrollments");

        await _unitOfWork.Courses.Delete(course);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CourseCodeExistsAsync(string courseCode)
    {
        return await _unitOfWork.Courses.CourseCodeExistsAsync(courseCode);
    }

    public async Task<IEnumerable<CourseDto>> GetByDepartmentAsync(string department)
    {
        var courses = await _unitOfWork.Courses.GetByDepartmentAsync(department);
        return courses.Select(MapToDto);
    }

    public async Task<IEnumerable<CourseDto>> GetByCreditRangeAsync(int minCredits, int maxCredits)
    {
        var courses = await _unitOfWork.Courses.GetByCreditRangeAsync(minCredits, maxCredits);
        return courses.Select(MapToDto);
    }

    public async Task<IEnumerable<CourseDto>> SearchByNameAsync(string searchTerm)
    {
        var courses = await _unitOfWork.Courses.SearchByNameAsync(searchTerm);
        return courses.Select(MapToDto);
    }

    public async Task<IEnumerable<CourseDto>> GetAvailableCoursesAsync()
    {
        var courses = await _unitOfWork.Courses.GetAvailableCoursesAsync();
        return courses.Select(MapToDto);
    }

    private static CourseDto MapToDto(Course course)
    {
        return new CourseDto
        {
            Id = course.Id,
            CourseCode = course.CourseCode,
            Name = course.Name,
            Description = course.Description,
            Credits = course.Credits,
            Department = course.Department,
            CreatedAt = course.CreatedAt,
            IsActive = course.IsActive
        };
    }
}
