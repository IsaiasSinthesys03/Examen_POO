using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExamenPOO.Application.DTOs;
using ExamenPOO.Application.Interfaces;

namespace ExamenPOO.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IStudentService _studentService;

    public AuthController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    /// <summary>
    /// Autenticar estudiante y obtener token JWT
    /// </summary>
    /// <param name="loginDto">Credenciales de estudiante</param>
    /// <returns>Token JWT y datos del estudiante</returns>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var result = await _studentService.AuthenticateAsync(loginDto);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Registrar un nuevo estudiante
    /// </summary>
    /// <param name="createStudentDto">Datos del nuevo estudiante</param>
    /// <returns>Estudiante creado</returns>
    [HttpPost("register")]
    public async Task<ActionResult<StudentDto>> Register([FromBody] CreateStudentDto createStudentDto)
    {
        try
        {
            var result = await _studentService.CreateStudentAsync(createStudentDto);
            return CreatedAtAction("GetStudent", "Students", new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Verificar si un número de estudiante existe
    /// </summary>
    /// <param name="studentNumber">Número de estudiante a verificar</param>
    /// <returns>True si existe, False si no existe</returns>
    [HttpGet("check-student-number/{studentNumber}")]
    public async Task<ActionResult<bool>> CheckStudentNumber(string studentNumber)
    {
        try
        {
            var exists = await _studentService.StudentNumberExistsAsync(studentNumber);
            return Ok(new { exists });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Verificar si un email existe
    /// </summary>
    /// <param name="email">Email a verificar</param>
    /// <returns>True si existe, False si no existe</returns>
    [HttpGet("check-email/{email}")]
    public async Task<ActionResult<bool>> CheckEmail(string email)
    {
        try
        {
            var exists = await _studentService.EmailExistsAsync(email);
            return Ok(new { exists });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
///CAMBIOS MENORES A LOS NOMBRES DE LOS CONTROLLERS