# NetCash | Sistema de Gestión de Cajas

## Descripción

NetCash es una aplicación web empresarial orientada
a la gestión integral de cajas, usuarios, clientes, turnos,
contratos y pagos de servicios de telecomunicaciones.

El proyecto fue desarrollado como parte de la prueba
técnica de VIAMATICA, aplicando principios de arquitectura
limpia, Domain-Driven Design (DDD), seguridad, modularidad
y separación de responsabilidades.

Su objetivo es centralizar las operaciones administrativas
y comerciales, facilitando el control de cajas, la atención
de clientes y el seguimiento de indicadores operativos.

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
- ASP.NET Core Rate Limiting
- SMTP

### Frontend

- Angular 21
- TypeScript
- Bootstrap 5
- Bootstrap Icons
- SCSS
- Reactive Forms
- Angular Router
- Angular Signals
- RxJS

### Infraestructura y herramientas

- Docker
- Docker Compose
- Nginx
- PostgreSQL 17
- Mailpit
- Git y GitHub
- Postman
- Visual Studio Code

## Arquitectura del proyecto

### Backend

El backend utiliza una arquitectura Onion, organizada
en cuatro capas principales:

- **NetCash.Dominio:** entidades, enumeraciones y reglas
  fundamentales del negocio.

- **NetCash.Aplicacion:** interfaces, DTOs y servicios
  de aplicación.

- **NetCash.Infraestructura:** persistencia, repositorios,
  seguridad, servicios externos y acceso a PostgreSQL.

- **NetCash.API:** controladores REST, autenticación,
  autorización, configuración y middleware.

Esta organización permite mantener la separación de
responsabilidades y facilita el mantenimiento y la
escalabilidad del sistema.

### Frontend

El frontend utiliza una arquitectura modular basada
en componentes standalone de Angular.

Incluye:

- Componentes reutilizables.
- Servicios para consumo de APIs REST.
- Guards de autenticación y autorización.
- Interceptores HTTP.
- Formularios reactivos.
- Rutas con Lazy Loading.
- Menú dinámico según roles.
- Manejo de estados mediante Signals.
- Diseño adaptable a diferentes dispositivos.

## Estructura del repositorio

```text
NetCash/
├── AI/
│   └── PROMPTS.md
├── Backend/
│   ├── NetCash.Dominio/
│   ├── NetCash.Aplicacion/
│   ├── NetCash.Infraestructura/
│   ├── NetCash.API/
│   ├── NetCash.sln
│   └── README.md
├── Database/
│   ├── Plantillas/
│   └── funciones.sql
├── Frontend/
│   ├── src/
│   ├── Dockerfile
│   ├── nginx.conf
│   └── README.md
├── Postman/
├── docker-compose.yml
├── .env.example
├── .gitignore
└── README.md
```

## Funcionalidades implementadas

### Autenticación y seguridad

- Inicio de sesión mediante JWT/JWE.
- Autorización basada en roles.
- Cifrado de información sensible.
- Protección de contraseñas mediante hashing.
- Recuperación de contraseña mediante correo electrónico.
- Restablecimiento de contraseña mediante tokens temporales.
- Validación de vigencia y uso único de tokens de recuperación.
- Limitación de solicitudes de recuperación.
- Protección de rutas del frontend.
- Control del estado de sesión de usuarios.

### Gestión de usuarios

- Registro de usuarios.
- Consulta y actualización de usuarios.
- Eliminación lógica.
- Gestión de gestores y cajeros.
- Registro de usuarios por parte del gestor.
- Aprobación de usuarios por parte del administrador.
- Validación de credenciales y datos personales.
- Importación masiva mediante archivos CSV.

### Gestión de clientes

- Registro de clientes.
- Consulta y búsqueda de clientes.
- Actualización de información.
- Validación de identificación.
- Eliminación lógica.
- Importación masiva mediante archivos CSV.
- Descarga de plantillas de importación.

### Gestión de cajas

- Creación y administración de cajas.
- Asignación de cajeros.
- Validación del máximo de dos cajeros asignados por caja.
- Apertura y cierre de cajas.
- Control de disponibilidad.
- Restricción de utilización simultánea de una misma caja.

### Gestión de turnos

