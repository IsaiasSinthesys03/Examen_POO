# ExamenPOO WebAPI - Clean Architecture

Una WebAPI desarrollada en ASP.NET Core (.NET 9) siguiendo la arquitectura ONIO (Clean Architecture) con todas las características empresariales implementadas.

## 🏗️ Arquitectura del Proyecto

El proyecto sigue los principios de Clean Architecture con las siguientes capas:

```
ExamenPOO/
├── ExamenPOO.Core/           # Capa de Dominio
│   ├── Entities/             # Entidades de negocio
│   └── Interfaces/           # Interfaces de repositorios
├── ExamenPOO.Application/    # Capa de Aplicación
│   ├── DTOs/                 # Data Transfer Objects
│   ├── Services/             # Servicios de aplicación
│   └── Validators/           # Validadores
├── ExamenPOO.Infrastructure/ # Capa de Infraestructura
│   ├── Data/                 # DbContext y configuraciones
│   ├── Repositories/         # Implementaciones de repositorios
│   └── Migrations/           # Migraciones de EF Core
└── ExamenPOO.API/            # Capa de Presentación
    ├── Controllers/          # Controladores REST
    ├── Middleware/           # Middleware personalizado
    └── Program.cs            # Configuración de la aplicación
```

## 🚀 Características Implementadas

### ✅ Operaciones CRUD Completas
- **Usuarios**: Registro, consulta, actualización y eliminación
- **Productos**: Gestión completa con relaciones
- **DTOs**: Separación entre entidades y datos de transferencia
- **Validaciones**: Validación de datos en todas las capas

### ✅ Paginación Avanzada
- Implementada en todos los endpoints GET de colecciones
- Parámetros: `pageNumber`, `pageSize`
- Metadatos de paginación en respuestas:
  ```json
  {
    "data": [...],
    "pageNumber": 1,
    "pageSize": 10,
    "totalRecords": 100,
    "totalPages": 10
  }
  ```

### ✅ Autenticación JWT
- **Registro y Login**: Endpoints seguros para autenticación
- **Tokens JWT**: Con tiempo de expiración configurable
- **Protección de rutas**: Atributos `[Authorize]` en endpoints protegidos
- **Configuración completa**: JWT configurado en Program.cs

### ✅ Entity Framework Core
- **SQL Server**: Configurado para base de datos SOMEE
- **Migraciones**: Automáticas para creación de tablas
- **Repository Pattern**: Abstracciones de datos implementadas
- **Unit of Work**: Patrón para transacciones
- **Datos de prueba**: Seed data incluido

### ✅ Documentación API (Swagger)
- **OpenAPI**: Documentación completa de endpoints
- **Autorización JWT**: Configurado en Swagger UI
- **Producción**: Disponible en ambiente de producción
- **Descripción detallada**: Cada endpoint documentado

### ✅ Configuración para SOMEE
- **Cadena de conexión**: Configurada para SOMEE.com
- **web.config**: Incluido para despliegue
- **CORS**: Configurado para múltiples orígenes
- **Producción**: Listo para publicar

## 🔧 Configuración y Uso

### Requisitos Previos
- .NET 9 SDK
- SQL Server (configurado en SOMEE.com)
- Visual Studio 2022 o VS Code

### Instalación Local

1. **Clonar el repositorio**
   ```bash
   git clone [tu-repositorio]
   cd ExamenPOO
   ```

2. **Restaurar paquetes**
   ```bash
   dotnet restore
   ```

3. **Configurar base de datos**
   - La cadena de conexión ya está configurada para SOMEE
   - Las migraciones ya están aplicadas

4. **Ejecutar la aplicación**
   ```bash
   cd ExamenPOO.API
   dotnet run
   ```

5. **Acceder a Swagger**
   - Abrir: `http://localhost:5078/swagger`

## 📝 Endpoints Principales

### Autenticación
- `POST /api/auth/register` - Registrar nuevo usuario
- `POST /api/auth/login` - Iniciar sesión

