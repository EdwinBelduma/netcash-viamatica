# NetCash | Backend

## Descripción

El backend de NetCash proporciona los servicios REST
necesarios para la gestión administrativa y comercial
de cajas, usuarios, clientes, turnos, contratos y pagos
de servicios de telecomunicaciones.

Fue desarrollado como parte de la prueba técnica
de VIAMATICA, aplicando principios de arquitectura
Onion, Domain-Driven Design (DDD), seguridad,
modularidad y separación de responsabilidades.

## Tecnologías utilizadas

- C#
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL 17
- Npgsql
- JWT / JWE
- AES-GCM
- PasswordHasher
- Serilog
- Swagger / OpenAPI
- ASP.NET Core Rate Limiting
- SMTP
- Docker
- Docker Compose

## Arquitectura

El backend se encuentra organizado en cuatro capas.

### NetCash.Dominio

Contiene las entidades, enumeraciones y elementos
fundamentales del negocio.

Responsabilidades:

- Definición de entidades.
- Estados del sistema.
- Reglas fundamentales del dominio.
- Eliminación lógica.
- Propiedades comunes de auditoría.

### NetCash.Aplicacion

Contiene los contratos y servicios de aplicación.

Responsabilidades:

- Interfaces.
- DTOs de solicitud y respuesta.
- Validación de datos.
- Servicios de aplicación.
- Definición de operaciones del negocio.

### NetCash.Infraestructura

Contiene las implementaciones técnicas requeridas
por el sistema.

Responsabilidades:

- Persistencia de datos.
- Entity Framework Core.
- Repositorios.
- Servicios de infraestructura.
- Migraciones de base de datos.
- Cifrado de información sensible.
- Generación de tokens JWT/JWE.
- Recuperación de contraseña.
- Envío de correos electrónicos.
- Importación masiva de archivos CSV.
- Consultas SQL parametrizadas.

### NetCash.API

Es el punto de entrada de la aplicación.

Responsabilidades:

- Controladores REST.
- Configuración de servicios.
- Inyección de dependencias.
- Autenticación.
- Autorización.
- Middleware.
- Configuración CORS.
- Rate Limiting.
- Documentación Swagger.
- Inicialización de la base de datos.
- Configuración de Serilog.

## Estructura del backend

```text
Backend/
├── NetCash.API/
│   ├── Controllers/
│   ├── Program.cs
│   └── appsettings.json
├── NetCash.Aplicacion/
│   ├── DTOs/
│   ├── Interfaces/
│   └── Servicios/
├── NetCash.Dominio/
│   ├── Comunes/
│   ├── Entidades/
│   └── Enumeraciones/
├── NetCash.Infraestructura/
│   ├── Persistencia/
│   │   └── Migraciones/
│   ├── Repositorios/
│   ├── Seguridad/
│   └── Servicios/
├── Dockerfile
├── NetCash.sln
└── README.md
```

## Dependencias entre capas

La arquitectura mantiene la siguiente organización
de dependencias:

```text
NetCash.API
    |
    +-- NetCash.Aplicacion
    |
    +-- NetCash.Infraestructura
             |
             +-- NetCash.Aplicacion
             |
             +-- NetCash.Dominio

NetCash.Aplicacion
    |
    +-- NetCash.Dominio
```

La capa de dominio permanece independiente de
los detalles de infraestructura.

## Modelo de datos

Las principales entidades del sistema son:

- Rol.
- Usuario.
- Cliente.
- Caja.
- UsuarioCaja.
- Turno.
- Servicio.
- Equipo.
- FormaPago.
- Contrato.
- Pago.
- Atencion.
- Menu.
- RolMenu.
- TokenRecuperacion.

La persistencia utiliza PostgreSQL y Entity Framework
Core para administrar relaciones, restricciones,
consultas y migraciones.

## Módulos funcionales

### Autenticación

- Inicio de sesión.
- Validación de credenciales.
- Generación de tokens JWT.
- Cifrado de tokens mediante JWE.
- Consulta del perfil autenticado.
- Control de acceso según roles.

### Usuarios

