import {
    Component,
    inject,
    signal
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
    FormBuilder,
    ReactiveFormsModule,
    Validators
} from '@angular/forms';

import { RouterLink } from '@angular/router';

import { finalize } from 'rxjs';

import {
    RecuperacionService
} from '../../../core/services/recuperacion.service';

@Component({
    selector: 'app-recuperar-contrasena',
    standalone: true,
    imports: [
        CommonModule,
        ReactiveFormsModule,
        RouterLink
    ],
    templateUrl: './recuperar-contrasena.html',
    styleUrl: './recuperar-contrasena.scss'
})
export class RecuperarContrasena {

    private readonly fb = inject(FormBuilder);

    private readonly recuperacionService =
        inject(RecuperacionService);

    readonly cargando = signal(false);

    readonly mensaje = signal('');

    readonly error = signal('');

    readonly formulario = this.fb.nonNullable.group({

        correo: [
            '',
            [
                Validators.required,
                Validators.email,
                Validators.maxLength(120)
            ]
        ]

    });

    enviar(): void {

        if (this.formulario.invalid ||
            this.cargando()) {

            this.formulario.markAllAsTouched();

            return;
        }

        this.mensaje.set('');
        this.error.set('');
        this.cargando.set(true);

        const correo =
            this.formulario.controls.correo.value.trim();

        this.recuperacionService
            .solicitar({ correo })
            .pipe(
                finalize(() => {
                    this.cargando.set(false);
                })
            )
            .subscribe({

                next: respuesta => {

                    this.mensaje.set(
                        respuesta.mensaje
                    );

                    this.formulario.reset();
                },

                error: respuesta => {

                    this.error.set(
                        respuesta.error?.mensaje ??
                        'No fue posible procesar la solicitud. Intenta nuevamente.'
                    );
                }

            });
    }
}