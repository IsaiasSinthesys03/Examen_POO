@echo off
echo ================================================
echo  SCRIPT DE DEPLOY PARA SOMEE - ExamenPOO API
echo ================================================
echo.

echo [1/5] Limpiando archivos anteriores...
if exist "publish" rmdir /s /q "publish"
if exist "deploy" rmdir /s /q "deploy"

echo [2/5] Restaurando paquetes NuGet...
dotnet restore

echo [3/5] Compilando en modo Release...
dotnet build --configuration Release --no-restore

echo [4/5] Publicando aplicación...
dotnet publish --configuration Release --output "./deploy" --no-build

echo [5/5] Preparando archivos para upload...
copy "web.config" "deploy\" /Y
copy "appsettings.Production.json" "deploy\" /Y

echo.
echo ================================================
echo   DEPLOY COMPLETADO EXITOSAMENTE
echo ================================================
echo.
echo INSTRUCCIONES PARA SOMEE:
echo 1. Comprimir la carpeta 'deploy' completa en un archivo ZIP
echo 2. Subir el ZIP a tu cuenta de SOMEE
echo 3. Extraer en la carpeta raíz de tu aplicación web
echo 4. Asegurarte de que el archivo web.config esté en la raíz
echo.
echo URL de tu API: https://academicuniversity.somee.com/swagger
echo.
pause