- Registro de usuarios.
- Consulta de usuarios.
- Actualización de información.
- Eliminación lógica.
- Registro de gestores y cajeros.
- Aprobación administrativa.
- Validación de estados de usuario.
- Control de sesiones.

### Clientes

- Registro de clientes.
- Consulta y búsqueda.
- Actualización de información.
- Validación de identificación.
- Eliminación lógica.
- Importación masiva.

### Cajas

- Creación de cajas.
- Actualización de cajas.
- Asignación de cajeros.
- Eliminación de asignaciones.
- Apertura y cierre de cajas.
- Control de disponibilidad.
- Validación del máximo de dos cajeros por caja.
- Restricción de utilización simultánea.

### Turnos

- Generación de turnos.
- Asignación de caja y cajero.
- Identificación del tipo de atención.
- Inicio de atención.
- Finalización de atención.
- Cancelación de turnos.
- Control de estados.

### Servicios y equipos

- Registro de servicios.
- Consulta de servicios.
- Actualización de servicios.
- Eliminación lógica.
- Administración de equipos asociados.
- Control de precios y características.

### Contratos y pagos

- Contratación de servicios.
- Registro de pagos.
- Cambio de servicio.
- Cambio de forma de pago.
- Cancelación de contratos.
- Control de vigencia contractual.
- Registro de atenciones comerciales.

### Dashboard

- Indicadores generales.
- Cajas ocupadas y disponibles.
- Turnos pendientes.
- Turnos en atención.
- Turnos atendidos.
- Contratos vigentes.
- Total de pagos.
- Total recaudado.
- Indicadores por caja, cajero y gestor.
- Consulta mediante intervalos de fechas.
- Indicadores según el rol autenticado.
- Integración de funciones SQL.

### Menús dinámicos

Los menús se obtienen desde la base de datos
según el rol del usuario autenticado.

### Importación masiva

El backend permite procesar archivos CSV para
la importación de usuarios y clientes.

Incluye:

- Validación de encabezados.
- Validación de registros.
- Procesamiento individual de filas.
- Identificación de registros rechazados.
- Resumen del resultado.
- Mensajes de errores de validación.

### Recuperación de contraseña

- Solicitud mediante correo electrónico.
- Generación de tokens criptográficamente seguros.
- Almacenamiento del hash SHA-256 del token.
- Vigencia de 15 minutos.
- Invalidación de tokens anteriores.
- Validación de uso único.
- Restablecimiento seguro de contraseña.
- Envío de enlaces mediante SMTP.
- Limitación de solicitudes.

## Documentación de endpoints

| Módulo | Ruta base |
|---|---|
| Autenticación | /api/autenticacion |
| Recuperación | /api/recuperacion |
| Usuarios | /api/usuarios |
| Clientes | /api/clientes |
| Cajas | /api/cajas |
| Turnos | /api/turnos |
| Servicios | /api/servicios |
| Equipos | /api/equipos |
| Contratos | /api/contratos |
| Pagos | /api/pagos |
| Formas de pago | /api/formas-pago |
| Dashboard | /api/dashboard |
| Menús | /api/menus |
| Importaciones | /api/importaciones |
| Estado | /api/estado |

La documentación detallada de métodos HTTP,
parámetros y modelos de datos se encuentra
disponible mediante Swagger.

## Autenticación y autorización

El sistema utiliza autenticación mediante JWT
y cifrado de tokens mediante JWE.

Los endpoints protegidos requieren el encabezado:

```http
Authorization: Bearer TOKEN
```

La autorización se realiza mediante los roles:

- Administrador.
- Gestor.
- Cajero.

Las contraseñas se protegen mediante hashing,
evitando almacenar contraseñas recuperables.

La información sensible de los usuarios se
protege mediante cifrado AES-GCM.

## Ejemplo de autenticación

Endpoint:

```http
POST /api/autenticacion/login
```

Cuerpo de la solicitud:

```json
{
    "nombreUsuario": "adminnet01",
    "contrasena": "CONTRASENA_CONFIGURADA"
}
```

La respuesta contiene el token de autenticación,
su fecha de expiración y la información del usuario.

