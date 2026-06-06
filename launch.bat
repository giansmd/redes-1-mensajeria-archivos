@echo off
echo Construyendo winProyComunicacionCOM1...
dotnet build winProyComunicacionCOM1\winProyComunicacionCOM1.csproj -c Debug
echo Construyendo winProyComunicacionCOM2...
dotnet build winProyComunicacionCOM2\winProyComunicacionCOM2.csproj -c Debug

echo.
echo Lanzando ambas instancias en ventanas separadas...
start "winProyComunicacion - COM1" winProyComunicacionCOM1\bin\Debug\net8.0-windows\winProyComunicacionCOM1.exe
start "winProyComunicacion - COM2" winProyComunicacionCOM2\bin\Debug\net8.0-windows\winProyComunicacionCOM2.exe

echo.
echo Ambas instancias en ejecucion.
echo Cierra las ventanas para terminar.
pause
