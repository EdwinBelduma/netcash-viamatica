import {
  Component,
  inject,
  input,
  output,
  signal
} from '@angular/core';

import { finalize } from 'rxjs';

import {
  ImportacionRespuesta,
  TipoImportacion
} from '../../../core/models/importacion.model';

import {
  ImportacionService
} from '../../../core/services/importacion.service';

@Component({
  selector: 'app-importacion-csv',
  standalone: true,
  templateUrl: './importacion-csv.html',
  styleUrl: './importacion-csv.scss'
})
export class ImportacionCsv {

  private readonly importacionService =
    inject(ImportacionService);

  readonly tipo = input.required<TipoImportacion>();

  readonly importacionCompletada = output<void>();

  readonly archivo = signal<File | null>(null);

  readonly importando = signal(false);

  readonly mostrarPanel = signal(false);

  readonly error = signal('');

  readonly resultado =
    signal<ImportacionRespuesta | null>(null);

  abrir(): void {

    this.archivo.set(null);
    this.error.set('');
    this.resultado.set(null);
    this.mostrarPanel.set(true);
  }

  cerrar(): void {

    if (this.importando()) {
      return;
    }

    this.mostrarPanel.set(false);
    this.archivo.set(null);
    this.error.set('');
    this.resultado.set(null);
  }

  seleccionarArchivo(evento: Event): void {

    const elemento = evento.target as HTMLInputElement;

    const archivo = elemento.files?.[0];

    this.error.set('');
    this.resultado.set(null);
    this.archivo.set(null);

    if (!archivo) {
      return;
    }

    if (!archivo.name.toLowerCase().endsWith('.csv')) {

      this.error.set(
        'Solo se permiten archivos CSV.'
      );

      elemento.value = '';

      return;
    }

    if (archivo.size === 0) {

      this.error.set('El archivo está vacío.');

      elemento.value = '';

      return;
    }

    if (archivo.size > 5 * 1024 * 1024) {

      this.error.set(
        'El archivo no puede superar los 5 MB.'
      );

      elemento.value = '';

      return;
    }

    this.archivo.set(archivo);
  }

  descargarPlantilla(): void {

    this.importacionService.descargarPlantilla(
      this.tipo()
    );
  }

  importar(): void {

    const archivo = this.archivo();

    if (!archivo || this.importando()) {
      return;
    }

    this.error.set('');
    this.resultado.set(null);
    this.importando.set(true);

    this.importacionService
      .importar(this.tipo(), archivo)
      .pipe(
        finalize(() => {
          this.importando.set(false);
        })
      )
      .subscribe({

        next: respuesta => {

          this.resultado.set(respuesta);

          this.archivo.set(null);

          if (respuesta.registrosImportados > 0) {

            this.importacionCompletada.emit();
          }
        },

        error: respuesta => {

          this.error.set(
            respuesta.error?.mensaje ??
            'No fue posible importar el archivo CSV.'
          );
        }
      });
  }
}