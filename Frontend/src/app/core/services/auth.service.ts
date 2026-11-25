
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';

import { environment } from '../../../environments/environment';

import {
    LoginSolicitud,
    LoginRespuesta,
    Usuario
} from '../models/auth.model';

@Injectable({
    providedIn: 'root'
})
export class AuthService {

    private http = inject(HttpClient);

    private apiUrl =
        `${environment.apiUrl}/autenticacion`;

    login(solicitud: LoginSolicitud) {
        return this.http.post<LoginRespuesta>(
            `${this.apiUrl}/login`,
            solicitud
        ).pipe(
            tap(respuesta => {
                sessionStorage.setItem(
                    'netcash_token',
                    respuesta.token
                );

                sessionStorage.setItem(
                    'netcash_usuario',
                    JSON.stringify(respuesta.usuario)
                );

                sessionStorage.setItem(
                    'netcash_expiracion',
                    respuesta.fechaExpiracion
                );
            })
        );
    }

    obtenerToken(): string | null {
        return sessionStorage.getItem(
            'netcash_token'
        );
    }

    obtenerUsuario(): Usuario | null {
        const datos = sessionStorage.getItem(
            'netcash_usuario'
        );

        return datos ? JSON.parse(datos) : null;
    }

    estaAutenticado(): boolean {
        const token = this.obtenerToken();

        const expiracion = sessionStorage.getItem(
            'netcash_expiracion'
        );

        return !!token &&
            !!expiracion &&
            Date.parse(expiracion) > Date.now();
    }

    cerrarSesion(): void {
        sessionStorage.removeItem('netcash_token');
        sessionStorage.removeItem('netcash_usuario');
        sessionStorage.removeItem('netcash_expiracion');
    }

    obtenerPerfil() {
        return this.http.get<Usuario>(
            `${this.apiUrl}/perfil`
        );
    }
}