- Generación de turnos.
- Asignación de turnos a cajas y cajeros.
- Identificación del tipo de atención.
- Inicio y finalización de atención.
- Control de estados.
- Cancelación de turnos.
- Consulta de turnos atendidos.

### Gestión comercial

- Administración de servicios de telecomunicaciones.
- Registro de equipos asociados a servicios.
- Contratación de servicios.
- Registro de pagos.
- Cambio de servicio contratado.
- Cambio de forma de pago.
- Cancelación de contratos.
- Control del estado contractual.
- Registro de atenciones comerciales.

### Dashboard

- Indicadores generales del sistema.
- Total de clientes y usuarios.
- Cajas disponibles y ocupadas.
- Turnos pendientes, en atención y atendidos.
- Contratos vigentes.
- Total de pagos y valores recaudados.
- Indicadores por caja, cajero y gestor.
- Consulta mediante filtros de fechas.
- Visualización del estado de sesión de usuarios.
- Consulta de usuarios pendientes de aprobación.
- Integración de funciones almacenadas de PostgreSQL.

### Auditoría

- Registro de eventos mediante Serilog.
- Registro de solicitudes HTTP.
- Registro de operaciones del sistema.
- Registro de errores y advertencias.
- Almacenamiento de logs en archivos.
- Rotación diaria de archivos de registro.
- Persistencia de logs mediante volúmenes de Docker.

## Roles del sistema

### Administrador

Responsable de la administración general del sistema.

Entre sus funcionalidades se incluyen:

- Gestión de usuarios.
- Aprobación de gestores y cajeros.
- Administración de cajas.
- Gestión de catálogos.
- Importación masiva de usuarios.
- Consulta de indicadores generales.
- Supervisión de sesiones.

### Gestor

Responsable de la coordinación de las operaciones.

Entre sus funcionalidades se incluyen:

- Registro de gestores y cajeros sujetos a aprobación.
- Asignación de cajeros a cajas.
- Gestión y asignación de turnos.
- Consulta de indicadores operativos.
- Seguimiento de usuarios pendientes de aprobación.

### Cajero

Responsable de la atención directa a los clientes
y de la ejecución de operaciones comerciales.

Entre sus funcionalidades se incluyen:

- Apertura y cierre de cajas.
- Registro y consulta de clientes.
- Importación masiva de clientes.
- Atención de turnos.
- Contratación de servicios.
- Registro de pagos.
- Cambio de servicios.
- Cambio de forma de pago.
- Cancelación de contratos.

## Instalación mediante Docker

### Requisitos previos

- Docker Desktop.
- Docker Compose.
- Git.
- Navegador web actualizado.

Docker Desktop debe encontrarse en ejecución antes
de iniciar el proyecto.

### 1. Clonar el repositorio

```bash
git clone https://github.com/EdwinBelduma/netcash-viamatica.git
cd netcash-viamatica
```

### 2. Configurar las variables de entorno

En PowerShell:

```powershell
Copy-Item .env.example .env
```

En Linux o macOS:

```bash
cp .env.example .env
```

Completar las variables de entorno correspondientes.

| Variable | Descripción |
|---|---|
| POSTGRES_DB | Nombre de la base de datos |
| POSTGRES_USER | Usuario de PostgreSQL |
| POSTGRES_PASSWORD | Contraseña de PostgreSQL |
| JWT_CLAVE_FIRMA | Clave de firma de tokens JWT |
| JWT_CLAVE_CIFRADO | Clave de cifrado JWE |
| SEGURIDAD_CLAVE_CIFRADO | Clave de cifrado de datos sensibles |
| ADMIN_CONTRASENA | Contraseña inicial del administrador |

Las tres claves criptográficas deben corresponder
a valores aleatorios de 32 bytes codificados en Base64.

Para generar una clave desde PowerShell:

```powershell
$bytes = New-Object byte[] 32
$rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
$rng.GetBytes($bytes)
[Convert]::ToBase64String($bytes)
$rng.Dispose()
```

Ejecutar el procedimiento tres veces para obtener
claves independientes.

No publicar el archivo `.env` en GitHub.

No modificar las claves criptográficas de una instalación
existente sin un procedimiento de migración de datos,
ya que podría impedir el descifrado de información
almacenada anteriormente.

### 3. Construir e iniciar los servicios

```bash
docker compose up -d --build
```

### 4. Verificar los contenedores

```bash
docker compose ps
```

