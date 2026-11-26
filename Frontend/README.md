
# NetCash Frontend

Aplicación web desarrollada con Angular 21
para la administración de cajas, usuarios,
clientes y operaciones comerciales.

## Tecnologías

- Angular 21.
- TypeScript.
- Bootstrap 5.
- Bootstrap Icons.
- SCSS.
- RxJS.
- Angular Router.

## Arquitectura

    src/
    |-- app/
    |   |-- core/
    |   |   |-- guards/
    |   |   |-- interceptors/
    |   |   |-- models/
    |   |   |-- services/
    |   |-- features/
    |   |-- layout/
    |   |-- shared/
    |-- environments/
    |-- styles.scss

## Características

- Diseño responsive.
- Inicio de sesión.
- Dashboard con indicadores.
- Menú dinámico basado en roles.
- Formularios con validaciones.
- Componentes reutilizables.
- Interceptor de autenticación.
- Protección de rutas.
- Lazy loading.
- Indicadores de carga.
- Mensajes de éxito y error.
- Integración con API REST.

## Requisitos

- Node.js 24.
- npm.
- Angular CLI.

## Instalación

    npm ci

## Ejecución local

    ng serve --port 4200 --proxy-config proxy.conf.json

Acceso:

http://localhost:4200

El backend debe estar disponible en el puerto 5000
para utilizar el proxy de desarrollo.

## Compilación

    npm run build

## Ejecución con Docker

Desde la raíz del repositorio:

    docker compose up -d --build

El frontend se publica mediante Nginx.

Nginx redirige las solicitudes /api hacia
el contenedor del backend.

## Módulos

- Login.
- Dashboard.
- Usuarios.
- Clientes.
- Cajas.
- Turnos.
- Servicios.
- Equipos.
- Contratos.
- Pagos.

## Seguridad

El frontend utiliza guards para proteger las rutas
e interceptores para enviar los tokens de
autenticación a la API.

La autorización definitiva se realiza en el backend.

## Autor

Edwin Leonel Belduma Belduma

Prueba técnica - VIAMATICA | 2026
