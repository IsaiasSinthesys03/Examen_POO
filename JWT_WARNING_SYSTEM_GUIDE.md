# Sistema de Advertencias JWT - Guía de Implementación

## Resumen
Se ha implementado un sistema completo de advertencias y manejo de errores para JWT en la API ExamenPOO. Este sistema proporciona respuestas detalladas y user-friendly cuando los usuarios no están autorizados.

## Componentes Implementados

### 1. JwtAuthorizationMiddleware
**Ubicación**: `ExamenPOO.API/Middleware/JwtAuthorizationMiddleware.cs`

**Funcionalidad**:
- Intercepta todas las peticiones a endpoints protegidos
- Verifica la presencia del token JWT
- Valida el formato del token
- Proporciona respuestas detalladas para diferentes escenarios de error

**Escenarios de Error Manejados**:
- `NO_TOKEN`: No se proporciona token JWT
- `INVALID_TOKEN_FORMAT`: Token sin formato "Bearer <token>"
- `EMPTY_TOKEN`: Token vacío después de "Bearer "

### 2. Eventos JWT Personalizados
**Ubicación**: `Program.cs` - Configuración JWT

**Eventos Manejados**:
- `OnChallenge`: Token inválido o expirado
- `OnForbidden`: Permisos insuficientes

**Tipos de Error**:
- `INVALID_TOKEN`: Token JWT inválido
- `EXPIRED_TOKEN`: Token JWT expirado
- `INVALID_SIGNATURE`: Firma del token inválida
- `INSUFFICIENT_PERMISSIONS`: Permisos insuficientes

### 3. RequireJwtAttribute
**Ubicación**: `ExamenPOO.API/Filters/RequireJwtAttribute.cs`

**Características**:
- Atributo personalizable para endpoints específicos
- Soporte para roles específicos
- Mensajes de error personalizados

## Ejemplos de Uso

### Uso Básico con [Authorize]
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // Usa el sistema estándar con respuestas mejoradas
public class StudentsController : ControllerBase
{
    // Métodos del controlador
}
```

### Uso con Atributo Personalizado
```csharp
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    [HttpGet]
    [RequireJwt(CustomMessage = "Acceso restringido solo para administradores", RequiredRole = "Admin")]
    public async Task<ActionResult> GetAdminData()
    {
        // Lógica del método
    }
}
```

## Endpoints Públicos (No Requieren Autenticación)

El middleware automáticamente permite acceso a:
- `/swagger` - Documentación de la API
- `/api/auth/login` - Endpoint de login
- `/api/auth/register` - Endpoint de registro
- `/` - Página principal (redirige a Swagger)
- `/_framework` - Archivos del framework
- `/_vs` - Archivos de Visual Studio
- `/favicon.ico` - Icono del sitio

## Ejemplos de Respuestas de Error

### Token No Proporcionado
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized Access",
  "status": 401,
  "detail": "Token JWT requerido",
  "instance": "/api/students",
  "errorCode": "NO_TOKEN",
  "timestamp": "2025-07-03T10:30:00Z",
  "securityInfo": {
    "message": "Esta API requiere autenticación JWT válida para acceder a recursos protegidos.",
    "instructions": [
      "1. Obtenga un token JWT válido usando el endpoint /api/auth/login",
      "2. Incluya el token en el header Authorization: Bearer <su-token>",
      "3. Asegúrese de que el token no haya expirado",
      "4. Verifique que el token tenga el formato correcto"
    ],
    "loginEndpoint": "/api/auth/login",
    "tokenExpiry": "Los tokens JWT tienen una duración limitada y deben renovarse periódicamente"
  }
}
```

### Token Expirado
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Authentication Failed",
  "status": 401,
  "detail": "El token JWT ha expirado",
  "instance": "/api/students",
  "errorCode": "EXPIRED_TOKEN",
  "timestamp": "2025-07-03T10:30:00Z",
  "authenticationInfo": {
    "message": "La autenticación JWT falló. Verifique su token y vuelva a intentarlo.",
    "possibleCauses": [
      "Token expirado - obtenga un nuevo token",
      "Token malformado o corrupto",
      "Firma del token inválida",
      "Token no proporcionado en el header Authorization"
    ],
    "solution": {
      "loginEndpoint": "/api/auth/login",
      "headerFormat": "Authorization: Bearer <your-jwt-token>",
      "tokenLifetime": "Verifique la fecha de expiración del token"
    }
  }
}
```

### Permisos Insuficientes
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Access Forbidden",
  "status": 403,
  "detail": "No tiene permisos suficientes para acceder a este recurso",
  "instance": "/api/admin/users",
  "errorCode": "INSUFFICIENT_PERMISSIONS",
  "timestamp": "2025-07-03T10:30:00Z",
  "authorizationInfo": {
    "message": "Su token JWT es válido pero no tiene los permisos necesarios para este recurso.",
    "requiredAction": "Contacte al administrador para obtener los permisos necesarios",
    "userInfo": {
      "isAuthenticated": true,
      "userName": "usuario@ejemplo.com"
    }
  }
}
```

## Configuración y Personalización

### Modificar Endpoints Públicos
Para añadir nuevos endpoints públicos, edite el array `publicEndpoints` en `JwtAuthorizationMiddleware.cs`:

```csharp
var publicEndpoints = new[]
{
    "/swagger",
    "/api/auth/login",
    "/api/auth/register",
    "/api/health", // Nuevo endpoint público
    "/api/version"  // Nuevo endpoint público
};
```

### Personalizar Mensajes de Error
Puede personalizar los mensajes de error modificando las respuestas en cada middleware o atributo según sus necesidades específicas.

## Logging y Monitoreo

El sistema incluye logging detallado que registra:
- Intentos de acceso no autorizados
- IP addresses de las peticiones
- Endpoints accedidos
- Tipos de errores de autenticación

## Consideraciones de Seguridad

1. **Información Sensible**: Las respuestas no exponen información sensible del token
2. **Rate Limiting**: Considere implementar rate limiting para endpoints de autenticación
3. **HTTPS**: Asegúrese de usar HTTPS en producción
4. **Token Expiry**: Configure tiempos de expiración apropiados para los tokens

## Integración con Swagger

El sistema está completamente integrado con Swagger UI, permitiendo:
- Pruebas de endpoints protegidos
- Documentación automática de requisitos de autenticación
- Interfaz visual para incluir tokens JWT

## Mantenimiento

Para mantener el sistema:
1. Revise regularmente los logs de autenticación
2. Actualice mensajes de error según feedback de usuarios
3. Considere añadir métricas de autenticación
4. Implemente alertas para intentos de acceso sospechosos
