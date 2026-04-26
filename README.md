# PolyStore - E-Commerce para Impresión 3D

PolyStore es una solución Fullstack moderna diseñada para gestionar una tienda online de productos impresos en 3D. El proyecto utiliza una **Arquitectura Limpia (Clean Architecture)** para separar la lógica de negocio de la infraestructura, garantizando un código escalable y fácil de mantener.

## 🚀 Tecnologías Utilizadas

* **Backend:** .NET 9 (C#) con ASP.NET Core Web API.
* **Base de Datos:** PostgreSQL.
* **ORM:** Entity Framework Core (Code First).
* **Documentación de API:** Scalar (moderno sustituto de Swagger).
* **Arquitectura:** Clean Architecture (Core, Infrastructure, API).

## 🛠️ Características Principales

* **Gestión de Productos:** CRUD completo de productos con soporte para GUIDs.
* **Sistema de Clasificación:** Soporte para etiquetas (Tags) dinámicas mediante arrays de PostgreSQL.
* **Lógica de Estado Inteligente:** Sistema automático que garantiza que solo un producto esté marcado como "Live" (Activo) a la vez, archivando automáticamente los anteriores.
* **Multimedia:** Gestión de URLs para imágenes principales, galerías, videos y renders 3D.

## 📂 Estructura del Proyecto

* **PolyStore.Core:** Entidades de dominio e interfaces.
* **PolyStore.Infrastructure:** Implementación de repositorios y contexto de base de datos.
* **PolyStore.API:** Endpoints REST y configuración del sistema.

## 🔧 Configuración Local

1.  Clonar el repositorio.
2.  Configurar la cadena de conexión en `appsettings.json`.
3.  Ejecutar las migraciones:
    ```bash
    dotnet ef database update --project PolyStore.Infrastructure --startup-project PolyStore.API
    ```
4.  Lanzar la aplicación y acceder a la documentación en `/scalar/v1`.