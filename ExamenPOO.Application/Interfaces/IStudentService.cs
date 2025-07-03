using ExamenPOO.Application.DTOs;

namespace ExamenPOO.Application.Interfaces;

public interface IStudentService
{
    Task<PagedResultDto<StudentDto>> GetStudentsAsync(int pageNumber, int pageSize);
    Task<StudentDto?> GetStudentByIdAsync(int id);
    Task<StudentDto?> GetStudentByStudentNumberAsync(string studentNumber);
    Task<StudentDto> CreateStudentAsync(CreateStudentDto createStudentDto);
    Task<StudentDto> UpdateStudentAsync(int id, UpdateStudentDto updateStudentDto);
    Task<bool> DeleteStudentAsync(int id);
    Task<LoginResponseDto> AuthenticateAsync(LoginDto loginDto);
    Task<bool> StudentNumberExistsAsync(string studentNumber);
    Task<bool> EmailExistsAsync(string email);
}
