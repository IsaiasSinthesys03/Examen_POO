using System.ComponentModel.DataAnnotations;

namespace ExamenPOO.Application.DTOs;

public class LoginDto
{
    public string? Email { get; set; }
    
    public string? StudentNumber { get; set; }

    [Required(ErrorMessage = "La contraseña es requerida")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string Password { get; set; } = string.Empty;
}
