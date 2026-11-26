# NetCash | Frontend

## Descripción

NetCash Frontend es una aplicación web empresarial
desarrollada con Angular 21 para la administración
de cajas, usuarios, clientes, turnos, contratos y
operaciones comerciales de servicios de telecomunicaciones.

El proyecto fue desarrollado como parte de la prueba
técnica de VIAMATICA, aplicando principios de modularidad,
seguridad, reutilización de componentes y separación
de responsabilidades.

La aplicación proporciona una interfaz moderna,
intuitiva y adaptable a diferentes dispositivos.

## Tecnologías utilizadas

- Angular 21.
- TypeScript.
- Bootstrap 5.
- Bootstrap Icons.
- SCSS.
- RxJS.
- Angular Router.
- Angular Signals.
- Reactive Forms.
- HttpClient.
- Node.js 24.
- Nginx.
- Docker.
- Docker Compose.

## Arquitectura del frontend

El frontend utiliza una arquitectura modular basada
en componentes standalone de Angular.

La organización del proyecto permite separar la
lógica de negocio, los servicios, las interfaces
de usuario y los componentes reutilizables.

## Estructura del proyecto

```text
Frontend/
├── src/
│   ├── app/
│   │   ├── core/
│   │   │   ├── guards/
│   │   │   ├── interceptors/
│   │   │   ├── models/
│   │   │   └── services/
│   │   ├── features/
│   │   │   ├── auth/
│   │   │   │   ├── login/
│   │   │   │   ├── recuperar-contrasena/
│   │   │   │   └── restablecer-contrasena/
│   │   │   ├── dashboard/
│   │   │   └── operaciones/
│   │   ├── layout/
│   │   │   └── main-layout/
│   │   ├── shared/
│   │   │   └── components/
│   │   │       ├── mantenimiento/
│   │   │       └── importacion-csv/
│   │   ├── app.routes.ts
│   │   └── app.config.ts
│   ├── environments/
│   └── styles.scss
├── angular.json
├── package.json
├── package-lock.json
├── proxy.conf.json
├── nginx.conf
├── Dockerfile
└── README.md
```

## Organización de componentes

### Core

Contiene los elementos principales utilizados
de manera transversal en la aplicación:

- Servicios de autenticación.
- Servicios de recuperación de contraseña.
- Servicios de mantenimiento.
- Servicios de operaciones comerciales.
- Servicios del dashboard.
- Servicios de importación masiva.
- Modelos e interfaces TypeScript.
- Guards de autenticación y autorización.
- Interceptores HTTP.

### Features

Contiene las funcionalidades principales
del sistema:

- Autenticación.
- Recuperación de contraseña.
- Restablecimiento de contraseña.
- Dashboard.
- Operaciones administrativas y comerciales.

### Layout

Contiene la estructura visual principal:

- Barra lateral de navegación.
- Menú dinámico.
- Información del usuario autenticado.
- Área principal de contenido.
- Navegación entre módulos.
- Cierre de sesión.

### Shared

Contiene componentes reutilizables para
diferentes funcionalidades:

- Formularios de mantenimiento.
- Listados de registros.
- Importación masiva CSV.
- Mensajes de confirmación.
- Indicadores de carga.
- Mensajes de éxito y error.

## Características principales

- Diseño responsive.
- Componentes standalone.
- Arquitectura modular.
- Inicio de sesión.
- Recuperación de contraseña.
- Restablecimiento de credenciales.
- Dashboard con indicadores.
- Menú dinámico basado en roles.
- Formularios reactivos.
- Validación de campos.
- Componentes reutilizables.
- Interceptor de autenticación.
- Protección de rutas.
- Lazy Loading.
- Manejo de estados mediante Signals.
- Indicadores de carga.
- Mensajes de éxito y error.
- Importación masiva mediante CSV.
- Descarga de plantillas.
- Integración con API REST.
- Comunicación mediante Docker y Nginx.

## Módulos del sistema

### Autenticación

Permite el acceso al sistema mediante
nombre de usuario y contraseña.

Incluye:

- Validación de credenciales.
- Visualización y ocultamiento de contraseña.
- Manejo de errores de autenticación.
- Almacenamiento del token durante la sesión.
- Redirección al dashboard.
- Cierre de sesión.
- Acceso al módulo de recuperación de contraseña.

