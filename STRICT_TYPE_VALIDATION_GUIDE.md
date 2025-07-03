# Sistema de Validación Estricta de Tipos - ExamenPOO API

## Introducción

Esta API implementa un sistema de validación estricta de tipos que garantiza que cada endpoint acepta únicamente el tipo de dato exacto esperado, rechazando cualquier valor que no coincida perfectamente con el tipo definido, incluso si aparentemente podría ser convertible.

## Principios Fundamentales

### 1. Validación Estricta de Tipos
- **No se permiten conversiones implícitas**: Un string que contiene "123" no será aceptado para un campo numérico
- **Tipos exactos**: Los tipos deben coincidir exactamente con lo especificado en la documentación
- **Validación contextual**: Los campos son validados según su contexto y propósito en el dominio

### 2. Reglas por Tipo de Campo

#### Campos de Texto (Nombres, Carreras, Departamentos)
- **Permitido**: Solo caracteres alfabéticos y espacios
- **Rechazado**: 
  - Valores puramente numéricos (ej: "123", "456.78")
  - Números enviados como strings para campos de texto
  - Caracteres especiales no permitidos

**Ejemplo válido:**
```json
{
    "firstName": "Juan Carlos",
    "lastName": "García",
    "career": "Ingeniería de Software"
}
```

**Ejemplo inválido:**
```json
{
    "firstName": "123",           // ❌ Puramente numérico
    "lastName": "García123",      // ❌ Contiene números
    "career": 12345              // ❌ Número en lugar de texto
}
```

#### Campos Numéricos (Créditos, Horas)
- **Permitido**: Valores numéricos genuinos
- **Rechazado**:
  - Strings que contienen números (ej: "123")
  - Conversiones de string a número

**Ejemplo válido:**
```json
{
    "creditHours": 3,
    "credits": 4
}
```

**Ejemplo inválido:**
```json
{
    "creditHours": "3",      // ❌ String en lugar de número
    "credits": "4.5"         // ❌ String en lugar de número
}
```

#### Campos de Email
- **Permitido**: Formato de email válido
- **Rechazado**:
  - Valores puramente numéricos
  - Formatos de email incorrectos

**Ejemplo válido:**
```json
{
    "email": "usuario@ejemplo.com"
}
```

**Ejemplo inválido:**
```json
{
    "email": "123456789",        // ❌ Puramente numérico
    "email": "usuario@",         // ❌ Formato incorrecto
    "email": 123                 // ❌ Número en lugar de string
}
```

#### Campos de Código (Códigos de Curso, Números de Estudiante)
- **Permitido**: Formatos específicos según el tipo
- **Rechazado**: Formatos que no coinciden con los patrones esperados

**Códigos de Curso:**
```json
{
    "courseCode": "COMP1234"     // ✅ 2-4 letras mayúsculas + 3-4 dígitos
}
```

**Números de Estudiante:**
```json
{
    "studentNumber": "STUD123456"  // ✅ 6-20 letras mayúsculas y números
}
```

#### Campos de Fecha
- **Permitido**: Objetos DateTime válidos
- **Rechazado**: Strings que representan fechas

**Ejemplo válido:**
```json
{
    "birthDate": "2000-01-15T00:00:00Z"  // ✅ ISO 8601 format
}
```

**Ejemplo inválido:**
```json
{
    "birthDate": "15/01/2000",    // ❌ String con formato de fecha
    "birthDate": "2000-01-15"     // ❌ String sin formato ISO completo
}
```

#### Campos Booleanos
- **Permitido**: Valores true/false
- **Rechazado**: Strings que representan booleanos

**Ejemplo válido:**
```json
{
    "isActive": true
}
```

**Ejemplo inválido:**
```json
{
    "isActive": "true",      // ❌ String en lugar de boolean
    "isActive": "false",     // ❌ String en lugar de boolean
    "isActive": 1            // ❌ Número en lugar de boolean
}
```

## Respuestas de Error

### Estructura de Respuesta de Error
```json
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "Strict Type Validation Failed",
    "status": 400,
    "detail": "One or more validation errors occurred.",
    "instance": "/api/students",
    "errors": {
        "firstName": [
            "The field 'firstName' cannot contain purely numeric values. Person names must contain alphabetic characters only."
        ],
        "creditHours": [
            "The field 'creditHours' must be an integer value, not a string conversion."
        ]
    },
    "validationRules": {
        "strictTypeMatching": "All parameters must match their expected types exactly. No implicit conversions are allowed.",
        "textFields": "Text fields must contain only alphabetic characters and spaces (no purely numeric values).",
        "numericFields": "Numeric fields must be genuine numeric types, not string representations of numbers.",
        "dateFields": "Date fields must be DateTime objects, not string representations.",
        "emailFields": "Email fields must follow valid email format and cannot be numeric values."
    }
}
```

### Tipos de Errores Comunes

#### 1. Conversión Implícita No Permitida
```json
{
    "error": "String-to-number conversions are not allowed in strict mode",
    "field": "creditHours",
    "expectedType": "Int32",
    "actualType": "String",
    "receivedValue": "3"
}
```

