
import { inject } from '@angular/core';

import {
    CanActivateFn,
    Router
} from '@angular/router';

import { AuthService } from '../services/auth.service';

export const rolGuard: CanActivateFn = (route) => {

    const auth = inject(AuthService);
    const router = inject(Router);

    const usuario = auth.obtenerUsuario();

    const roles = route.data['roles'] as string[] ?? [];

    if (!auth.estaAutenticado()) {

        return router.createUrlTree(['/login']);
    }

    if (
        usuario &&
        roles.includes(usuario.rol)
    ) {
        return true;
    }

    return router.createUrlTree(['/dashboard']);
};
