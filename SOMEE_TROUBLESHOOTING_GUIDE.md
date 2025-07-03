# 🚨 GUÍA DE SOLUCIÓN DE PROBLEMAS - SOMEE DEPLOY

## ❌ **ERROR: "ERR_CONNECTION_CLOSED"**

### 🔍 **Causas Comunes:**
1. ❌ Aplicación .NET no inicia correctamente
2. ❌ Archivos de configuración incorrectos
3. ❌ Problemas con la base de datos
4. ❌ Runtime de .NET no encontrado

## ✅ **SOLUCIONES IMPLEMENTADAS**

### **1. web.config Optimizado**
```xml
✅ AspNetCore Module V2 configurado
✅ Variables de entorno de producción
✅ Manejo de errores detallado
✅ Redirección a Swagger
✅ Puerto 80 configurado
```

### **2. Archivos de Compatibilidad Agregados**
```
✅ default.aspx    ← Página de entrada para IIS
✅ global.asax     ← Manejo de aplicación ASP.NET
✅ web.config      ← Configuración IIS optimizada
```

### **3. Configuración de Producción Mejorada**
```json
✅ Connection timeout extendido (30s)
✅ Logging más detallado para debug
✅ AllowedHosts = "*" (más permisivo)
✅ DetailedErrors = true (para debug)
✅ Puerto 80 explícitamente configurado
```

## 🚀 **PASOS PARA RESOLVER EL ERROR**

### **Paso 1: Verificar Archivos**
```bash
📁 Carpeta deploy/ debe contener:
├── ✅ ExamenPOO.API.dll
├── ✅ web.config (ACTUALIZADO)
├── ✅ default.aspx (NUEVO)
├── ✅ global.asax (NUEVO)
├── ✅ appsettings.Production.json (ACTUALIZADO)
└── ✅ Todas las DLLs de Microsoft.*
```

### **Paso 2: Resubir a Somee**
```bash
1. 🗜️ Comprimir carpeta deploy/ completa
2. 🌐 Ir a Somee File Manager
3. 🗑️ BORRAR todos los archivos del sitio
4. ⬆️ Subir el ZIP nuevo
5. 📂 Extraer ZIP en la raíz
6. 🔄 Restart Application en Somee panel
```

### **Paso 3: Verificar URLs**
```
🏠 Sitio principal: https://academicuniversity.somee.com
📚 Swagger: https://academicuniversity.somee.com/swagger
🔐 Login: https://academicuniversity.somee.com/api/auth/login
```

### **Paso 4: Si sigue fallando**
```bash
1. 📋 Revisar logs en Somee Control Panel
2. 🔍 Verificar que la BD esté activa
3. 🔄 Reiniciar aplicación desde panel
4. ⏰ Esperar 2-3 minutos para startup
```

## 🛠️ **DIAGNÓSTICO AVANZADO**

### **Si aún no funciona, verificar:**

1. **🔐 Base de Datos**
   ```sql
   -- La BD debe estar activa en Somee
   -- Connection string debe ser exacto
   -- Credenciales deben ser válidas
   ```

2. **🌐 DNS/Dominio**
   ```
   - El dominio debe estar configurado en Somee
   - Puede tardar hasta 24h en propagarse
   - Verificar en panel de Somee que esté activo
   ```

3. **⚙️ Runtime .NET**
   ```
   - Somee debe tener .NET 8 instalado
   - Verificar en panel qué versiones soporta
   - Cambiar target framework si es necesario
   ```

## 🎯 **CHECKLIST FINAL**

- [ ] ✅ Archivos actualizados subidos
- [ ] ✅ Aplicación reiniciada en Somee
- [ ] ✅ BD activa y accessible
- [ ] ✅ Esperado 2-3 minutos para startup
- [ ] ✅ Probado directamente: /swagger
- [ ] ✅ Verificado logs en Somee panel

## 🔥 **¡DEBERÍA FUNCIONAR AHORA!**

Con estos cambios, tu aplicación debería:
- ✅ Iniciar correctamente en Somee
- ✅ Mostrar Swagger en la raíz
- ✅ Conectar a la base de datos
- ✅ Manejar las APIs correctamente

**Si persiste el error, el problema podría ser:**
- 🚫 Somee no tiene .NET 8 disponible
- 🚫 La base de datos está inactiva
- 🚫 Problema de configuración en el panel de Somee
