
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '../../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class MantenimientoService {

    private http = inject(HttpClient);

    private api = environment.apiUrl;

    listar(modulo: string) {

        return this.http.get<Record<string, any>[]>(
            `${this.api}/${modulo}`
        );
    }

    obtener(modulo: string, id: number) {

        return this.http.get<Record<string, any>>(
            `${this.api}/${modulo}/${id}`
        );
    }

    crear(modulo: string, datos: object) {

        return this.http.post(
            `${this.api}/${modulo}`,
            datos
        );
    }

    actualizar(
        modulo: string,
        id: number,
        datos: object
    ) {

        return this.http.put(
            `${this.api}/${modulo}/${id}`,
            datos
        );
    }

    eliminar(modulo: string, id: number) {

        return this.http.delete(
            `${this.api}/${modulo}/${id}`
        );
    }

    aprobarUsuario(id: number) {

        return this.http.patch(
            `${this.api}/usuarios/${id}/aprobar`,
            {}
        );
    }

}
