
# NetCash | Sistema de Gestión de Cajas

## Descripción

NetCash es una aplicación web empresarial orientada
a la gestión de cajas, usuarios, clientes, turnos,
contratos y pagos de servicios de telecomunicaciones.

El proyecto fue desarrollado como parte de la prueba
técnica de VIAMATICA, aplicando principios de
arquitectura limpia, seguridad y separación de
responsabilidades.

## Tecnologías utilizadas

### Backend

- C#
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- JWT / JWE
- Serilog
- Swagger / OpenAPI

### Frontend

- Angular 21
- TypeScript
- Bootstrap 5
- Bootstrap Icons
- SCSS
- Reactive Forms
- Angular Router

### Infraestructura

- Docker
- Docker Compose
- Nginx
- PostgreSQL 17
- Git y GitHub
- Postman

## Arquitectura del proyecto

El backend utiliza una arquitectura Onion,
organizada en cuatro capas:

- NetCash.Dominio
- NetCash.Aplicacion
- NetCash.Infraestructura
- NetCash.API

El frontend utiliza componentes standalone,
servicios, guards, interceptores y rutas lazy loading.

## Estructura del repositorio

    NetCash/
    |-- Backend/
    |-- Frontend/
    |-- Database/
    |-- Postman/
    |-- AI/
    |-- docker-compose.yml
    |-- .env.example
    |-- .gitignore
    |-- README.md

## Funcionalidades implementadas

- Autenticación mediante JWT/JWE.
- Gestión de usuarios y aprobación.
- Control de acceso según roles.
- Gestión de clientes.
- Administración de cajas.
- Asignación de cajeros.
- Apertura y cierre de cajas.
- Gestión de turnos.
- Catálogo de servicios y equipos.
- Contratación de servicios.
- Registro de pagos.
- Cambio de servicio.
- Cambio de forma de pago.
- Cancelación de contratos.
- Dashboard con indicadores.
- Menú dinámico basado en roles.
- Eliminación lógica de registros.
- Registro de eventos mediante Serilog.

## Roles del sistema

### Administrador

Administra usuarios, aprobaciones, cajas, catálogos
e indicadores generales del sistema.

### Gestor

Gestiona asignaciones de cajas, cajeros y turnos,
según los permisos implementados.

### Cajero

Realiza operaciones comerciales, atención de clientes,
contrataciones, pagos y apertura o cierre de cajas.

## Instalación mediante Docker

### Requisitos

- Docker Desktop.
- Docker Compose.
- Git.

### Clonar el repositorio

    git clone https://github.com/EdwinBelduma/netcash-viamatica.git
    cd netcash-viamatica

### Configurar variables de entorno

En PowerShell:

    Copy-Item .env.example .env

Completar las variables del archivo .env.

Las tres claves criptográficas deben corresponder
a valores aleatorios de 32 bytes codificados en Base64.

No publicar el archivo .env en GitHub.

### Construir e iniciar los servicios

    docker compose up -d --build

### Verificar los contenedores

    docker compose ps

## Acceso al sistema

| Servicio | URL |
|---|---|
| Frontend | http://localhost:4200 |
| Backend | http://localhost:5000 |
| Swagger | http://localhost:5000/swagger/index.html |
| Estado API | http://localhost:5000/api/estado |

## Usuario inicial

- Usuario: adminnet01
- Contraseña: definida en ADMIN_CONTRASENA.

Las credenciales iniciales se configuran mediante
variables de entorno y no se almacenan en el repositorio.

## Base de datos

Las migraciones de Entity Framework Core se ejecutan
automáticamente durante el inicio del backend.

El directorio Database contiene el script SQL de la
función de consulta de turnos atendidos.

## Seguridad

- Autenticación mediante tokens JWT cifrados con JWE.
- Cifrado de información sensible mediante AES.
- Contraseñas protegidas mediante hashing.
- Autorización basada en roles.
- Protección de rutas del frontend.
- Variables de entorno para información confidencial.
- Eliminación lógica de registros.

## Pruebas

Se realizaron pruebas manuales del inicio de sesión,
dashboard, registro de cajas, servicios, equipos,
asignación de cajeros, atención de turnos,
contratación de servicios y registro de pagos.

El directorio Postman contiene la documentación
exportada de los endpoints de la API.

## Limitaciones y funcionalidades pendientes

- Importación masiva de usuarios y clientes.
- Recuperación de contraseña.
- Ampliación de permisos del gestor.
- Actualización del estado de sesión en el backend
  al cerrar sesión.
- Integración de la función SQL en el dashboard.
- Pruebas adicionales de operaciones comerciales.

## Uso de inteligencia artificial

Se como herramienta de apoyo y orientación
técnica para la estructuración del proyecto, la comprensión
de la arquitectura Onion, la organización de los componentes
y el desarrollo e integración de las funcionalidades del sistema.

## Autor

Edwin Leonel Belduma Belduma

Ingeniero de Software

GitHub: https://github.com/EdwinBelduma

Prueba técnica - VIAMATICA | 2026