### Usuarios
- `GET /api/users` - Obtener usuarios (paginado)
- `GET /api/users/{id}` - Obtener usuario por ID
- `PUT /api/users/{id}` - Actualizar usuario
- `DELETE /api/users/{id}` - Eliminar usuario

### Productos
- `GET /api/products` - Obtener productos (paginado)
- `GET /api/products/{id}` - Obtener producto por ID
- `POST /api/products` - Crear producto
- `PUT /api/products/{id}` - Actualizar producto
- `DELETE /api/products/{id}` - Eliminar producto

## 🔐 Autenticación

### Registro de Usuario
```json
POST /api/auth/register
{
  "username": "usuario123",
  "email": "usuario@email.com",
  "password": "Password123!",
  "firstName": "Juan",
  "lastName": "Pérez"
}
```

### Inicio de Sesión
```json
POST /api/auth/login
{
  "username": "usuario123",
  "password": "Password123!"
}
```

**Respuesta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiration": "2024-01-02T00:00:00Z",
  "user": {
    "id": 1,
    "username": "usuario123",
    "email": "usuario@email.com"
  }
}
```

## 📊 Paginación

Todos los endpoints de colecciones soportan paginación:

```
GET /api/products?pageNumber=1&pageSize=10
```

**Respuesta:**
```json
{
  "data": [...],
  "pageNumber": 1,
  "pageSize": 10,
  "totalRecords": 156,
  "totalPages": 16,
  "hasPrevious": false,
  "hasNext": true
}
```

## 🛠️ Configuración para SOMEE

### Cadena de Conexión
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "workstation id=examendbo.mssql.somee.com;packet size=4096;user id=BoniceFrio_SQLLogin_1;pwd=65ahxeadnt;data source=examendbo.mssql.somee.com;persist security info=False;initial catalog=examendbo;TrustServerCertificate=True"
  }
}
```

### Publicación
1. Compilar en modo Release
2. Publicar archivos
3. Subir a SOMEE.com
4. La aplicación estará disponible con Swagger habilitado

## 🧪 Datos de Prueba

El sistema incluye datos de prueba:

### Usuarios
- **Admin**: `admin` / `admin123`
- **Demo**: `demo` / `demo123`

### Productos
- Laptop Gaming ($1,299.99)
- Wireless Mouse ($29.99)
- Coffee Mug ($12.99)

## 🔧 Tecnologías Utilizadas

- **ASP.NET Core 9**: Framework web
- **Entity Framework Core**: ORM
- **SQL Server**: Base de datos
- **JWT Bearer**: Autenticación
- **Swagger/OpenAPI**: Documentación
- **BCrypt**: Hashing de contraseñas
- **CORS**: Soporte para múltiples orígenes

## 📋 Estructura de Base de Datos

### Tabla Users
- Id (PK)
- Username (Unique)
- Email (Unique)
- PasswordHash
- FirstName
- LastName
- CreatedAt
- UpdatedAt
- IsActive

### Tabla Products
- Id (PK)
- Name
- Description
- Price
- Stock
- Category
- CreatedAt
- UpdatedAt
- IsActive
- UserId (FK)

## 🎯 Características Adicionales

- **Logging**: Configurado para desarrollo y producción
- **Error Handling**: Middleware para manejo de errores
- **Validation**: Validaciones en DTOs y entidades
- **Security**: Headers de seguridad configurados
- **Performance**: Queries optimizadas y paginación eficiente

## 🚀 Despliegue en Producción

La aplicación está completamente configurada para despliegue en SOMEE.com:

1. **Base de datos**: Configurada y migrada
2. **Swagger**: Habilitado en producción
3. **CORS**: Configurado para múltiples dominios
4. **Seguridad**: JWT y validaciones implementadas
5. **Rendimiento**: Paginación y queries optimizadas

## 📞 Soporte

Para cualquier consulta o problema:
- Revisar la documentación de Swagger
- Verificar logs de aplicación
- Consultar códigos de error HTTP estándar

---

**Desarrollado con ❤️ usando Clean Architecture y las mejores prácticas de .NET**