#### 2. Valor Puramente Numérico en Campo de Texto
```json
{
    "error": "Field 'firstName' cannot contain purely numeric values",
    "expectedFormat": "Alphabetic characters and spaces only",
    "receivedValue": "123"
}
```

#### 3. Formato de Código Incorrecto
```json
{
    "error": "Field 'courseCode' must follow course code format",
    "expectedFormat": "2-4 uppercase letters followed by 3-4 digits (e.g., COMP1234)",
    "receivedValue": "comp123"
}
```

## Endpoints Afectados

### POST /api/students
**Campos con validación estricta:**
- `firstName`: Solo texto alfabético
- `lastName`: Solo texto alfabético  
- `email`: Formato de email válido, no numérico
- `studentNumber`: Formato específico de código
- `career`: Solo texto alfabético
- `birthDate`: Objeto DateTime válido

### POST /api/courses
**Campos con validación estricta:**
- `courseCode`: Formato específico (ej: COMP1234)
- `courseName`: Solo texto alfabético
- `name`: Solo texto alfabético
- `creditHours`: Número entero genuino
- `credits`: Número entero genuino
- `department`: Solo texto alfabético

### PUT /api/students/{id}
**Campos con validación estricta:**
- Los mismos campos que POST, pero opcionales

### PUT /api/courses/{id}
**Campos con validación estricta:**
- Los mismos campos que POST, pero opcionales

## Mejores Prácticas

### 1. Envío de Datos Correctos
```json
// ✅ CORRECTO
{
    "firstName": "María",
    "lastName": "González",
    "email": "maria.gonzalez@university.edu",
    "studentNumber": "STUD123456",
    "creditHours": 4,
    "isActive": true,
    "birthDate": "1995-03-15T00:00:00Z"
}

// ❌ INCORRECTO
{
    "firstName": "123",              // Numérico en campo de texto
    "lastName": "González",
    "email": "maria.gonzalez@university.edu",
    "studentNumber": "STUD123456",
    "creditHours": "4",              // String en campo numérico
    "isActive": "true",              // String en campo booleano
    "birthDate": "1995-03-15"        // String en campo de fecha
}
```

### 2. Validación del Cliente
Antes de enviar datos a la API, valide:
- Tipos de datos correctos
- Formatos específicos de códigos
- Rangos válidos para números
- Formatos de fecha ISO 8601

### 3. Manejo de Errores
```javascript
// Ejemplo de manejo de errores en JavaScript
try {
    const response = await fetch('/api/students', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            firstName: "María",        // ✅ Texto
            creditHours: 4,           // ✅ Número
            isActive: true            // ✅ Booleano
        })
    });
    
    if (!response.ok) {
        const errorData = await response.json();
        console.error('Validation errors:', errorData.errors);
        // Manejar errores específicos por campo
    }
} catch (error) {
    console.error('Request failed:', error);
}
```

## Configuración de Desarrollo

### Swagger/OpenAPI
La documentación de Swagger refleja los tipos exactos esperados. Preste atención a:
- Tipos de datos especificados (string, integer, boolean, etc.)
- Formatos específicos (email, date-time, etc.)
- Patrones de validación para códigos

### Herramientas de Testing
Use herramientas como Postman o curl para probar con tipos incorrectos:

```bash
# Ejemplo que causará error de validación
curl -X POST "https://localhost:7000/api/students" \
     -H "Content-Type: application/json" \
     -d '{
       "firstName": "123",
       "creditHours": "4"
     }'
```

## Ventajas del Sistema

1. **Predictibilidad**: Los clientes saben exactamente qué esperar
2. **Consistencia**: Todas las validaciones siguen las mismas reglas
3. **Depuración**: Errores claros y específicos
4. **Robustez**: Previene errores de datos corruptos
5. **Documentación**: Fuerza documentación precisa de tipos

## Consideraciones de Migración

Si está actualizando desde una versión anterior:
1. Revise todos los payloads JSON existentes
2. Asegúrese de que los tipos sean exactos
3. Actualice la documentación del cliente
4. Pruebe exhaustivamente con datos reales
5. Considere implementar gradualmente por endpoint

## Soporte y Resolución de Problemas

### Errores Comunes y Soluciones

1. **"String-to-number conversions are not allowed"**
   - **Solución**: Envíe números sin comillas
   - **Incorrecto**: `"creditHours": "4"`
   - **Correcto**: `"creditHours": 4`

2. **"Field cannot contain purely numeric values"**
   - **Solución**: Asegúrese de que los campos de texto contengan caracteres alfabéticos
   - **Incorrecto**: `"firstName": "123"`
   - **Correcto**: `"firstName": "María"`

3. **"Invalid email format"**
   - **Solución**: Use formato de email válido
   - **Incorrecto**: `"email": "123"`
   - **Correcto**: `"email": "usuario@dominio.com"`

4. **"DateTime object expected"**
   - **Solución**: Use formato ISO 8601
   - **Incorrecto**: `"birthDate": "15/01/2000"`
   - **Correcto**: `"birthDate": "2000-01-15T00:00:00Z"`

Para más ayuda, revise los logs de la aplicación o contacte al equipo de desarrollo.
