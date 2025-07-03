using Microsoft.AspNetCore.Mvc;
using ExamenPOO.Application.DTOs;
using ExamenPOO.Application.Validation;
using System.ComponentModel.DataAnnotations;

namespace ExamenPOO.API.Controllers;

/// <summary>
/// Controlador de prueba para demostrar el sistema de validación estricta de tipos
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ValidationTestController : ControllerBase
{
    private readonly ILogger<ValidationTestController> _logger;

    public ValidationTestController(ILogger<ValidationTestController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Endpoint de prueba para validación estricta de estudiantes
    /// </summary>
    /// <param name="student">Datos del estudiante con validación estricta</param>
    /// <returns>Resultado de la validación</returns>
    [HttpPost("student")]
    public ActionResult<object> TestStudentValidation([FromBody] CreateStudentDto student)
    {
        _logger.LogInformation("Student validation test passed for: {StudentNumber}", student.StudentNumber);
        
        return Ok(new
        {
            message = "Validation successful! All data types are correct.",
            receivedData = new
            {
                studentNumber = student.StudentNumber,
                email = student.Email,
                firstName = student.FirstName,
                lastName = student.LastName,
                career = student.Career,
                birthDate = student.BirthDate,
                phoneNumber = student.PhoneNumber,
                address = student.Address
            },
            validationPassed = true,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Endpoint de prueba para validación estricta de cursos
    /// </summary>
    /// <param name="course">Datos del curso con validación estricta</param>
    /// <returns>Resultado de la validación</returns>
    [HttpPost("course")]
    public ActionResult<object> TestCourseValidation([FromBody] CreateCourseDto course)
    {
        _logger.LogInformation("Course validation test passed for: {CourseCode}", course.CourseCode);
        
        return Ok(new
        {
            message = "Validation successful! All data types are correct.",
            receivedData = new
            {
                courseCode = course.CourseCode,
                courseName = course.CourseName,
                name = course.Name,
                description = course.Description,
                creditHours = course.CreditHours,
                credits = course.Credits,
                department = course.Department,
                prerequisites = course.Prerequisites
            },
            validationPassed = true,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Endpoint que demuestra ejemplos de datos válidos e inválidos
    /// </summary>
    /// <returns>Ejemplos de uso</returns>
    [HttpGet("examples")]
    public ActionResult<object> GetValidationExamples()
    {
        return Ok(new
        {
            title = "Strict Type Validation Examples",
            description = "This endpoint shows examples of valid and invalid data for testing the strict validation system.",
            
            studentExamples = new
            {
                valid = new
                {
                    studentNumber = "STUD123456",
                    email = "student@university.edu",
                    firstName = "María",
                    lastName = "González",
                    career = "Ingeniería de Software",
                    birthDate = "2000-01-15T00:00:00Z",
                    phoneNumber = "+1-555-123-4567",
                    address = "123 University Ave",
                    password = "SecurePassword123"
                },
                invalid = new
                {
                    examples = new object[]
                    {
                        new { field = "firstName", value = "123", reason = "Purely numeric value in text field" },
                        new { field = "email", value = "12345", reason = "Numeric value instead of email format" },
                        new { field = "studentNumber", value = "stud123", reason = "Lowercase letters not allowed" },
                        new { field = "career", value = (object)123, reason = "Number instead of text" },
                        new { field = "birthDate", value = "2000-01-15", reason = "String instead of DateTime object" }
                    }
                }
            },
            
            courseExamples = new
            {
                valid = new
                {
                    courseCode = "COMP1234",
                    courseName = "Programación Avanzada",
                    name = "Advanced Programming",
                    description = "Course covering advanced programming concepts",
                    creditHours = 4,
                    credits = 4,
                    department = "Computer Science",
                    prerequisites = "COMP1001, MATH2001"
                },
                invalid = new
                {
                    examples = new object[]
                    {
                        new { field = "courseCode", value = "comp123", reason = "Lowercase letters not allowed" },
                        new { field = "courseName", value = "123", reason = "Purely numeric value in text field" },
                        new { field = "creditHours", value = "4", reason = "String instead of integer" },
                        new { field = "department", value = (object)456, reason = "Number instead of text" },
                        new { field = "credits", value = "3.5", reason = "String instead of integer" }
                    }
                }
            },
            
            testInstructions = new
            {
                message = "To test the validation system, send POST requests to /api/validationtest/student or /api/validationtest/course",
                steps = new[]
                {
                    "1. Try sending valid data first to ensure the endpoint works",
                    "2. Then try sending invalid data types to see validation errors",
                    "3. Pay attention to the detailed error messages returned",
                    "4. Notice how the API rejects type conversions and mismatched types"
                }
            },
            
            commonValidationRules = new
            {
                textFields = "Must contain only alphabetic characters and spaces, no purely numeric values",
                emailFields = "Must be valid email format, cannot be numeric",
                codeFields = "Must follow specific patterns (course codes: COMP1234, student numbers: STUD123456)",
                numericFields = "Must be genuine numbers, not strings containing numbers",
                booleanFields = "Must be true/false, not strings like 'true'/'false'",
                dateFields = "Must be ISO 8601 DateTime objects, not date strings"
            }
        });
    }

    /// <summary>
    /// Endpoint para probar validación de tipos específicos
    /// </summary>
    /// <param name="testData">Datos de prueba</param>
    /// <returns>Resultado de validación por tipo</returns>
    [HttpPost("types")]
    public ActionResult<object> TestSpecificTypes([FromBody] TypeTestDto testData)
    {
        return Ok(new
        {
            message = "Type validation successful!",
            receivedTypes = new
            {
                stringValue = new { value = testData.StringValue, type = testData.StringValue?.GetType().Name },
                intValue = new { value = testData.IntValue, type = testData.IntValue?.GetType().Name },
                boolValue = new { value = testData.BoolValue, type = testData.BoolValue?.GetType().Name },
                dateValue = new { value = testData.DateValue, type = testData.DateValue?.GetType().Name },
                decimalValue = new { value = testData.DecimalValue, type = testData.DecimalValue?.GetType().Name }
            },
            validationPassed = true,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Endpoint simple para probar tipos básicos de JSON
    /// </summary>
    [HttpPost("basic-types")]
    public ActionResult<object> TestBasicTypes([FromBody] BasicTypesDto data)
    {
        return Ok(new
        {
            message = "All types validated successfully!",
            receivedData = new
            {
                stringValue = data.StringValue,
                intValue = data.IntValue,
                boolValue = data.BoolValue,
                decimalValue = data.DecimalValue,
                dateValue = data.DateValue
            },
            timestamp = DateTime.UtcNow
        });
    }
}

/// <summary>
/// DTO para probar validación de tipos específicos
/// </summary>
public class TypeTestDto
{
    public string? StringValue { get; set; }
    public int? IntValue { get; set; }
    public bool? BoolValue { get; set; }
    public DateTime? DateValue { get; set; }
    public decimal? DecimalValue { get; set; }
}

/// <summary>
/// DTO para probar tipos básicos
/// </summary>
public class BasicTypesDto
{
    [Required]
    [StrictText]
    public string StringValue { get; set; } = string.Empty;
    
    [Required]
    [StrictNumeric(typeof(int), 1, 1000)]
    public int IntValue { get; set; }
    
    [Required]
    public bool BoolValue { get; set; }
    
    [StrictNumeric(typeof(decimal), 0.01, 9999.99)]
    public decimal? DecimalValue { get; set; }
    
    [StrictDateTime]
    public DateTime? DateValue { get; set; }
}
///CAMBIOS MENORES A LOS NOMBRES DE LOS CONTROLLERS