### Recuperación de contraseña

Permite solicitar el restablecimiento de las
credenciales mediante el correo electrónico registrado.

El formulario incluye:

- Validación del correo electrónico.
- Indicadores de procesamiento.
- Mensajes de confirmación.
- Manejo de errores.
- Navegación hacia el inicio de sesión.

### Restablecimiento de contraseña

Permite establecer una nueva contraseña utilizando
el enlace generado por el backend.

Incluye:

- Validación del formato del token.
- Ingreso de nueva contraseña.
- Confirmación de contraseña.
- Visualización y ocultamiento de credenciales.
- Validación de longitud.
- Validación de letras mayúsculas y números.
- Confirmación de actualización.
- Retorno al inicio de sesión.

La validación definitiva del token se realiza
en el backend.

### Dashboard

Presenta indicadores operativos y administrativos
según el rol del usuario autenticado.

Incluye:

- Total de usuarios.
- Total de clientes.
- Cajas disponibles y ocupadas.
- Turnos pendientes.
- Turnos en atención.
- Turnos atendidos.
- Contratos vigentes.
- Total de pagos.
- Total recaudado.
- Indicadores por caja.
- Indicadores por cajero.
- Indicadores por gestor.
- Filtros de fechas.
- Estado de sesiones.
- Usuarios pendientes de aprobación.

### Gestión de usuarios

Permite administrar usuarios conforme a los
permisos asignados a cada rol.

Incluye:

- Registro de usuarios.
- Consulta de registros.
- Búsqueda de usuarios.
- Actualización de información.
- Aprobación administrativa.
- Eliminación lógica.
- Importación masiva de usuarios mediante CSV.

### Gestión de clientes

Permite administrar la información de los
clientes registrados en el sistema.

Incluye:

- Registro de clientes.
- Consulta y búsqueda.
- Actualización de información.
- Validación de identificación.
- Validación de correo electrónico.
- Validación de teléfono y dirección.
- Eliminación lógica.
- Importación masiva mediante CSV.

### Gestión de cajas

Permite administrar las cajas disponibles
para las operaciones comerciales.

Incluye:

- Registro y mantenimiento de cajas.
- Consulta de disponibilidad.
- Asignación de cajeros.
- Eliminación de asignaciones.
- Apertura de cajas.
- Cierre de cajas.
- Consulta del estado operativo.

### Gestión de turnos

Permite administrar la atención de clientes
mediante turnos asociados a cajas y cajeros.

Incluye:

- Generación de turnos.
- Asignación de turnos.
- Consulta de estados.
- Inicio de atención.
- Finalización de atención mediante
  operaciones comerciales.
- Cancelación de turnos.

### Servicios y equipos

Permite administrar el catálogo de servicios
de telecomunicaciones y sus equipos asociados.

Incluye:

- Registro de servicios.
- Consulta de servicios.
- Actualización de características.
- Administración de precios.
- Registro de equipos.
- Actualización de equipos.
- Eliminación lógica.

### Contratos

Permite administrar las contrataciones
de servicios de telecomunicaciones.

Incluye:

- Registro de contratos.
- Consulta de contratos.
- Cambio de servicio.
- Cambio de forma de pago.
- Cancelación de contratos.
- Consulta del estado contractual.

### Pagos

Permite registrar y consultar las transacciones
correspondientes a los servicios contratados.

Incluye:

- Registro de pagos.
- Selección de forma de pago.
- Consulta de pagos.
- Validación de valores.
- Asociación de pagos con contratos.
- Visualización de operaciones registradas.

## Roles y navegación

El sistema dispone de tres roles operativos:

### Administrador

Acceso a funcionalidades administrativas,
gestión de usuarios, aprobaciones, cajas,
catálogos e indicadores generales.

### Gestor

Acceso a funcionalidades de coordinación
operativa, registro de usuarios sujeto a
aprobación, asignación de cajeros y turnos.

### Cajero

Acceso a funcionalidades de atención al
cliente, apertura y cierre de cajas,
contrataciones y registro de pagos.

El menú principal se obtiene dinámicamente
desde el backend según el rol autenticado.

## Rutas de la aplicación

