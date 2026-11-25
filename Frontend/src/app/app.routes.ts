
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

    {
        path: 'login',
        loadComponent: () =>
            import('./features/auth/login/login')
                .then(m => m.Login)
    },

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

            {
                path: 'dashboard',
                loadComponent: () =>
                    import('./features/dashboard/dashboard/dashboard')
                        .then(m => m.Dashboard)
            },

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

    {
        path: '**',
        redirectTo: 'dashboard'
    }

];
