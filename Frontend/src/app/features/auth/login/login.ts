import { Component, inject, signal } from '@angular/core';

import {
    ReactiveFormsModule,
    FormBuilder,
    Validators
} from '@angular/forms';

import {
    Router,
    RouterLink
} from '@angular/router';

import {
    AuthService
} from '../../../core/services/auth.service';

@Component({
    selector: 'app-login',
    standalone: true,
    imports: [
        ReactiveFormsModule,
        RouterLink
    ],
    templateUrl: './login.html',
    styleUrl: './login.scss'
})
export class Login {

    private fb = inject(FormBuilder);

    private authService = inject(AuthService);

    private router = inject(Router);

    cargando = signal(false);

    mostrarContrasena = signal(false);

    mensajeError = signal('');

    // ==========================================
    // FORMULARIO DE INICIO DE SESION
    // ==========================================

    formulario = this.fb.nonNullable.group({

        nombreUsuario: [
            '',
            [
                Validators.required,
                Validators.minLength(8),
                Validators.maxLength(20)
            ]
        ],

        contrasena: [
            '',
            [
                Validators.required,
                Validators.minLength(8),
                Validators.maxLength(30)
            ]
        ]

    });

    // ==========================================
    // MOSTRAR U OCULTAR CONTRASENA
    // ==========================================

    alternarContrasena(): void {

        this.mostrarContrasena.update(
            valor => !valor
        );
    }

    // ==========================================
    // INICIAR SESION
    // ==========================================

    iniciarSesion(): void {

        this.mensajeError.set('');

        if (this.formulario.invalid) {

            this.formulario.markAllAsTouched();

            return;
        }

        if (this.cargando()) {

            return;
        }

        this.cargando.set(true);

        this.authService.login(
            this.formulario.getRawValue()
        ).subscribe({

            next: () => {

                this.cargando.set(false);

                this.router.navigate(['/dashboard']);
            },

            error: (error) => {

                this.cargando.set(false);

                if (error.status === 0) {

                    this.mensajeError.set(
                        'No se pudo conectar con el servidor.'
                    );

                } else if (error.status === 401) {

                    this.mensajeError.set(
                        'Usuario o contraseña incorrectos.'
                    );

                } else {

                    this.mensajeError.set(
                        'No fue posible iniciar sesión.'
                    );
                }
            }

        });
    }
}