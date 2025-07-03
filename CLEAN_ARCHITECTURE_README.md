# ExamenPOO - Clean Architecture (Onion Architecture)

## Resumen de la Arquitectura Implementada

Este proyecto implementa una **Arquitectura Onion (Clean Architecture)** siguiendo los principios de Robert C. Martin. La arquitectura está dividida en capas concéntricas donde las dependencias apuntan hacia el centro.

## Estructura de Capas

### 🎯 Core (Dominio) - `ExamenPOO.Core`
**El centro de la arquitectura - No tiene dependencias externas**

#### Entidades (`Entities/`)
- `BaseEntity.cs` - Clase base para todas las entidades del dominio
- `Student.cs` - Entidad principal con lógica de negocio encapsulada
- `Course.cs`, `SemesterEnrollment.cs`, `EnrolledCourse.cs` - Otras entidades del dominio

#### Value Objects (`ValueObjects/`)
- `Email.cs` - Encapsula la lógica de validación de emails
- `StudentNumber.cs` - Encapsula la lógica de validación de números de estudiante

#### Servicios de Dominio (`Services/`)
- `IPasswordHashingService.cs` - Interfaz para el servicio de hashing de contraseñas

#### Repositorios (`Interfaces/`)
- Interfaces de repositorios que definen contratos sin implementación
- `IRepository<T>`, `IStudentRepository`, etc.

### 📋 Application (Casos de Uso) - `ExamenPOO.Application`
**Orquesta la lógica de negocio y coordina entre capas**

#### DTOs (`DTOs/`)
- Objetos de transferencia de datos para comunicación con el exterior
- `StudentDto`, `CreateStudentDto`, `UpdateStudentDto`, etc.

#### Servicios de Aplicación (`Services/`)
- `StudentService.cs` - Implementa casos de uso relacionados con estudiantes
- `CourseService.cs` - Implementa casos de uso relacionados con cursos

#### Interfaces (`Interfaces/`)
- Contratos para servicios de aplicación
- `IStudentService`, `ICourseService`, etc.

### 🔧 Infrastructure (Infraestructura) - `ExamenPOO.Infrastructure`
**Implementa detalles técnicos y acceso a datos**

#### Datos (`Data/`)
- `ApplicationDbContext.cs` - Contexto de Entity Framework con configuraciones
- Conversiones de Value Objects para persistencia

#### Repositorios (`Repositories/`)
- Implementaciones concretas de los repositorios
- `StudentRepository.cs`, `CourseRepository.cs`, etc.

#### Servicios (`Services/`)
- `PasswordHashingService.cs` - Implementación concreta usando BCrypt
- `DatabaseSeederService.cs` - Servicio para sembrar datos iniciales
- `JwtService.cs` - Servicio para manejo de tokens JWT

### 🌐 API (Interfaz de Usuario) - `ExamenPOO.API`
**Capa de presentación y punto de entrada**

#### Controladores (`Controllers/`)
- `StudentsController.cs` - Endpoints REST para gestión de estudiantes
- `AuthController.cs` - Endpoints para autenticación
- Otros controladores...

## Principios de Clean Architecture Implementados

### 1. **Regla de Dependencia**
- ✅ Las capas externas dependen de las internas, nunca al revés
- ✅ El Core no tiene referencias a capas externas
- ✅ Application solo depende de Core
- ✅ Infrastructure implementa interfaces de Core y Application

### 2. **Separación de Responsabilidades**
- ✅ **Core**: Lógica de negocio y reglas de dominio
- ✅ **Application**: Casos de uso y orchestración
- ✅ **Infrastructure**: Detalles técnicos y persistencia
- ✅ **API**: Presentación y comunicación externa

### 3. **Inversión de Dependencias (DIP)**
- ✅ Las interfaces están definidas en Core
- ✅ Las implementaciones están en Infrastructure
- ✅ La inyección de dependencias resuelve las implementaciones

### 4. **Principio de Responsabilidad Única (SRP)**
- ✅ Cada clase tiene una única razón para cambiar
- ✅ Value Objects encapsulan validaciones específicas
- ✅ Servicios de dominio manejan lógica específica

## Mejoras Implementadas en la Arquitectura Onion

### 🔒 Encapsulación del Dominio
```csharp
public class Student : BaseEntity
{
    // Propiedades con setters internos - solo el dominio puede modificarlas
    public StudentNumber StudentNumber { get; internal set; } = null!;
    public Email Email { get; internal set; } = null!;
    
    // Constructor público con validaciones de negocio
    public Student(StudentNumber studentNumber, Email email, ...)
    {
        // Validaciones de negocio aquí
    }
    
    // Métodos de negocio que mantienen invariantes
    public void UpdateEmail(Email newEmail) { ... }
    public void Activate() { ... }
    public void Deactivate() { ... }
}
```

### 🎯 Value Objects
```csharp
public class Email : IEquatable<Email>
{
    public string Value { get; }
    
    public Email(string value)
    {
        // Validaciones específicas del dominio
        if (!EmailRegex.IsMatch(value))
            throw new ArgumentException("Invalid email format");
        
        Value = value.ToLowerInvariant();
    }
}
```

### 🔨 Servicios de Dominio
```csharp
// Interfaz en Core
public interface IPasswordHashingService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}

// Implementación en Infrastructure
public class PasswordHashingService : IPasswordHashingService
{
    public string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    // ...
}
```

### 📊 Configuración de EF Core para Value Objects
```csharp
entity.Property(e => e.Email)
    .HasConversion(
        v => v.Value,                    // A la base de datos
        v => new Email(v))               // Desde la base de datos
    .IsRequired()
    .HasMaxLength(100);
```

## Ventajas de esta Implementación

### ✅ Mantenibilidad
- Código organizado por responsabilidades claras
- Fácil de entender y modificar
- Cambios en una capa no afectan otras

### ✅ Testabilidad
- Lógica de negocio aislada y testeable
- Interfaces permiten mocking fácil
- Casos de uso claramente definidos

### ✅ Flexibilidad
- Fácil cambio de tecnologías (base de datos, framework web)
- Adición de nuevas funcionalidades sin romper existentes
- Extensibilidad a través de interfaces

### ✅ Seguridad del Dominio
- Value Objects previenen datos inválidos
- Encapsulación protege invariantes del negocio
- Validaciones centralizadas en el dominio

## Comandos de Desarrollo

```bash
# Restaurar paquetes
dotnet restore

# Compilar solución
dotnet build

# Ejecutar migraciones
dotnet ef database update --project ExamenPOO.Infrastructure --startup-project ExamenPOO.API

# Ejecutar aplicación
dotnet run --project ExamenPOO.API
```

## Datos de Prueba

Al ejecutar la aplicación, se crean automáticamente 3 estudiantes de prueba:

- **Email**: john.doe@university.edu, **Contraseña**: Demo123!
- **Email**: jane.smith@university.edu, **Contraseña**: Demo123!  
- **Email**: mike.johnson@university.edu, **Contraseña**: Demo123!

## Endpoints Principales

- `POST /api/auth/login` - Autenticación
- `GET /api/students` - Listar estudiantes (paginado)
- `POST /api/students` - Crear estudiante
- `PUT /api/students/{id}` - Actualizar estudiante
- `DELETE /api/students/{id}` - Desactivar estudiante

---

**Esta implementación demuestra una arquitectura robusta, mantenible y escalable siguiendo los principios de Clean Architecture.**
