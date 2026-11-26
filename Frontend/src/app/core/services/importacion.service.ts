import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

import {
    ImportacionRespuesta,
    TipoImportacion
} from '../models/importacion.model';

@Injectable({
    providedIn: 'root'
})
export class ImportacionService {

    private readonly http = inject(HttpClient);

    private readonly apiUrl =
        `${environment.apiUrl}/importaciones`;

    // ==========================================
    // IMPORTAR ARCHIVOS CSV
    // ==========================================

    importar(
        tipo: TipoImportacion,
        archivo: File
    ): Observable<ImportacionRespuesta> {

        const formulario = new FormData();

        formulario.append('archivo', archivo);

        return this.http.post<ImportacionRespuesta>(
            `${this.apiUrl}/${tipo}`,
            formulario
        );
    }

    // ==========================================
    // DESCARGAR PLANTILLA CON REGISTROS DE EJEMPLO
    // ==========================================

    descargarPlantilla(tipo: TipoImportacion): void {

        const filas = tipo === 'usuarios'
            ? [
                [
                    'NombreUsuario',
                    'Nombres',
                    'Apellidos',
                    'Identificacion',
                    'Correo',
                    'Contrasena',
                    'RolId'
                ],
                [
                    'gestor005',
                    'Andres',
                    'Mendoza',
                    '0700000101',
                    'andres@example.com',
                    'Andres2026@',
                    '2'
                ],
                [
                    'gestor006',
                    'Daniela',
                    'Torres',
                    '0700000102',
                    'daniela@example.com',
                    'Daniela2026@',
                    '2'
                ],
                [
                    'cajero007',
                    'Miguel',
                    'Vargas',
                    '0700000103',
                    'miguel@example.com',
                    'Miguel2026@',
                    '3'
                ],
                [
                    'cajero008',
                    'Valeria',
                    'Castro',
                    '0700000104',
                    'valeria@example.com',
                    'Valeria2026@',
                    '3'
                ],
                [
                    'cajero009',
                    'David',
                    'Romero',
                    '0700000105',
                    'david@example.com',
                    'David2026@',
                    '3'
                ]
            ]
            : [
                [
                    'Nombres',
                    'Apellidos',
                    'Identificacion',
                    'Correo',
                    'Telefono',
                    'Direccion',
                    'ReferenciaDireccion'
                ],
                [
                    'Carlos',
                    'Fernandez',
                    '0700000201',
                    'carlos@example.com',
                    '0990000201',
                    'Avenida principal y calle diez',
                    'Frente al parque central de la ciudad'
                ],
                [
                    'Maria',
                    'Jimenez',
                    '0700000202',
                    'maria@example.com',
                    '0990000202',
                    'Barrio central calle secundaria',
                    'Junto al establecimiento comercial'
                ],
                [
                    'Jose',
                    'Morales',
                    '0700000203',
                    'jose@example.com',
                    '0990000203',
                    'Ciudadela Las Palmeras calle principal',
                    'Frente a la cancha deportiva del sector'
                ],
                [
                    'Gabriela',
                    'Ramirez',
                    '0700000204',
                    'gabriela@example.com',
                    '0990000204',
                    'Avenida de las Americas sector norte',
                    'Junto a la entrada principal del barrio'
                ],
                [
                    'Luis',
                    'Herrera',
                    '0700000205',
                    'luis@example.com',
                    '0990000205',
                    'Ciudadela El Bosque calle primera',
                    'Frente al centro comunitario del sector'
                ]
            ];

        const contenidoCSV = filas
            .map(fila => fila.join(';'))
            .join('\r\n');

        const contenido = new Blob(
            ['\uFEFF' + contenidoCSV],
            {
                type: 'text/csv;charset=utf-8;'
            }
        );

        const url = URL.createObjectURL(contenido);

        const enlace = document.createElement('a');

        enlace.href = url;

        enlace.download = `plantilla_${tipo}.csv`;

        document.body.appendChild(enlace);

        enlace.click();

        enlace.remove();

        URL.revokeObjectURL(url);
    }
}