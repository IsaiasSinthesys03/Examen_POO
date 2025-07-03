using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExamenPOO.Application.DTOs;
using ExamenPOO.Application.Interfaces;

namespace ExamenPOO.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    /// <summary>
    /// Obtener todos los cursos con paginación
    /// </summary>
    /// <param name="pageNumber">Número de página (por defecto 1)</param>
    /// <param name="pageSize">Tamaño de página (por defecto 10)</param>
    /// <returns>Lista paginada de cursos</returns>
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<CourseDto>>> GetCourses(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        try
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var result = await _courseService.GetCoursesAsync(pageNumber, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obtener un curso por ID
    /// </summary>
    /// <param name="id">ID del curso</param>
    /// <returns>Curso encontrado</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<CourseDto>> GetCourse(int id)
    {
        try
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound(new { message = "Curso no encontrado" });
            }
            return Ok(course);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obtener un curso por código
    /// </summary>
    /// <param name="courseCode">Código del curso</param>
    /// <returns>Curso encontrado</returns>
    [HttpGet("by-code/{courseCode}")]
    public async Task<ActionResult<CourseDto>> GetCourseByCode(string courseCode)
    {
        try
        {
            var course = await _courseService.GetCourseByCourseCodeAsync(courseCode);
            if (course == null)
            {
                return NotFound(new { message = "Curso no encontrado" });
            }
            return Ok(course);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Crear un nuevo curso
    /// </summary>
    /// <param name="createCourseDto">Datos del nuevo curso</param>
    /// <returns>Curso creado</returns>
    [HttpPost]
    public async Task<ActionResult<CourseDto>> CreateCourse([FromBody] CreateCourseDto createCourseDto)
    {
        try
        {
            var result = await _courseService.CreateCourseAsync(createCourseDto);
            return CreatedAtAction(nameof(GetCourse), new { id = result.Id }, result);
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
    /// Actualizar un curso
    /// </summary>
    /// <param name="id">ID del curso a actualizar</param>
    /// <param name="updateCourseDto">Datos actualizados del curso</param>
    /// <returns>Curso actualizado</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<CourseDto>> UpdateCourse(int id, [FromBody] UpdateCourseDto updateCourseDto)
    {
        try
        {
            var result = await _courseService.UpdateCourseAsync(id, updateCourseDto);
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
    /// Eliminar un curso (soft delete)
    /// </summary>
    /// <param name="id">ID del curso a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCourse(int id)
    {
        try
        {
            var result = await _courseService.DeleteCourseAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Curso no encontrado" });
            }
            return Ok(new { message = "Curso eliminado exitosamente" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
///CAMBIOS MENORES A LOS NOMBRES DE LOS CONTROLLERS