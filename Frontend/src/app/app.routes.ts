import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { rolGuard } from './core/guards/rol.guard';

const rolesOperativos = [
    'Administrador',
    'Gestor',
    'Cajero'
];

const mantenimiento = () =>
    import('./shared/components/mantenimiento/mantenimiento')
        .then(m => m.Mantenimiento);

const operaciones = () =>
    import('./features/operaciones/operaciones')
        .then(m => m.Operaciones);

export const routes: Routes = [

    // ==========================================
    // RUTAS PUBLICAS - AUTENTICACION
    // ==========================================

    {
        path: 'login',
        loadComponent: () =>
            import('./features/auth/login/login')
                .then(m => m.Login)
    },

    // ==========================================
    // RECUPERACION DE CONTRASENA
    // ==========================================

    {
        path: 'recuperar-contrasena',
        loadComponent: () =>
            import(
                './features/auth/recuperar-contrasena/recuperar-contrasena'
            ).then(m => m.RecuperarContrasena)
    },

    {
        path: 'restablecer-contrasena',
        loadComponent: () =>
            import(
                './features/auth/restablecer-contrasena/restablecer-contrasena'
            ).then(m => m.RestablecerContrasena)
    },

    // ==========================================
    // RUTAS PRIVADAS - SISTEMA NETCASH
    // ==========================================

    {
        path: '',
        canActivate: [authGuard],
        canActivateChild: [authGuard],

        loadComponent: () =>
            import('./layout/main-layout/main-layout')
                .then(m => m.MainLayout),

        children: [

            {
                path: '',
                redirectTo: 'dashboard',
                pathMatch: 'full'
            },

            // ==================================
            // DASHBOARD
            // ==================================

            {
                path: 'dashboard',
                loadComponent: () =>
                    import('./features/dashboard/dashboard/dashboard')
                        .then(m => m.Dashboard)
            },

            // ==================================
            // GESTION DE USUARIOS
            // ==================================

            {
                path: 'usuarios',
                canActivate: [rolGuard],
                data: {
                    modulo: 'usuarios',
                    titulo: 'Gestión de usuarios',
                    roles: ['Administrador']
                },
                loadComponent: mantenimiento
            },

            // ==================================
            // GESTION DE CLIENTES
            // ==================================

            {
                path: 'clientes',
                canActivate: [rolGuard],
                data: {
                    modulo: 'clientes',
                    titulo: 'Gestión de clientes',
                    roles: ['Administrador', 'Cajero']
                },
                loadComponent: mantenimiento
            },

            // ==================================
            // OPERACIONES DE CAJAS
            // ==================================

            {
                path: 'cajas/operaciones',
                canActivate: [rolGuard],
                data: {
                    modulo: 'cajas',
                    titulo: 'Operaciones de cajas',
                    roles: rolesOperativos
                },
                loadComponent: operaciones
            },

            // ==================================
            // GESTION DE CAJAS
            // ==================================

            {
                path: 'cajas',
                canActivate: [rolGuard],
                data: {
                    modulo: 'cajas',
                    titulo: 'Gestión de cajas',
                    roles: rolesOperativos
                },
                loadComponent: mantenimiento
            },

            // ==================================
            // GESTION DE SERVICIOS
            // ==================================

            {
                path: 'servicios',
                canActivate: [rolGuard],
                data: {
                    modulo: 'servicios',
                    titulo: 'Gestión de servicios',
                    roles: rolesOperativos
                },
                loadComponent: mantenimiento
            },

            // ==================================
            // GESTION DE EQUIPOS
            // ==================================

            {
                path: 'equipos',
                canActivate: [rolGuard],
                data: {
                    modulo: 'equipos',
                    titulo: 'Gestión de equipos',
                    roles: rolesOperativos
                },
                loadComponent: mantenimiento
            },

            // ==================================
            // GESTION DE TURNOS
            // ==================================

            {
                path: 'turnos',
                canActivate: [rolGuard],
                data: {
                    modulo: 'turnos',
                    titulo: 'Gestión de turnos',
                    roles: rolesOperativos
                },
                loadComponent: operaciones
            },

            // ==================================
            // GESTION DE CONTRATOS
            // ==================================

            {
                path: 'contratos',
                canActivate: [rolGuard],
                data: {
                    modulo: 'contratos',
                    titulo: 'Gestión de contratos',
                    roles: rolesOperativos
                },
                loadComponent: operaciones
            },

            // ==================================
            // GESTION DE PAGOS
            // ==================================

            {
                path: 'pagos',
                canActivate: [rolGuard],
                data: {
                    modulo: 'pagos',
                    titulo: 'Gestión de pagos',
                    roles: rolesOperativos
                },
                loadComponent: operaciones
            }

        ]
    },

    // ==========================================
    // RUTA NO ENCONTRADA
    // ==========================================

    {
        path: '**',
        redirectTo: 'dashboard'
    }

];