### 5. Consultar los registros

```bash
docker compose logs --tail=60 backend
```

## Servicios Docker

| Contenedor | Descripción | Puerto local |
|---|---|---|
| netcash-postgres | Base de datos PostgreSQL | 5433 |
| netcash-backend | ASP.NET Core Web API | 5000 |
| netcash-frontend | Angular mediante Nginx | 4200 |
| netcash-mailpit | Servidor de correo de pruebas | 8025 |

Los servicios se comunican mediante la red interna
administrada por Docker Compose.

## Acceso al sistema

| Servicio | URL |
|---|---|
| Frontend | http://localhost:4200 |
| Backend | http://localhost:5000 |
| Swagger | http://localhost:5000/swagger/index.html |
| Estado de la API | http://localhost:5000/api/estado |
| Mailpit | http://localhost:8025 |

## Usuario inicial

- Usuario: `adminnet01`
- Contraseña: definida en `ADMIN_CONTRASENA`.

El administrador inicial se crea automáticamente
durante la inicialización del backend cuando no
existe previamente.

Las credenciales se configuran mediante variables
de entorno y no se almacenan en el repositorio.

## Base de datos

El sistema utiliza PostgreSQL 17 como motor de
base de datos relacional.

La persistencia se realiza mediante Entity Framework
Core, utilizando migraciones para administrar
la estructura de la base de datos.

Las migraciones se ejecutan automáticamente durante
el inicio del backend.

El sistema utiliza eliminación lógica para conservar
el historial de los registros.

### Funciones almacenadas

El proyecto incorpora la función PostgreSQL:

`fn_turnos_atendidos`

Esta función permite consultar la cantidad de turnos
atendidos dentro de un intervalo de fechas y admite
un filtro opcional por cajero.

La función se crea automáticamente mediante una
migración de Entity Framework Core.

También se encuentra documentada en:

`Database/funciones.sql`

Su integración con el dashboard se realiza mediante
consultas SQL parametrizadas.

## Recuperación de contraseña

El proceso de recuperación de contraseña permite
restablecer las credenciales de los usuarios registrados.

### Flujo de recuperación

1. El usuario selecciona la opción
   "¿Olvidaste tu contraseña?".
2. Ingresa su correo electrónico registrado.
3. El backend genera un token criptográficamente seguro.
4. Se almacena únicamente el hash del token.
5. Se envía un enlace mediante SMTP.
6. El usuario accede al enlace de recuperación.
7. Ingresa y confirma su nueva contraseña.
8. El backend valida el token y actualiza las credenciales.
9. El token queda invalidado para usos posteriores.

Los tokens de recuperación tienen una vigencia
de 15 minutos y son de uso único.

### Mailpit

Mailpit se utiliza como servidor SMTP local para
verificar el envío de correos electrónicos durante
el desarrollo y las pruebas.

La bandeja de entrada está disponible en:

http://localhost:8025

Mailpit captura los mensajes dentro del entorno
de demostración y no realiza entregas a cuentas
de correo electrónico externas.

Para un entorno de producción se requiere configurar
un proveedor SMTP real y utilizar HTTPS.

## Importación masiva

NetCash permite importar usuarios y clientes mediante
archivos CSV.

El sistema incluye:

- Descarga de plantillas.
- Validación de estructura.
- Validación de campos.
- Procesamiento de registros.
- Identificación de registros rechazados.
- Resumen de resultados.
- Visualización de errores de importación.

Las plantillas también están disponibles en:

`Database/Plantillas/`

Los datos incluidos en las plantillas son ejemplos
destinados exclusivamente a pruebas.

## Seguridad

El sistema incorpora las siguientes medidas:

- Autenticación mediante JWT.
- Cifrado de tokens mediante JWE.
- Cifrado de información sensible mediante AES-GCM.
- Contraseñas protegidas mediante hashing.
- Autorización basada en roles.
- Guards de autenticación y autorización en Angular.
- Consultas SQL parametrizadas.
- Validación de datos en frontend y backend.
- Tokens temporales de recuperación.
- Limitación de solicitudes de recuperación.
- Variables de entorno para información confidencial.
- Eliminación lógica de registros.

Las claves criptográficas, contraseñas y tokens
activos no deben publicarse en el repositorio.

## Auditoría y registros

