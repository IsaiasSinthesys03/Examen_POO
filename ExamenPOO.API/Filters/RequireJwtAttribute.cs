using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace ExamenPOO.API.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireJwtAttribute : Attribute, IAuthorizationFilter
    {
        public string? CustomMessage { get; set; }
        public string? RequiredRole { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Verificar si el usuario está autenticado
            if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
            {
                var response = new
                {
                    Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                    Title = "Authentication Required",
                    Status = 401,
                    Detail = CustomMessage ?? "Este endpoint requiere autenticación JWT válida",
                    Instance = context.HttpContext.Request.Path.Value,
                    ErrorCode = "AUTHENTICATION_REQUIRED",
                    Timestamp = DateTime.UtcNow,
                    AuthenticationRequirements = new
                    {
                        Message = "Debe proporcionar un token JWT válido para acceder a este recurso",
                        LoginEndpoint = "/api/auth/login",
                        TokenFormat = "Authorization: Bearer <your-jwt-token>",
                        RequiredRole = RequiredRole ?? "Any authenticated user"
                    },
                    SecurityInfo = new
                    {
                        EndpointProtected = true,
                        RequiresValidToken = true,
                        TokenExpiry = "Los tokens tienen duración limitada",
                        RenewalRequired = "Obtenga un nuevo token si el actual ha expirado"
                    }
                };

                context.Result = new JsonResult(response)
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized
                };
                return;
            }

            // Verificar rol específico si se especifica
            if (!string.IsNullOrEmpty(RequiredRole))
            {
                if (!context.HttpContext.User.IsInRole(RequiredRole))
                {
                    var response = new
                    {
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                        Title = "Insufficient Permissions",
                        Status = 403,
                        Detail = $"Este endpoint requiere el rol '{RequiredRole}'",
                        Instance = context.HttpContext.Request.Path.Value,
                        ErrorCode = "INSUFFICIENT_ROLE",
                        Timestamp = DateTime.UtcNow,
                        AuthorizationInfo = new
                        {
                            CurrentUser = context.HttpContext.User.Identity?.Name ?? "Unknown",
                            RequiredRole = RequiredRole,
                            UserRoles = context.HttpContext.User.Claims
                                .Where(c => c.Type == "role")
                                .Select(c => c.Value)
                                .ToArray(),
                            Message = "Su token es válido pero no tiene el rol necesario para este recurso"
                        }
                    };

                    context.Result = new JsonResult(response)
                    {
                        StatusCode = (int)HttpStatusCode.Forbidden
                    };
                    return;
                }
            }
        }
    }
}
