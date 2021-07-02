# Appets-Backend

Backend para la aplicación de Appets

## Entity Framework

Se utilizará la herramienta de EF para generar la base de datos del sistema.

Se utiliza SQL Server 2019 y SQL Server Managment v18.5 para visualizar la BD.

Para poder utilizar la versión 3.1.3 del paquete Microsoft.EntityFrameworkCore.Tools (el cual permite realizar las migraciones de la BD) es necesario actualizar el SDK de dotnet a la versión 3.1
(Link de descarga del SDK: https://dotnet.microsoft.com/download/dotnet-core/thank-you/sdk-3.1.201-windows-x64-installer).

Para generar una nueva migración de la base de datos, debemos pararnos sobre WebApi y ejecutar el comando 'dotnet ef migrations add Appets_(version) -p ../Appets.DataAccess'.
Dicho comando generará una carpeta Migrations en el proyecto Appets.DataAccess y dentro de ella se guardarán todas las migraciones de la BD.
Cada migración consta de 3 archivos con toda la metadata necesaria para crear y configurar la BD según el código de EF realizado en el proyecto de Appets.DataAccess.

Para actualizar la BD usando la última migración debemos ejecutar 'dotnet ef database update -p ../Appets.DataAccess' desde WebApi

Es importante recordar que para ver impactado un cambio en la estructura de la BD se debe realizar una migración y posteriormente un update de la misma
