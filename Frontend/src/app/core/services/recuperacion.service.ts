import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

export interface RecuperarContrasenaSolicitud {
    correo: string;
}

export interface RestablecerContrasenaSolicitud {
    token: string;
    nuevaContrasena: string;
    confirmarContrasena: string;
}

export interface RecuperacionRespuesta {
    mensaje: string;
}

@Injectable({
    providedIn: 'root'
})
export class RecuperacionService {

    private readonly http = inject(HttpClient);

    private readonly apiUrl =
        `${environment.apiUrl}/recuperacion`;

    // ==========================================
    // SOLICITAR RECUPERACION DE CONTRASENA
    // ==========================================

    solicitar(
        solicitud: RecuperarContrasenaSolicitud
    ): Observable<RecuperacionRespuesta> {

        return this.http.post<RecuperacionRespuesta>(
            `${this.apiUrl}/solicitar`,
            solicitud
        );
    }

    // ==========================================
    // RESTABLECER CONTRASENA
    // ==========================================

    restablecer(
        solicitud: RestablecerContrasenaSolicitud
    ): Observable<RecuperacionRespuesta> {

        return this.http.post<RecuperacionRespuesta>(
            `${this.apiUrl}/restablecer`,
            solicitud
        );
    }
}