No utilizar credenciales reales en documentación
o archivos de pruebas compartidos.

## Ejemplo de recuperación de contraseña

Solicitar recuperación:

```http
POST /api/recuperacion/solicitar
```

```json
{
    "correo": "usuario@example.com"
}
```

Restablecer contraseña:

```http
POST /api/recuperacion/restablecer
```

```json
{
    "token": "TOKEN_RECIBIDO",
    "nuevaContrasena": "NUEVA_CONTRASENA",
    "confirmarContrasena": "NUEVA_CONTRASENA"
}
```

El token debe encontrarse vigente y no haber sido
utilizado anteriormente.

## Base de datos y migraciones

Las migraciones de Entity Framework Core se
encuentran en:

```text
NetCash.Infraestructura/Persistencia/Migraciones/
```

Las migraciones se aplican automáticamente
durante la inicialización de la API.

Para generar una nueva migración desde Backend:

```powershell
dotnet ef migrations add NombreMigracion --project NetCash.Infraestructura --startup-project NetCash.API
```

La generación de nuevas migraciones requiere
tener disponible la herramienta dotnet-ef.

## Funciones almacenadas de PostgreSQL

El sistema incorpora la función:

```sql
fn_turnos_atendidos
```

Esta función permite obtener la cantidad de
turnos atendidos en un intervalo de fechas.

También permite aplicar un filtro opcional
por cajero.

Ejemplo de ejecución:

```sql
SELECT fn_turnos_atendidos(
    NOW() - INTERVAL '1 day',
    NOW(),
    NULL
);
```

La función se crea automáticamente mediante
una migración de Entity Framework Core.

El script SQL se encuentra disponible en:

```text
Database/funciones.sql
```

La integración con el dashboard utiliza
consultas parametrizadas de Entity Framework Core.

Para el rol Gestor se utilizan consultas filtradas
mediante Entity Framework, conservando el alcance
de sus operaciones.

## Configuración

Las configuraciones sensibles se administran
mediante variables de entorno.

| Variable | Descripción |
|---|---|
| POSTGRES_DB | Nombre de la base de datos |
| POSTGRES_USER | Usuario de PostgreSQL |
| POSTGRES_PASSWORD | Contraseña de PostgreSQL |
| JWT_CLAVE_FIRMA | Clave de firma JWT |
| JWT_CLAVE_CIFRADO | Clave de cifrado JWE |
| SEGURIDAD_CLAVE_CIFRADO | Clave de cifrado AES |
| ADMIN_CONTRASENA | Contraseña inicial del administrador |

Las claves criptográficas deben ser independientes
y tener una longitud de 32 bytes codificados
en Base64.

Las credenciales no deben almacenarse directamente
en el código fuente.

## Compilación local

Requisitos:

- SDK de .NET 10.
- PostgreSQL disponible.
- Configuración local de conexión y seguridad.
- Herramientas de Entity Framework Core.

Desde el directorio Backend:

```powershell
dotnet restore NetCash.sln
dotnet build NetCash.sln
```

