import {
    Component,
    inject,
    signal
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
    AbstractControl,
    FormBuilder,
    ReactiveFormsModule,
    ValidationErrors,
    Validators
} from '@angular/forms';

import {
    ActivatedRoute,
    RouterLink
} from '@angular/router';

import { finalize } from 'rxjs';

import {
    RecuperacionService
} from '../../../core/services/recuperacion.service';

@Component({
    selector: 'app-restablecer-contrasena',
    standalone: true,
    imports: [
        CommonModule,
        ReactiveFormsModule,
        RouterLink
    ],
    templateUrl: './restablecer-contrasena.html',
    styleUrl: './restablecer-contrasena.scss'
})
export class RestablecerContrasena {

    private readonly fb = inject(FormBuilder);

    private readonly ruta = inject(ActivatedRoute);

    private readonly recuperacionService =
        inject(RecuperacionService);

    private readonly token =
        this.ruta.snapshot.queryParamMap.get('token') ?? '';

    readonly tokenValido =
        /^[A-Fa-f0-9]{64}$/.test(this.token);

    readonly cargando = signal(false);

    readonly completado = signal(false);

    readonly mensaje = signal('');

    readonly error = signal('');

    readonly mostrarContrasena = signal(false);

    readonly mostrarConfirmacion = signal(false);

    readonly formulario = this.fb.nonNullable.group({

        nuevaContrasena: [
            '',
            [
                Validators.required,
                Validators.minLength(8),
                Validators.maxLength(30),
                Validators.pattern(
                    /^(?=.*[A-Z])(?=.*[0-9]).{8,30}$/
                )
            ]
        ],

        confirmarContrasena: [
            '',
            [
                Validators.required
            ]
        ]

    }, {
        validators: this.validarCoincidencia
    });

    // ==========================================
    // VALIDAR COINCIDENCIA DE CONTRASENAS
    // ==========================================

    private validarCoincidencia(
        control: AbstractControl
    ): ValidationErrors | null {

        const contrasena =
            control.get('nuevaContrasena')?.value;

        const confirmacion =
            control.get('confirmarContrasena')?.value;

        return contrasena === confirmacion
            ? null
            : { contrasenasNoCoinciden: true };
    }

    // ==========================================
    // MOSTRAR U OCULTAR CONTRASENAS
    // ==========================================

    alternarContrasena(): void {

        this.mostrarContrasena.update(
            valor => !valor
        );
    }

    alternarConfirmacion(): void {

        this.mostrarConfirmacion.update(
            valor => !valor
        );
    }

    // ==========================================
    // RESTABLECER CONTRASENA
    // ==========================================

    restablecer(): void {

        if (
            !this.tokenValido ||
            this.formulario.invalid ||
            this.cargando() ||
            this.completado()
        ) {

            this.formulario.markAllAsTouched();

            return;
        }

        this.error.set('');
        this.mensaje.set('');
        this.cargando.set(true);

        const datos = this.formulario.getRawValue();

        this.recuperacionService
            .restablecer({

                token: this.token,

                nuevaContrasena:
                    datos.nuevaContrasena,

                confirmarContrasena:
                    datos.confirmarContrasena

            })
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

                    this.completado.set(true);

                    this.formulario.reset();
                },

                error: respuesta => {

                    this.error.set(

                        respuesta.error?.mensaje ??

                        'No fue posible restablecer la contraseña. Solicita un nuevo enlace si ha expirado.'

                    );
                }

            });
    }
}