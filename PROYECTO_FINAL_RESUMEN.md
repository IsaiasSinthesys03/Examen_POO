# ExamenPOO - Sistema de Inscripción Universitaria
## Proyecto Final - Clean Architecture con .NET 8.0

### 📋 RESUMEN DEL PROYECTO

Este proyecto implementa un sistema de inscripción universitaria siguiendo la arquitectura **Clean Architecture/Onion Architecture** con .NET 8.0, diseñado específicamente para deploy en **SOMEE**.

### 🏗️ ARQUITECTURA IMPLEMENTADA

#### **ExamenPOO.Core** (Dominio)
- **Entidades**: Student, Course, SemesterEnrollment, EnrolledCourse
- **Interfaces de Repositorio**: IStudentRepository, ICourseRepository, ISemesterEnrollmentRepository
- **Reglas de Negocio**: Control de créditos, validación de duplicidad, restricciones de eliminación

#### **ExamenPOO.Application** (Casos de Uso)
- **Servicios**: StudentService, CourseService, SemesterEnrollmentService, JwtService
- **DTOs**: StudentDto, CourseDto, SemesterEnrollmentDto, EnrolledCourseDto
- **Interfaces de Servicios**: IStudentService, ICourseService, ISemesterEnrollmentService, IJwtService

#### **ExamenPOO.Infrastructure** (Infraestructura)
- **Repositorios**: StudentRepository, CourseRepository, SemesterEnrollmentRepository
- **Contexto de Base de Datos**: ApplicationDbContext (Entity Framework Core)
- **Servicios de Infraestructura**: JwtService implementation

#### **ExamenPOO.API** (Presentación)
- **Controladores**: StudentsController, CoursesController, SemesterEnrollmentsController, AuthController
- **Middleware**: GlobalExceptionMiddleware, JwtAuthorizationMiddleware
- **Configuración**: Program.cs, appsettings.json, web.config

### 📋 REGLAS DE NEGOCIO IMPLEMENTADAS

1. **Control de Créditos**: Los estudiantes no pueden inscribirse en más de 20 créditos por semestre
2. **No Duplicidad**: No se permite duplicar matrícula de estudiantes ni códigos de curso
3. **Validación de Correo**: Cada estudiante debe tener un correo único
4. **Restricciones de Eliminación**: No se pueden eliminar estudiantes con inscripciones activas
5. **Gestión de Inscripciones**: Control completo de inscripciones semestrales y por materia

### 🔧 ENDPOINTS DISPONIBLES

#### **Estudiantes** (`/api/students`)
- `GET /api/students` - Listar todos los estudiantes
- `GET /api/students/{id}` - Obtener estudiante por ID
- `POST /api/students` - Crear nuevo estudiante
- `PUT /api/students/{id}` - Actualizar estudiante
- `DELETE /api/students/{id}` - Eliminar estudiante (solo si no tiene inscripciones)

#### **Cursos** (`/api/courses`)
- `GET /api/courses` - Listar todos los cursos
- `GET /api/courses/{id}` - Obtener curso por ID
- `POST /api/courses` - Crear nuevo curso
- `PUT /api/courses/{id}` - Actualizar curso
- `DELETE /api/courses/{id}` - Eliminar curso

#### **Inscripciones Semestrales** (`/api/semester-enrollments`)
- `GET /api/semester-enrollments` - Listar todas las inscripciones
- `GET /api/semester-enrollments/{id}` - Obtener inscripción por ID
- `POST /api/students/{studentId}/semesters` - Crear inscripción semestral
- `PUT /api/semester-enrollments/{id}` - Actualizar inscripción
- `DELETE /api/semester-enrollments/{id}` - Eliminar inscripción

#### **Inscripción de Materias** (`/api/semesters/{semesterId}/courses`)
- `POST /api/semesters/{semesterId}/courses/{courseId}` - Inscribir materia
- `DELETE /api/semesters/{semesterId}/courses/{courseId}` - Desinscribir materia

