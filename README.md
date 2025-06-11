# TorneosBasketBall
![image](https://github.com/user-attachments/assets/d1cd83f5-b61f-4c24-a54d-fdc3f80b88ad)

Torneos Basketball UABC es una aplicación web desarrollada en ASP.NET Core MVC que permite gestionar torneos de basketball de forma integral, con funcionalidades de autenticación de administrador, administración de equipos, jugadores, partidos, generación de calendarios round-robin, estadísticas y un dashboard interactivo.

Características principales

Autenticación de administrador basada en sesiones.

CRUD completo para:

Equipos

Jugadores

Partidos

Estadísticas de equipos

Generación automática de calendario round-robin para enfrentamientos equitativos.

Dashboard interactivo con:

Filtrado por equipo

Paginación de la lista de equipos

Tablas de jugadores, partidos y estadísticas actualizadas

Interfaz responsiva con Bootstrap.

Validación de formularios con jQuery Validation.

Tecnologías y dependencias

Framework: .NET 9.0, ASP.NET Core MVC

ORM: Entity Framework Core

Base de datos: SQL Server

Front-end: Bootstrap (CDN), HTML5/CSS3

Validación: jQuery Validation, Unobtrusive

Instalación y puesta en marcha

Clona este repositorio:

git clone https://github.com/tu-usuario/torneos-basketball-uabc.git
cd torneos-basketball-uabc

Configura la cadena de conexión en appsettings.json:

"ConnectionStrings": {
  "DefaultConnection": "Server=TU_SERVIDOR;Database=TorneoBasketBall;Trusted_Connection=True;TrustServerCertificate=True;"
}

Restaura paquetes NuGet y build del proyecto:

dotnet restore
dotnet build

Aplica migraciones y genera la base de datos:

dotnet ef database update

Ejecuta la aplicación:

dotnet run

Abre tu navegador en https://localhost:7263/Login para iniciar sesión como administrador.

Uso de la aplicación

/Login: Formulario de acceso para administradores.

/Admin: Panel principal con accesos rápidos al CRUD y al generador round-robin.

/Home: Dashboard con resumen de equipos, jugadores, partidos y estadísticas.

/Equipoes, /Jugadores, /Partidos, /EstadisticasEquipos: Rutas para gestión detallada.