Serilog permite registrar eventos y solicitudes
generadas durante la ejecución del sistema.

Los archivos de registro se almacenan dentro del
contenedor backend en:

`/app/logs`

Para consultar los archivos disponibles:

```bash
docker compose exec backend ls -lh /app/logs
```

Los registros cuentan con rotación diaria y persistencia
mediante un volumen de Docker.

## Documentación de la API

La documentación interactiva de la API REST se
encuentra disponible mediante Swagger:

http://localhost:5000/swagger/index.html

El directorio `Postman/` contiene la especificación
OpenAPI exportada del backend.

### Uso de Postman

1. Importar el archivo `NetCash.openapi.json`.
2. Configurar la variable `baseUrl` con:

   `http://localhost:5000`

3. Ejecutar el endpoint de inicio de sesión:

   `POST /api/autenticacion/login`

4. Obtener el token generado.
5. Configurar Bearer Token para los endpoints protegidos.

La API incluye endpoints relacionados con:

- Autenticación.
- Recuperación de contraseña.
- Usuarios.
- Clientes.
- Cajas.
- Turnos.
- Servicios.
- Equipos.
- Contratos.
- Pagos.
- Formas de pago.
- Dashboard.
- Menús.
- Importaciones.

Las contraseñas reales y los tokens activos no deben
incluirse en archivos de pruebas compartidos.

## Pruebas funcionales

Durante el desarrollo se realizaron pruebas manuales
de las funcionalidades principales:

- Inicio de sesión.
- Autorización mediante roles.
- Registro y aprobación de usuarios.
- Registro y consulta de clientes.
- Importación masiva de usuarios y clientes.
- Administración de cajas.
- Asignación de cajeros.
- Gestión y atención de turnos.
- Contratación de servicios.
- Registro de pagos.
- Recuperación de contraseña.
- Envío de correos mediante Mailpit.
- Indicadores del dashboard.
- Ejecución de funciones SQL.
- Migraciones automáticas.
- Comunicación entre servicios Docker.
- Consumo de endpoints mediante Swagger y Postman.

## Consideraciones y limitaciones

El proyecto fue desarrollado como una prueba técnica
en un entorno local de demostración.

Como oportunidades de mejora para una futura
implementación productiva se consideran:

- Configuración de un proveedor SMTP real.
- Implementación de HTTPS y certificados de seguridad.
- Ampliación de pruebas unitarias y de integración.
- Automatización de procesos CI/CD.
- Monitoreo centralizado de aplicaciones.
- Fortalecimiento de la revocación de tokens y sesiones.
- Revisión de seguridad previa al despliegue productivo.
- Optimización de consultas para grandes volúmenes de datos.

La importación masiva se implementó mediante archivos CSV.

El uso de hashing para las contraseñas responde a
una decisión de seguridad orientada a evitar el
almacenamiento de contraseñas recuperables.

## Detener los servicios

```bash
docker compose down
```

Los volúmenes permiten conservar la información
de PostgreSQL y los archivos de registro.

Evitar eliminar los volúmenes cuando se requiera
conservar los datos existentes.

## Uso de inteligencia artificial

Durante el desarrollo se utilizó ChatGPT como
herramienta de apoyo y orientación técnica para:

- Estructuración de la arquitectura Onion.
- Organización de componentes del sistema.
- Diseño de entidades y relaciones.
- Desarrollo e integración de funcionalidades.
- Identificación y corrección de errores.
- Configuración de Docker.
- Orientación sobre mecanismos de seguridad.
- Diseño de interfaces.
- Elaboración de documentación técnica.

El código generado o sugerido fue incorporado
progresivamente al proyecto, realizando revisiones,
ajustes y pruebas manuales durante su implementación.

La documentación relacionada con los prompts utilizados
se encuentra en el directorio:

`AI/`

## Documentación complementaria

- [Documentación del Backend](Backend/README.md)
- [Documentación del Frontend](Frontend/README.md)
- [Scripts de base de datos](Database/)
- [Documentación de API](Postman/)
- [Documentación de inteligencia artificial](AI/)

## Autor

**Edwin Leonel Belduma Belduma**

Ingeniero de Software

GitHub: https://github.com/EdwinBelduma

Repositorio: https://github.com/EdwinBelduma/netcash-viamatica

---

**NetCash — Prueba técnica VIAMATICA | 2026**