Para ejecutar localmente:

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run --project NetCash.API --no-launch-profile --urls http://localhost:5000
```

La ejecución local requiere configurar previamente
la conexión de PostgreSQL y las claves de seguridad
mediante User Secrets o variables de entorno.

Para evaluar el proyecto se recomienda utilizar
Docker Compose, debido a que configura los servicios
de manera centralizada.

## Ejecución mediante Docker

Desde el directorio principal del repositorio:

```bash
docker compose up -d --build
```

Verificar los servicios:

```bash
docker compose ps
```

Reconstruir únicamente el backend:

```bash
docker compose up -d --build backend
```

Consultar registros:

```bash
docker compose logs --tail=60 backend
```

## Acceso a la API

| Servicio | Dirección |
|---|---|
| Backend | http://localhost:5000 |
| Swagger | http://localhost:5000/swagger/index.html |
| Estado de la API | http://localhost:5000/api/estado |
| Mailpit | http://localhost:8025 |

## Usuario administrador inicial

El sistema crea automáticamente un administrador
durante la inicialización cuando este no existe.

- Usuario: adminnet01
- Contraseña: definida en ADMIN_CONTRASENA.

La contraseña debe configurarse antes de iniciar
los contenedores por primera vez.

## Servicio de correo electrónico

El envío de correos de recuperación utiliza SMTP.

Para el entorno local se utiliza Mailpit,
configurado mediante Docker Compose.

Mailpit permite visualizar los mensajes enviados
sin necesidad de utilizar una cuenta de correo real.

Bandeja de entrada:

http://localhost:8025

Para producción se requiere configurar un proveedor
SMTP externo y habilitar las medidas correspondientes
de seguridad del transporte.

## Auditoría con Serilog

El backend incorpora Serilog para el registro de
eventos durante la ejecución del sistema.

Los registros incluyen:

- Solicitudes HTTP.
- Eventos de autenticación.
- Operaciones del sistema.
- Advertencias.
- Errores.
- Inicialización de servicios.
- Ejecución de migraciones.

Los archivos de registro se almacenan en:

```text
/app/logs
```

Configuración:

- Rotación diaria.
- Retención de hasta 30 archivos.
- Registro en consola.
- Registro en archivos.
- Persistencia mediante volúmenes Docker.

Consultar archivos disponibles:

```bash
docker compose exec backend ls -lh /app/logs
```

No deben registrarse contraseñas, claves privadas
ni tokens completos de autenticación o recuperación.

## Seguridad y buenas prácticas

- Separación de responsabilidades.
- Inyección de dependencias.
- Autenticación JWT/JWE.
- Cifrado AES-GCM.
- Hashing de contraseñas.
- Autorización mediante roles.
- Validación de solicitudes.
- Eliminación lógica.
- Consultas SQL parametrizadas.
- Transacciones de base de datos.
- Tokens de recuperación temporales.
- Limitación de solicitudes.
- Variables de entorno.
- Registro de errores y advertencias.

## Pruebas funcionales

Durante el desarrollo se realizaron pruebas
manuales de los principales módulos mediante
Swagger, Postman y el frontend Angular.

Entre las funcionalidades comprobadas se incluyen:

- Autenticación.
- Gestión de usuarios.
- Gestión de clientes.
- Administración de cajas.
- Asignación de cajeros.
- Gestión de turnos.
- Catálogo de servicios.
- Contratación de servicios.
- Registro de pagos.
- Importación masiva CSV.
- Recuperación de contraseña.
- Ejecución de funciones SQL.
- Indicadores del dashboard.
- Inicialización mediante Docker.

## Consideraciones y limitaciones

El backend fue desarrollado para un entorno
local de evaluación técnica.

Como oportunidades de mejora se consideran:

- Ampliación de pruebas unitarias automatizadas.
- Pruebas de integración automatizadas.
- Implementación de CI/CD.
- Configuración HTTPS para producción.
- Revocación completa de tokens y sesiones.
- Integración con proveedores SMTP productivos.
- Monitoreo centralizado de aplicaciones.
- Optimización para grandes volúmenes de datos.
- Revisión integral de seguridad antes del
  despliegue productivo.

La importación masiva se implementó mediante CSV.

La protección de contraseñas mediante hashing
fue una decisión técnica de seguridad para evitar
el almacenamiento reversible de credenciales.

## Uso de inteligencia artificial

Se utilizó IA de apoyo
durante el desarrollo para la organización de
la arquitectura,resolución de errores y documentación técnica.

Las sugerencias fueron incorporadas progresivamente
mediante revisión, configuración y pruebas manuales.

La documentación complementaria de los prompts
se encuentra en el directorio AI del repositorio.

## Documentación complementaria

- [README principal](../README.md)
- [Documentación Frontend](../Frontend/README.md)
- [Scripts SQL](../Database/)
- [Documentación Postman](../Postman/)
- [Documentación IA](../AI/)

## Autor

**Edwin Leonel Belduma Belduma**

Ingeniero de Software

GitHub: https://github.com/EdwinBelduma

Repositorio:
https://github.com/EdwinBelduma/netcash-viamatica

---

**NetCash Backend | Prueba técnica VIAMATICA | 2026**