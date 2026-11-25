import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import { environment } from '../../../environments/environment';

import {
    DashboardRespuesta,
    Menu
} from '../models/dashboard.model';

@Injectable({
    providedIn: 'root'
})
export class DashboardService {

    private http = inject(HttpClient);

    obtenerResumen(desde: string, hasta: string) {

        const params = new HttpParams()
            .set('desde', desde)
            .set('hasta', hasta);

        return this.http.get<DashboardRespuesta>(
            `${environment.apiUrl}/dashboard/resumen`,
            { params }
        );
    }

    obtenerMenu() {
        return this.http.get<Menu[]>(
            `${environment.apiUrl}/menus/mi-menu`
        );
    }
}