### Rutas públicas

| Ruta | Funcionalidad |
|---|---|
| /login | Inicio de sesión |
| /recuperar-contrasena | Solicitud de recuperación |
| /restablecer-contrasena | Restablecimiento de contraseña |

### Rutas protegidas

| Ruta | Funcionalidad |
|---|---|
| /dashboard | Indicadores del sistema |
| /usuarios | Gestión de usuarios |
| /clientes | Gestión de clientes |
| /cajas | Gestión de cajas |
| /cajas/operaciones | Operaciones de cajas |
| /turnos | Gestión de turnos |
| /servicios | Catálogo de servicios |
| /equipos | Gestión de equipos |
| /contratos | Gestión de contratos |
| /pagos | Registro y consulta de pagos |

Las rutas protegidas requieren autenticación.

La disponibilidad de cada módulo depende de
los permisos correspondientes al rol del usuario.

## Importación masiva CSV

El frontend incorpora componentes reutilizables
para la importación de usuarios y clientes.

El proceso permite:

1. Descargar una plantilla CSV.
2. Completar los datos correspondientes.
3. Seleccionar el archivo.
4. Enviar el archivo al backend.
5. Visualizar los resultados.
6. Consultar los registros rechazados.
7. Actualizar el listado después de la importación.

El backend realiza la validación definitiva
de los registros y las reglas de negocio.

Las plantillas contienen datos de ejemplo
destinados exclusivamente a pruebas.

## Integración con la API REST

El frontend utiliza HttpClient para consumir
los servicios proporcionados por ASP.NET Core.

La configuración de acceso a la API se
encuentra en:

```text
src/environments/environment.ts
```

La URL relativa utilizada es:

```typescript
apiUrl: '/api'
```

Durante el desarrollo local, las solicitudes
se redirigen mediante el proxy de Angular.

Dentro de Docker, Nginx redirige las solicitudes
hacia el contenedor del backend.

## Seguridad

El frontend incorpora los siguientes
mecanismos de seguridad:

- Guards de autenticación.
- Guards de autorización por roles.
- Interceptor HTTP de autenticación.
- Envío de tokens mediante Bearer Token.
- Protección de rutas privadas.
- Validación de formularios.
- Manejo de errores de autenticación.
- Control de vigencia de sesión.
- Navegación según permisos.
- Recuperación de contraseña mediante
  tokens temporales.

La autorización definitiva y la validación
de las reglas de negocio se realizan
exclusivamente en el backend.

No deben almacenarse claves criptográficas
ni contraseñas privadas en el código fuente
del frontend.

## Requisitos para ejecución local

- Node.js 24.
- npm.
- Angular CLI.
- Backend de NetCash disponible.
- PostgreSQL configurado.
- Navegador web actualizado.

## Instalación de dependencias

Desde el directorio Frontend:

```bash
npm ci
```

Este comando instala las dependencias definidas
en el archivo package-lock.json.

## Ejecución local

Desde el directorio Frontend:

```bash
npx ng serve --port 4200 --proxy-config proxy.conf.json
```

Acceso a la aplicación:

http://localhost:4200

El backend debe encontrarse disponible
en el puerto 5000 para utilizar el proxy
de desarrollo.

## Compilación

Para generar la compilación del frontend:

```bash
npm run build
```

Los archivos compilados se generan en
el directorio dist.

La compilación se utiliza para publicar
la aplicación mediante Nginx dentro de Docker.

## Ejecución mediante Docker

La ejecución recomendada para evaluar
el proyecto es mediante Docker Compose.

Desde la raíz del repositorio:

```bash
docker compose up -d --build
```

Verificar los contenedores:

```bash
docker compose ps
```

Reconstruir únicamente el frontend:

```bash
docker compose up -d --build frontend
```

Consultar los registros:

```bash
docker compose logs --tail=60 frontend
```

## Configuración de Nginx

El frontend utiliza Nginx para publicar
los archivos compilados de Angular.

La configuración se encuentra en:

```text
Frontend/nginx.conf
```

Nginx cumple las siguientes funciones:

- Publicación de archivos estáticos.
- Soporte para navegación SPA.
- Redirección de solicitudes hacia la API.
- Comunicación con el backend mediante
  la red interna de Docker.

