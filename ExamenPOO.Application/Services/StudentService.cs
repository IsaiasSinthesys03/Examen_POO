using ExamenPOO.Application.DTOs;
using ExamenPOO.Application.Interfaces;
using ExamenPOO.Core.Entities;
using ExamenPOO.Core.Interfaces;
using ExamenPOO.Core.Services;
using ExamenPOO.Core.ValueObjects;

namespace ExamenPOO.Application.Services;

public class StudentService : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHashingService _passwordHashingService;

    public StudentService(IUnitOfWork unitOfWork, IJwtService jwtService, IPasswordHashingService passwordHashingService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _passwordHashingService = passwordHashingService;
    }

    public async Task<PagedResultDto<StudentDto>> GetStudentsAsync(int pageNumber, int pageSize)
    {
        var students = await _unitOfWork.Students.GetPagedAsync(pageNumber, pageSize);
        var totalCount = await _unitOfWork.Students.GetCountAsync();

        var studentDtos = students.Select(s => new StudentDto
        {
            Id = s.Id,
            StudentNumber = s.StudentNumber,
            Email = s.Email,
            FirstName = s.FirstName,
            LastName = s.LastName,
            PhoneNumber = s.PhoneNumber,
            Address = s.Address,
            BirthDate = s.BirthDate,
            Career = s.Career,
            IsActive = s.IsActive,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
        }).ToList();

        return new PagedResultDto<StudentDto>
        {
            Data = studentDtos,
            TotalItems = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<StudentDto?> GetStudentByIdAsync(int id)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(id);
        if (student == null) return null;

        return new StudentDto
        {
            Id = student.Id,
            StudentNumber = student.StudentNumber,
            Email = student.Email,
            FirstName = student.FirstName,
            LastName = student.LastName,
            PhoneNumber = student.PhoneNumber,
            Address = student.Address,
            BirthDate = student.BirthDate,
            Career = student.Career,
            IsActive = student.IsActive,
            CreatedAt = student.CreatedAt,
            UpdatedAt = student.UpdatedAt
        };
    }

    public async Task<StudentDto?> GetStudentByStudentNumberAsync(string studentNumber)
    {
        var student = await _unitOfWork.Students.GetByStudentNumberAsync(studentNumber);
        if (student == null) return null;

        return new StudentDto
        {
            Id = student.Id,
            StudentNumber = student.StudentNumber,
            Email = student.Email,
            FirstName = student.FirstName,
            LastName = student.LastName,
            PhoneNumber = student.PhoneNumber,
            Address = student.Address,
            BirthDate = student.BirthDate,
            Career = student.Career,
            IsActive = student.IsActive,
            CreatedAt = student.CreatedAt,
            UpdatedAt = student.UpdatedAt
        };
    }

    public async Task<StudentDto> CreateStudentAsync(CreateStudentDto createStudentDto)
    {
        // Create Value Objects
        var studentNumber = new StudentNumber(createStudentDto.StudentNumber);
        var email = new Email(createStudentDto.Email);
        
        // Validar que no exista el número de estudiante
        if (await StudentNumberExistsAsync(createStudentDto.StudentNumber))
        {
            throw new InvalidOperationException("El número de estudiante ya existe");
        }

        // Validar que no exista el email
        if (await EmailExistsAsync(createStudentDto.Email))
        {
            throw new InvalidOperationException("El email ya existe");
        }

        // Create student using domain constructor
        var student = new Student(
            studentNumber,
            email,
            createStudentDto.FirstName,
            createStudentDto.LastName,
            createStudentDto.BirthDate ?? DateTime.UtcNow.AddYears(-18),
            createStudentDto.Career,
            createStudentDto.PhoneNumber,
            createStudentDto.Address
        );

        // Set password using domain method
        var hashedPassword = _passwordHashingService.HashPassword(createStudentDto.Password);
        student.SetPassword(hashedPassword);

        await _unitOfWork.Students.AddAsync(student);
        await _unitOfWork.SaveChangesAsync();

        return new StudentDto
        {
            Id = student.Id,
            StudentNumber = student.StudentNumber,
            Email = student.Email,
            FirstName = student.FirstName,
            LastName = student.LastName,
            PhoneNumber = student.PhoneNumber,
            Address = student.Address,
            BirthDate = student.BirthDate,
            Career = student.Career,
            IsActive = student.IsActive,
            CreatedAt = student.CreatedAt,
            UpdatedAt = student.UpdatedAt,
            FullName = student.FullName
        };
    }

    public async Task<StudentDto> UpdateStudentAsync(int id, UpdateStudentDto updateStudentDto)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(id);
        if (student == null)
        {
            throw new InvalidOperationException("Estudiante no encontrado");
        }

        // Validar email si se está cambiando
        if (!string.IsNullOrEmpty(updateStudentDto.Email) && 
            updateStudentDto.Email != student.Email.Value && 
            await EmailExistsAsync(updateStudentDto.Email))
        {
            throw new InvalidOperationException("El email ya existe");
        }

        // Use domain methods for updates
        if (!string.IsNullOrEmpty(updateStudentDto.Email))
        {
            var newEmail = new Email(updateStudentDto.Email);
            student.UpdateEmail(newEmail);
        }
        
        if (!string.IsNullOrEmpty(updateStudentDto.FirstName) || 
            !string.IsNullOrEmpty(updateStudentDto.LastName) ||
            updateStudentDto.PhoneNumber != null ||
            updateStudentDto.Address != null)
        {
            student.UpdatePersonalInfo(
                updateStudentDto.FirstName ?? student.FirstName,
                updateStudentDto.LastName ?? student.LastName,
                updateStudentDto.PhoneNumber ?? student.PhoneNumber,
                updateStudentDto.Address ?? student.Address
            );
        }
        
        if (updateStudentDto.IsActive.HasValue)
        {
            if (updateStudentDto.IsActive.Value)
                student.Activate();
            else
                student.Deactivate();
        }

        await _unitOfWork.Students.UpdateAsync(student);
        await _unitOfWork.SaveChangesAsync();

        return new StudentDto
        {
            Id = student.Id,
            StudentNumber = student.StudentNumber,
            Email = student.Email,
            FirstName = student.FirstName,
            LastName = student.LastName,
            PhoneNumber = student.PhoneNumber,
            Address = student.Address,
            BirthDate = student.BirthDate,
            Career = student.Career,
            IsActive = student.IsActive,
            CreatedAt = student.CreatedAt,
            UpdatedAt = student.UpdatedAt,
            FullName = student.FullName
        };
    }

    public async Task<bool> DeleteStudentAsync(int id)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(id);
        if (student == null) return false;

        // Verificar si el estudiante tiene inscripciones semestrales activas
        var hasActiveEnrollments = await _unitOfWork.SemesterEnrollments.HasActiveEnrollmentAsync(id);
        
        if (hasActiveEnrollments)
        {
            throw new InvalidOperationException(
                "No se puede eliminar el estudiante porque tiene inscripciones semestrales activas. " +
                "Primero debe completar o cancelar todas sus inscripciones.");
        }

        // Soft delete using domain method
        student.Deactivate();
        
        await _unitOfWork.Students.UpdateAsync(student);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<LoginResponseDto> AuthenticateAsync(LoginDto loginDto)
    {
        Student? student = null;

        // Buscar por email o número de estudiante
        if (!string.IsNullOrEmpty(loginDto.Email))
        {
            student = await _unitOfWork.Students.GetByEmailAsync(loginDto.Email);
        }
        else if (!string.IsNullOrEmpty(loginDto.StudentNumber))
        {
            student = await _unitOfWork.Students.GetByStudentNumberAsync(loginDto.StudentNumber);
        }
        else
        {
            throw new UnauthorizedAccessException("Debe proporcionar email o número de estudiante");
        }

        if (student == null || !student.IsActive)
        {
            throw new UnauthorizedAccessException("Credenciales inválidas");
        }

        if (!_passwordHashingService.VerifyPassword(loginDto.Password, student.PasswordHash))
        {
            throw new UnauthorizedAccessException("Credenciales inválidas");
        }

        var token = _jwtService.GenerateToken(student.Id, student.Email);
        var expiresAt = DateTime.UtcNow.AddHours(24); // Token válido por 24 horas

        return new LoginResponseDto
        {
            Id = student.Id,
            StudentNumber = student.StudentNumber,
            Email = student.Email,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Career = student.Career,
            Token = token,
            ExpiresAt = expiresAt,
            Student = new StudentDto
            {
                Id = student.Id,
                StudentNumber = student.StudentNumber,
                Email = student.Email,
                FirstName = student.FirstName,
                LastName = student.LastName,
                PhoneNumber = student.PhoneNumber,
                Address = student.Address,
                BirthDate = student.BirthDate,
                Career = student.Career,
                IsActive = student.IsActive,
                CreatedAt = student.CreatedAt,
                UpdatedAt = student.UpdatedAt
            }
        };
    }

    public async Task<bool> StudentNumberExistsAsync(string studentNumber)
    {
        return await _unitOfWork.Students.StudentNumberExistsAsync(studentNumber);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _unitOfWork.Students.EmailExistsAsync(email);
    }
}
