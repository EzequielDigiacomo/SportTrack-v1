# SportTrack Backend - .NET 8 🚣‍♂️

Sistema de gestión deportiva avanzada para eventos de remo y canotaje. Esta API RESTful proporciona toda la lógica de negocio, persistencia de datos y sincronización en tiempo real para la plataforma SportTrack.

---

## 🚀 Características Principales

*   **Arquitectura Multi-tenant (SaaS):** Soporte para múltiples planes de suscripción (Bronce, Plata, Oro) y jerarquías de clubes.
*   **Autenticación y Autorización Segura:** Implementación de **JWT (JSON Web Tokens)** utilizando cookies `HttpOnly` para máxima seguridad y prevención de ataques XSS.
*   **Tiempo Real (Real-Time):** Integración con **SignalR** para la transmisión de resultados y tiempos oficiales de las regatas al instante.
*   **Gestión de Competencias:** Creación de eventos, pruebas, series (heats), semifinales y finales con lógica de promoción automática de atletas.
*   **Trazabilidad y Auditoría:** Middleware personalizado para captura de excepciones y logs de acciones administrativas.

---

## 🛠️ Stack Tecnológico

*   **Framework:** .NET 8 (Web API)
*   **Base de Datos:** PostgreSQL
*   **ORM:** Entity Framework Core 8 (Code-First)
*   **Real-Time:** ASP.NET Core SignalR
*   **Documentación API:** Swagger / OpenAPI
*   **Seguridad:** BCrypt (Hasheo de contraseñas), JWT (Autenticación)

---

## 📁 Estructura del Proyecto

El proyecto sigue principios de Arquitectura Limpia (Clean Architecture), dividido en los siguientes módulos:

*   `SportTrack-v1.Api`: Punto de entrada de la aplicación, Controladores REST, Middlewares y configuración de inyección de dependencias.
*   `SportTrack-v1.Controladores`: Lógica de negocio (Servicios), DTOs e interfaces.
*   `SportTrack-v1.AccesoDatos`: Contexto de Entity Framework (`ApplicationDbContext`), Migraciones y Repositorios.
*   `SportTrack-v1.Dominio`: Entidades del núcleo (Core), Modelos de base de datos y Enums.

👉 Para más detalles, consulta la [Documentación de Arquitectura](docs/ARCHITECTURE.md).

---

## ⚙️ Configuración Rápida y Requisitos

### Requisitos Previos
*   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
*   PostgreSQL Server (Local o en Docker)
*   Entity Framework Core CLI (`dotnet tool install --global dotnet-ef`)

### Instalación

1. **Clonar el repositorio:**
   ```bash
   git clone https://github.com/EzequielDigiacomo/SportTrack-v1.git
   cd SportTrack-v1
   ```

2. **Configurar Variables de Entorno (`appsettings.json`):**
   Crea o modifica el archivo `appsettings.json` en el proyecto de la API (`SportTrack-v1.Api`) con tus credenciales:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=SportTrackDB;Username=SportTrackDBAdmin;Password=Admin2508"
     },
     "TokenKey": "SportTrackSuperSecretKey2026!ForEducationalPurposeOnly_LongEnoughToBeSecure",
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning"
       }
     },
     "AllowedHosts": "*"
   }
   ```

3. **Ejecutar Migraciones:**
   Aplica la estructura de la base de datos ejecutando desde la raíz del proyecto:
   ```bash
   dotnet ef database update --project SportTrack-v1.AccesoDatos --startup-project SportTrack-v1.Api
   ```

4. **Ejecutar el Proyecto:**
   ```bash
   dotnet run --project SportTrack-v1.Api
   ```

---

## 📖 Documentación de la API

Una vez que la aplicación esté en ejecución, puedes explorar y probar todos los endpoints a través de la interfaz de Swagger:

*   **URL Local:** `http://localhost:5029/swagger` (o el puerto configurado)

---

## 💻 Comandos Útiles de Desarrollo

**Crear una nueva migración de BD:**
```bash
dotnet ef migrations add NombreDeLaMigracion --project SportTrack-v1.AccesoDatos --startup-project SportTrack-v1.Api
```

**Limpiar y recompilar la solución:**
```bash
dotnet clean
dotnet build
```
