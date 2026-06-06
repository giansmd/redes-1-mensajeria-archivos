@echo off
echo Construyendo winProyComunicacionCOMA...
dotnet build winProyComunicacionCOMA\winProyComunicacionCOMA.csproj -c Debug
echo Construyendo winProyComunicacionCOMB...
dotnet build winProyComunicacionCOMB\winProyComunicacionCOMB.csproj -c Debug

echo.
echo Lanzando ambas instancias en ventanas separadas...
start "SRChat - COMA" winProyComunicacionCOMA\bin\Debug\net8.0-windows\winProyComunicacionCOMA.exe
start "SRChat - COMB" winProyComunicacionCOMB\bin\Debug\net8.0-windows\winProyComunicacionCOMB.exe

echo.
echo Ambas instancias en ejecucion.
echo Cierra las ventanas para terminar.
pause