#### **Autenticación** (`/api/auth`)
- `POST /api/auth/login` - Iniciar sesión (obtener JWT)

### 📝 EJEMPLOS DE JSON PARA PRUEBAS

#### **1. Crear Estudiante**
```json
POST /api/students
{
    "firstName": "Juan",
    "lastName": "Pérez",
    "email": "juan.perez@universidad.edu",
    "studentCode": "EST001",
    "enrollmentDate": "2024-01-15T00:00:00Z"
}
```

#### **2. Crear Curso**
```json
POST /api/courses
{
    "name": "Programación Orientada a Objetos",
    "courseCode": "POO001",
    "credits": 4,
    "description": "Curso introductorio a la programación orientada a objetos"
}
```

#### **3. Crear Inscripción Semestral**
```json
POST /api/students/1/semesters
{
    "semester": "2024-1",
    "year": 2024,
    "maxCredits": 20,
    "enrollmentDate": "2024-01-15T00:00:00Z"
}
```

#### **4. Inscribir Materia**
```json
POST /api/semesters/1/courses/1
{
    "enrollmentDate": "2024-01-16T00:00:00Z"
}
```

#### **5. Login (Autenticación)**
```json
POST /api/auth/login
{
    "email": "juan.perez@universidad.edu",
    "password": "password123"
}
```

#### **6. Actualizar Estudiante**
```json
PUT /api/students/1
{
    "firstName": "Juan Carlos",
    "lastName": "Pérez González",
    "email": "juan.perez@universidad.edu",
    "studentCode": "EST001",
    "enrollmentDate": "2024-01-15T00:00:00Z"
}
```

#### **7. Actualizar Curso**
```json
PUT /api/courses/1
{
    "name": "Programación Orientada a Objetos Avanzada",
    "courseCode": "POO001",
    "credits": 5,
    "description": "Curso avanzado de programación orientada a objetos con patrones de diseño"
}
```

### 🚀 INSTRUCCIONES DE DEPLOY EN SOMEE

1. **Subir el archivo ZIP**: Sube `ExamenPOO-FINAL-Deploy.zip` a tu cuenta de SOMEE
2. **Extraer archivos**: Extrae todos los archivos en la carpeta raíz de tu sitio web
3. **Configurar Base de Datos**: Actualiza la cadena de conexión en `appsettings.Production.json`
4. **Verificar funcionamiento**: Accede a `tu-sitio.somee.com` que redirigirá automáticamente a Swagger UI

### 📊 CARACTERÍSTICAS TÉCNICAS

- **.NET 8.0**: Compatible con SOMEE
- **Entity Framework Core**: Para acceso a datos
- **JWT Authentication**: Sistema de autenticación seguro
- **Swagger UI**: Documentación interactiva de la API
- **Clean Architecture**: Separación clara de responsabilidades
- **Validation**: Validaciones robustas en todos los niveles
- **Error Handling**: Manejo centralizado de errores

### 🔐 SEGURIDAD

- **JWT Tokens**: Autenticación basada en tokens
- **CORS**: Configurado para permitir acceso desde diferentes orígenes
- **Validation**: Validaciones estrictas en entrada de datos
- **Error Handling**: Manejo seguro de errores sin exposición de información sensible

### 📁 ARCHIVOS IMPORTANTES

- `ExamenPOO-FINAL-Deploy.zip` - Archivo final listo para deploy
- `web.config` - Configuración optimizada para SOMEE
- `default.aspx` - Página de inicio que redirige a Swagger
- `global.asax` - Configuración global de la aplicación

### ✅ ESTADO DEL PROYECTO

- ✅ Arquitectura Clean/Onion implementada correctamente
- ✅ Migración a .NET 8.0 completada
- ✅ Reglas de negocio implementadas
- ✅ Endpoints CRUD funcionando
- ✅ Sistema de inscripciones implementado
- ✅ Validaciones y restricciones aplicadas
- ✅ Deploy package listo para SOMEE
- ✅ Documentación y ejemplos completos

**El proyecto está 100% listo para deploy en SOMEE.**