Las solicitudes dirigidas a /api son
redireccionadas al servicio backend
en el puerto interno 8080.

## Acceso a los servicios

| Servicio | Dirección |
|---|---|
| Frontend | http://localhost:4200 |
| Backend | http://localhost:5000 |
| Swagger | http://localhost:5000/swagger/index.html |
| Mailpit | http://localhost:8025 |

## Usuario inicial

Para acceder al sistema por primera vez:

- Usuario: adminnet01.
- Contraseña: definida en ADMIN_CONTRASENA.

La contraseña se configura mediante el
archivo .env ubicado en la raíz del proyecto.

No se incluyen credenciales privadas
dentro del código del frontend.

## Prueba de recuperación de contraseña

Para comprobar esta funcionalidad:

1. Acceder a la pantalla de inicio de sesión.
2. Seleccionar "¿Olvidaste tu contraseña?".
3. Ingresar el correo de un usuario activo.
4. Enviar la solicitud de recuperación.
5. Acceder a Mailpit en el puerto 8025.
6. Abrir el correo recibido.
7. Seleccionar el enlace de recuperación.
8. Ingresar y confirmar la nueva contraseña.
9. Regresar al inicio de sesión.
10. Iniciar sesión con las nuevas credenciales.

Mailpit se utiliza únicamente como servidor
SMTP de pruebas y no envía correos a
destinatarios externos.

## Diseño de interfaz

La interfaz utiliza Bootstrap 5, Bootstrap
Icons y estilos personalizados mediante SCSS.

El diseño mantiene una identidad visual
corporativa basada en:

- Tonalidades azul y azul oscuro.
- Formularios de acceso modernos.
- Tarjetas de indicadores.
- Menús laterales.
- Tablas de información.
- Iconografía consistente.
- Mensajes de retroalimentación.
- Adaptabilidad a diferentes pantallas.

## Pruebas funcionales

Durante el desarrollo se realizaron pruebas
manuales de las principales funcionalidades:

- Inicio de sesión.
- Navegación entre módulos.
- Protección de rutas.
- Menú dinámico.
- Formularios y validaciones.
- Gestión de usuarios.
- Gestión de clientes.
- Importación masiva CSV.
- Gestión de cajas.
- Asignación de cajeros.
- Atención de turnos.
- Contratación de servicios.
- Registro de pagos.
- Recuperación de contraseña.
- Visualización del dashboard.
- Comunicación con el backend.
- Ejecución mediante Docker.

## Consideraciones y limitaciones

El frontend fue desarrollado para un entorno
local de evaluación técnica.

Como oportunidades de mejora se consideran:

- Ampliación de pruebas unitarias automatizadas.
- Pruebas automatizadas de extremo a extremo.
- Optimización de rendimiento.
- Ampliación de pruebas de accesibilidad.
- Monitoreo de errores del frontend.
- Configuración HTTPS para producción.
- Fortalecimiento de la gestión de sesiones.
- Mejoras en la experiencia de usuario.

La importación masiva se implementó
mediante archivos CSV.

## Uso de inteligencia artificial

Durante el desarrollo se utilizó de apoyo para:
## Uso de inteligencia artificial

Durante el desarrollo del proyecto se utilizó ChatGPT
como herramienta de apoyo técnico para:

- Orientación en la arquitectura y estructura del software.
- Sugerencias de implementación y buenas prácticas.
- Apoyo en la generación y revisión de código.
- Identificación, análisis y solución de errores.
- Sugerencias de optimización y seguridad.
- Orientación en la integración de tecnologías.
- Apoyo en la elaboración de documentación técnica.

Las sugerencias proporcionadas fueron revisadas,
adaptadas e incorporadas progresivamente al proyecto,
realizando pruebas funcionales para verificar su
correcto funcionamiento.

La documentación complementaria de los prompts
utilizados se encuentra en el directorio AI.

Las sugerencias fueron incorporadas
progresivamente mediante revisión,
configuración y pruebas manuales.

La documentación complementaria se
encuentra en el directorio AI.

## Documentación complementaria

- [README principal](../README.md)
- [Documentación Backend](../Backend/README.md)
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

**NetCash Frontend | Prueba técnica VIAMATICA | 2026**