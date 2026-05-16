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
