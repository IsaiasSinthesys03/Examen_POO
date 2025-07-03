using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExamenPOO.Application.DTOs;
using ExamenPOO.Application.Interfaces;

namespace ExamenPOO.API.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class SemesterEnrollmentsController : ControllerBase
{
    private readonly ISemesterEnrollmentService _semesterEnrollmentService;
    private readonly IStudentService _studentService;
    private readonly ICourseService _courseService;

    public SemesterEnrollmentsController(
        ISemesterEnrollmentService semesterEnrollmentService,
        IStudentService studentService,
        ICourseService courseService)
    {
        _semesterEnrollmentService = semesterEnrollmentService;
        _studentService = studentService;
        _courseService = courseService;
    }

    /// <summary>
    /// Crear inscripción semestral para un estudiante (Endpoint requerido)
    /// </summary>
    /// <param name="studentId">ID del estudiante</param>
    /// <param name="createDto">Datos de la inscripción semestral</param>
    /// <returns>Inscripción semestral creada</returns>
    [HttpPost("students/{studentId}/semesters")]
    public async Task<ActionResult<SemesterEnrollmentDto>> CreateSemesterEnrollment(
        int studentId, 
        [FromBody] CreateSemesterEnrollmentDto createDto)
    {
        try
        {
            // Verificar que el estudiante existe
            var student = await _studentService.GetStudentByIdAsync(studentId);
            if (student == null)
            {
                return NotFound(new 
                { 
                    message = $"Estudiante con ID {studentId} no encontrado",
                    errorCode = "STUDENT_NOT_FOUND"
                });
            }

            // Asignar el StudentId del parámetro de ruta
            createDto.StudentId = studentId;

            var result = await _semesterEnrollmentService.CreateSemesterEnrollmentAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetSemesterEnrollment), 
                new { id = result.Id }, 
                result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new 
            { 
                message = ex.Message,
                errorCode = "BUSINESS_RULE_VIOLATION"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new 
            { 
                message = "Error interno del servidor",
                detail = ex.Message
            });
        }
    }

    /// <summary>
    /// Inscribir una materia a un semestre específico (Endpoint requerido)
    /// </summary>
    /// <param name="semesterId">ID del semestre</param>
    /// <param name="courseId">ID de la materia</param>
    /// <returns>Resultado de la inscripción</returns>
    [HttpPost("semesters/{semesterId}/courses/{courseId}")]
    public async Task<ActionResult> EnrollCourse(int semesterId, int courseId)
    {
        try
        {
            // Verificar que el semestre existe
            var semester = await _semesterEnrollmentService.GetSemesterEnrollmentByIdAsync(semesterId);
            if (semester == null)
            {
                return NotFound(new 
                { 
                    message = $"Inscripción semestral con ID {semesterId} no encontrada",
                    errorCode = "SEMESTER_ENROLLMENT_NOT_FOUND"
                });
            }

            // Verificar que la materia existe
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                return NotFound(new 
                { 
                    message = $"Materia con ID {courseId} no encontrada",
                    errorCode = "COURSE_NOT_FOUND"
                });
            }

            var enrollDto = new EnrollCourseDto { CourseId = courseId };
            var result = await _semesterEnrollmentService.EnrollCourseAsync(semesterId, enrollDto);
            
            return Ok(new 
            { 
                message = $"Estudiante inscrito exitosamente en la materia {course.CourseName}",
                semesterId = semesterId,
                courseId = courseId,
                courseName = course.CourseName,
                creditHours = course.CreditHours,
                enrolledCourse = result
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new 
            { 
                message = ex.Message,
                errorCode = "ENROLLMENT_BUSINESS_RULE_VIOLATION"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new 
            { 
                message = "Error interno del servidor",
                detail = ex.Message
            });
        }
    }

    /// <summary>
    /// Desinscribir una materia de un semestre
    /// </summary>
    /// <param name="semesterId">ID del semestre</param>
    /// <param name="courseId">ID de la materia</param>
    /// <returns>Resultado de la desinscripción</returns>
    [HttpDelete("semesters/{semesterId}/courses/{courseId}")]
    public async Task<ActionResult> UnenrollCourse(int semesterId, int courseId)
    {
        try
        {
            await _semesterEnrollmentService.UnenrollCourseAsync(semesterId, courseId);
            
            return Ok(new 
            { 
                message = "Estudiante desinscrito exitosamente de la materia",
                semesterId = semesterId,
                courseId = courseId
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new 
            { 
                message = ex.Message,
                errorCode = "ENROLLMENT_NOT_FOUND"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new 
            { 
                message = "Error interno del servidor",
                detail = ex.Message
            });
        }
    }

    /// <summary>
    /// Obtener todas las inscripciones semestrales
    /// </summary>
    /// <param name="pageNumber">Número de página</param>
    /// <param name="pageSize">Tamaño de página</param>
    /// <returns>Lista paginada de inscripciones semestrales</returns>
    [HttpGet("semesterenrollments")]
    public async Task<ActionResult<PagedResultDto<SemesterEnrollmentDto>>> GetSemesterEnrollments(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var result = await _semesterEnrollmentService.GetSemesterEnrollmentsAsync(pageNumber, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new 
            { 
                message = "Error interno del servidor",
                detail = ex.Message
            });
        }
    }

    /// <summary>
    /// Obtener una inscripción semestral por ID
    /// </summary>
    /// <param name="id">ID de la inscripción semestral</param>
    /// <returns>Inscripción semestral encontrada</returns>
    [HttpGet("semesterenrollments/{id}")]
    public async Task<ActionResult<SemesterEnrollmentDto>> GetSemesterEnrollment(int id)
    {
        try
        {
            var result = await _semesterEnrollmentService.GetSemesterEnrollmentByIdAsync(id);
            if (result == null)
            {
                return NotFound(new 
                { 
                    message = $"Inscripción semestral con ID {id} no encontrada",
                    errorCode = "SEMESTER_ENROLLMENT_NOT_FOUND"
                });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new 
            { 
                message = "Error interno del servidor",
                detail = ex.Message
            });
        }
    }

    /// <summary>
    /// Actualizar una inscripción semestral
    /// </summary>
    /// <param name="id">ID de la inscripción semestral</param>
    /// <param name="updateDto">Datos actualizados</param>
    /// <returns>Inscripción semestral actualizada</returns>
    [HttpPut("semesterenrollments/{id}")]
    public async Task<ActionResult<SemesterEnrollmentDto>> UpdateSemesterEnrollment(
        int id, 
        [FromBody] UpdateSemesterEnrollmentDto updateDto)
    {
        try
        {
            var result = await _semesterEnrollmentService.UpdateSemesterEnrollmentAsync(id, updateDto);
            if (result == null)
            {
                return NotFound(new 
                { 
                    message = $"Inscripción semestral con ID {id} no encontrada",
                    errorCode = "SEMESTER_ENROLLMENT_NOT_FOUND"
                });
            }

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new 
            { 
                message = ex.Message,
                errorCode = "BUSINESS_RULE_VIOLATION"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new 
            { 
                message = "Error interno del servidor",
                detail = ex.Message
            });
        }
    }

    /// <summary>
    /// Eliminar una inscripción semestral
    /// </summary>
    /// <param name="id">ID de la inscripción semestral</param>
    /// <returns>Resultado de la eliminación</returns>
    [HttpDelete("semesterenrollments/{id}")]
    public async Task<ActionResult> DeleteSemesterEnrollment(int id)
    {
        try
        {
            var result = await _semesterEnrollmentService.DeleteSemesterEnrollmentAsync(id);
            if (!result)
            {
                return NotFound(new 
                { 
                    message = $"Inscripción semestral con ID {id} no encontrada",
                    errorCode = "SEMESTER_ENROLLMENT_NOT_FOUND"
                });
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new 
            { 
                message = ex.Message,
                errorCode = "BUSINESS_RULE_VIOLATION"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new 
            { 
                message = "Error interno del servidor",
                detail = ex.Message
            });
        }
    }
}
///CAMBIOS MENORES A LOS NOMBRES DE LOS CONTROLLERS