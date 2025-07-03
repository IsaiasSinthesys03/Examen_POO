using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExamenPOO.Application.DTOs;
using ExamenPOO.Application.Interfaces;

namespace ExamenPOO.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    /// <summary>
    /// Obtener todos los estudiantes con paginación
    /// </summary>
    /// <param name="pageNumber">Número de página (por defecto 1)</param>
    /// <param name="pageSize">Tamaño de página (por defecto 10)</param>
    /// <returns>Lista paginada de estudiantes</returns>
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<StudentDto>>> GetStudents(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        try
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var result = await _studentService.GetStudentsAsync(pageNumber, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obtener un estudiante por ID
    /// </summary>
    /// <param name="id">ID del estudiante</param>
    /// <returns>Estudiante encontrado</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<StudentDto>> GetStudent(int id)
    {
        try
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
            {
                return NotFound(new { message = "Estudiante no encontrado" });
            }
            return Ok(student);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obtener un estudiante por número de estudiante
    /// </summary>
    /// <param name="studentNumber">Número de estudiante</param>
    /// <returns>Estudiante encontrado</returns>
    [HttpGet("by-student-number/{studentNumber}")]
    public async Task<ActionResult<StudentDto>> GetStudentByStudentNumber(string studentNumber)
    {
        try
        {
            var student = await _studentService.GetStudentByStudentNumberAsync(studentNumber);
            if (student == null)
            {
                return NotFound(new { message = "Estudiante no encontrado" });
            }
            return Ok(student);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Actualizar un estudiante
    /// </summary>
    /// <param name="id">ID del estudiante a actualizar</param>
    /// <param name="updateStudentDto">Datos actualizados del estudiante</param>
    /// <returns>Estudiante actualizado</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<StudentDto>> UpdateStudent(int id, [FromBody] UpdateStudentDto updateStudentDto)
    {
        try
        {
            var result = await _studentService.UpdateStudentAsync(id, updateStudentDto);
            return Ok(result);
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
    /// Eliminar un estudiante (soft delete)
    /// </summary>
    /// <param name="id">ID del estudiante a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteStudent(int id)
    {
        try
        {
            var result = await _studentService.DeleteStudentAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Estudiante no encontrado" });
            }
            return Ok(new { message = "Estudiante eliminado exitosamente" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
///CAMBIOS MENORES A LOS NOMBRES DE LOS CONTROLLERS