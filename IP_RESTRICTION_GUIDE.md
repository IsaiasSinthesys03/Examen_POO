# 🛡️ Guía de Restricción de IP - API ExamenPOO

## 📋 Implementación Realizada

Se ha implementado la **Opción 2: Configuración de CORS Restrictiva** para limitar el acceso de la API únicamente a la IP **187.155.101.200**.

## 🔧 Cambios Realizados

### 1. Modificación de Program.cs
- **CORS configurado** para permitir solo la IP específica
- **Configuración dinámica** que lee desde appsettings.json
- **Soporte HTTP/HTTPS** automático
- **Credenciales habilitadas** para autenticación JWT

### 2. Configuración en appsettings.json
```json
{
  "AllowedIPs": {
    "Enabled": true,
    "IPs": ["187.155.101.200"]
  }
}
```

### 3. Configuración de Desarrollo
En `appsettings.Development.json`:
```json
{
  "AllowedIPs": {
    "Enabled": false,
    "IPs": ["localhost", "127.0.0.1", "187.155.101.200"]
  }
}
```

## ⚙️ Cómo Funciona

### En Producción (`Enabled: true`)
- ✅ Solo permite acceso desde **187.155.101.200**
- ✅ Bloquea **todas las demás IPs**
- ✅ Soporta **HTTP y HTTPS**
- ✅ Permite **credenciales JWT**

### En Desarrollo (`Enabled: false`)
- ✅ Permite **cualquier origen** (modo desarrollo)
- ✅ No restringe IPs para facilitar desarrollo local
- ✅ Mantiene funcionalidad completa

## 🚀 Configuración de Despliegue

### Para Habilitar la Restricción:
1. En `appsettings.json` (producción):
   ```json
   {
     "AllowedIPs": {
       "Enabled": true,
       "IPs": ["187.155.101.200"]
     }
   }
   ```

### Para Agregar Más IPs:
```json
{
  "AllowedIPs": {
    "Enabled": true,
    "IPs": [
      "187.155.101.200",
      "192.168.1.100",
      "10.0.0.50"
    ]
  }
}
```

### Para Desactivar la Restricción:
```json
{
  "AllowedIPs": {
    "Enabled": false
  }
}
```

## 🔍 Verificación

### Acceso Permitido (desde 187.155.101.200):
- ✅ **200 OK** - Respuestas normales de la API
- ✅ **Swagger UI** funcionando
- ✅ **Autenticación JWT** funcionando

### Acceso Bloqueado (desde otras IPs):
- ❌ **CORS Error** en navegadores
- ❌ **Network Error** en aplicaciones
- ❌ **Blocked by CORS policy** en consola del navegador

## ⚠️ Limitaciones de CORS

### Lo que SÍ bloquea:
- ✅ **Navegadores web** (Chrome, Firefox, Safari, etc.)
- ✅ **Aplicaciones web JavaScript**
- ✅ **PWAs y SPAs**

### Lo que NO bloquea completamente:
- ⚠️ **Herramientas como Postman** (pueden ignorar CORS)
- ⚠️ **cURL desde terminal**
- ⚠️ **Aplicaciones que no respetan CORS**

## 🛠️ Comandos de Prueba

### Desde IP Permitida (187.155.101.200):
```bash
curl -X GET "https://tu-api.com/swagger" -H "Accept: application/json"
# Resultado: 200 OK
```

### Desde IP No Permitida:
```bash
curl -X GET "https://tu-api.com/swagger" -H "Accept: application/json"
# Resultado: CORS Error (en navegadores)
```

## 📝 Mantenimiento

### Para Cambiar la IP Permitida:
1. Editar `appsettings.json`
2. Cambiar el valor en `AllowedIPs.IPs`
3. Reiniciar la aplicación

### Para Modo Desarrollo:
1. Usar `appsettings.Development.json`
2. Establecer `Enabled: false`
3. La API funcionará sin restricciones

## 🔒 Consideraciones de Seguridad

1. **CORS no es seguridad completa** - Es una medida de protección parcial
2. **Para máxima seguridad** - Considera implementar también:
   - Middleware de filtrado de IP
   - Firewall a nivel de servidor
   - VPN o túneles seguros
3. **Monitoreo** - Implementar logs para detectar intentos de acceso

## ✅ Estado Actual

- ✅ **Implementado**: Restricción CORS por IP
- ✅ **Configurado**: Para IP 187.155.101.200
- ✅ **Flexible**: Configurable via appsettings.json
- ✅ **Desarrollo**: Sin restricciones en modo Development
- ✅ **Producción**: Restricción activa
