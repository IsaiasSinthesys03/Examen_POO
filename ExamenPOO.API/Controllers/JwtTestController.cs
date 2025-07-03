using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExamenPOO.API.Filters;

namespace ExamenPOO.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JwtTestController : ControllerBase
    {
        /// <summary>
        /// Endpoint público para pruebas (no requiere autenticación)
        /// </summary>
        /// <returns>Mensaje de prueba público</returns>
        [HttpGet("public")]
        public ActionResult<object> GetPublicEndpoint()
        {
            return Ok(new
            {
                Message = "Este es un endpoint público, no requiere autenticación",
                Timestamp = DateTime.UtcNow,
                Status = "Success",
                AuthenticationRequired = false
            });
        }

        /// <summary>
        /// Endpoint protegido con autorización estándar
        /// </summary>
        /// <returns>Información del usuario autenticado</returns>
        [HttpGet("protected")]
        [Authorize]
        public ActionResult<object> GetProtectedEndpoint()
        {
            return Ok(new
            {
                Message = "¡Acceso autorizado! Este endpoint requiere JWT válido",
                Timestamp = DateTime.UtcNow,
                Status = "Success",
                UserInfo = new
                {
                    IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
                    UserName = User.Identity?.Name ?? "Unknown",
                    Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
                }
            });
        }

        /// <summary>
        /// Endpoint con mensaje personalizado de JWT
        /// </summary>
        /// <returns>Datos sensibles del usuario</returns>
        [HttpGet("sensitive")]
        [RequireJwt(CustomMessage = "Este endpoint contiene información sensible y requiere autenticación estricta")]
        public ActionResult<object> GetSensitiveEndpoint()
        {
            return Ok(new
            {
                Message = "Datos sensibles del usuario",
                Timestamp = DateTime.UtcNow,
                Status = "Success",
                SensitiveData = new
                {
                    UserProfile = "Información confidencial del usuario",
                    AccessLevel = "High Security",
                    LastAccess = DateTime.UtcNow.AddDays(-1)
                }
            });
        }

        /// <summary>
        /// Endpoint que requiere rol específico
        /// </summary>
        /// <returns>Información administrativa</returns>
        [HttpGet("admin")]
        [RequireJwt(RequiredRole = "Admin", CustomMessage = "Solo administradores pueden acceder a este recurso")]
        public ActionResult<object> GetAdminEndpoint()
        {
            return Ok(new
            {
                Message = "¡Acceso de administrador concedido!",
                Timestamp = DateTime.UtcNow,
                Status = "Success",
                AdminData = new
                {
                    TotalUsers = 150,
                    ActiveSessions = 42,
                    SystemHealth = "Optimal",
                    LastMaintenance = DateTime.UtcNow.AddDays(-3)
                }
            });
        }

        /// <summary>
        /// Endpoint para obtener información del token actual
        /// </summary>
        /// <returns>Información detallada del token JWT</returns>
        [HttpGet("token-info")]
        [Authorize]
        public ActionResult<object> GetTokenInfo()
        {
            return Ok(new
            {
                Message = "Información del token JWT actual",
                Timestamp = DateTime.UtcNow,
                Status = "Success",
                TokenInfo = new
                {
                    IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
                    AuthenticationType = User.Identity?.AuthenticationType ?? "Unknown",
                    UserName = User.Identity?.Name ?? "Unknown",
                    Claims = User.Claims.Select(c => new 
                    { 
                        Type = c.Type, 
                        Value = c.Value,
                        Description = GetClaimDescription(c.Type)
                    }).ToList(),
                    Roles = User.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList()
                }
            });
        }

        /// <summary>
        /// Obtiene una descripción friendly del tipo de claim
        /// </summary>
        /// <param name="claimType">Tipo de claim</param>
        /// <returns>Descripción del claim</returns>
        private string GetClaimDescription(string claimType)
        {
            return claimType switch
            {
                "sub" => "Subject (User ID)",
                "name" => "User Name",
                "email" => "Email Address",
                "role" => "User Role",
                "exp" => "Expiration Time",
                "iat" => "Issued At",
                "iss" => "Issuer",
                "aud" => "Audience",
                _ => claimType
            };
        }
    }
}
///CAMBIOS MENORES A LOS NOMBRES DE LOS CONTROLLERS