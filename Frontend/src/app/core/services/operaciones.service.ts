
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class OperacionesService {

    private http = inject(HttpClient);
    private api = environment.apiUrl;

    listar(modulo: string) {
        return this.http.get<Record<string, any>[]>(
            `${this.api}/${modulo}`
        );
    }

    ejecutar(
        metodo: string,
        ruta: string,
        datos: object = {}
    ) {
        return this.http.request(
            metodo,
            `${this.api}/${ruta}`,
            { body: metodo === 'DELETE' ? undefined : datos }
        );
    }
}
