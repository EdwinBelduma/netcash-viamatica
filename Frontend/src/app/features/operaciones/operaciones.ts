
import {
    Component,
    OnInit,
    inject,
    signal
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

import { AuthService } from '../../core/services/auth.service';
import { OperacionesService } from '../../core/services/operaciones.service';

interface Campo {
    nombre: string;
    etiqueta: string;
    tipo: string;
    opcional?: boolean;
}

interface Accion {
    nombre: string;
    metodo: string;
    ruta: string;
    roles: string[];
    campos: Campo[];
}

@Component({
    selector: 'app-operaciones',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './operaciones.html',
    styleUrl: './operaciones.scss'
})
export class Operaciones implements OnInit {

    private servicio = inject(OperacionesService);
    private auth = inject(AuthService);
    private ruta = inject(ActivatedRoute);

    usuario = this.auth.obtenerUsuario();

    modulo = '';
    titulo = '';

    acciones = signal<Accion[]>([]);
    seleccion = signal<Accion | null>(null);

    registros = signal<Record<string, any>[]>([]);
    columnas = signal<string[]>([]);

    cargando = signal(false);
    guardando = signal(false);

    mensaje = signal('');
    error = signal('');

    formulario: Record<string, any> = {};

    private administrativos = [
        'Administrador',
        'Gestor'
    ];

    private cajeros = ['Cajero'];

    ngOnInit(): void {

        this.ruta.data.subscribe(datos => {

            this.modulo = datos['modulo'];
            this.titulo = datos['titulo'];

            this.seleccion.set(null);
            this.mensaje.set('');
            this.error.set('');
            this.registros.set([]);

            this.configurar();
            this.cargar();
        });
    }

    campo(
        nombre: string,
        etiqueta: string,
        tipo = 'number',
        opcional = false
    ): Campo {

        return {
            nombre,
            etiqueta,
            tipo,
            opcional
        };
    }

    configurar(): void {

        let acciones: Accion[] = [];

        switch (this.modulo) {

            // ==================================
            // OPERACIONES DE CAJAS
            // ==================================

            case 'cajas':

                this.columnas.set([
                    'id',
                    'descripcion',
                    'usuarioEnSesionId',
                    'fechaApertura'
                ]);

                acciones = [

                    {
                        nombre: 'Asignar cajero',
                        metodo: 'POST',
                        ruta: 'cajas/{cajaId}/cajeros',
                        roles: this.administrativos,
                        campos: [
                            this.campo('cajaId', 'ID de caja'),
                            this.campo('usuarioId', 'ID del cajero')
                        ]
                    },

                    {
                        nombre: 'Retirar cajero',
                        metodo: 'DELETE',
                        ruta: 'cajas/{cajaId}/cajeros/{usuarioId}',
                        roles: this.administrativos,
                        campos: [
                            this.campo('cajaId', 'ID de caja'),
                            this.campo('usuarioId', 'ID del cajero')
                        ]
                    },

                    {
                        nombre: 'Abrir caja',
                        metodo: 'POST',
                        ruta: 'cajas/{cajaId}/abrir',
                        roles: this.cajeros,
                        campos: [
                            this.campo('cajaId', 'ID de caja')
                        ]
                    },

                    {
                        nombre: 'Cerrar caja',
                        metodo: 'POST',
                        ruta: 'cajas/{cajaId}/cerrar',
                        roles: this.cajeros,
                        campos: [
                            this.campo('cajaId', 'ID de caja')
                        ]
                    }
                ];

                break;

            // ==================================
            // TURNOS
            // ==================================

            case 'turnos':

                this.columnas.set([
                    'id',
                    'descripcion',
                    'cajaId',
                    'cajeroId',
                    'clienteId',
                    'tipoAtencion',
                    'estado'
                ]);

                acciones = [

                    {
                        nombre: 'Generar turno',
                        metodo: 'POST',
                        ruta: 'turnos',
                        roles: this.administrativos,
                        campos: [
                            this.campo('cajaId', 'ID de caja'),
                            this.campo('cajeroId', 'ID del cajero'),
                            this.campo(
                                'clienteId',
                                'ID del cliente (opcional)',
                                'number',
                                true
                            ),
                            this.campo(
                                'tipoAtencion',
                                'Tipo de atención',
                                'atencion'
                            )
                        ]
                    },

                    {
                        nombre: 'Iniciar atención',
                        metodo: 'PATCH',
                        ruta: 'turnos/{turnoId}/iniciar',
                        roles: this.cajeros,
                        campos: [
                            this.campo('turnoId', 'ID del turno')
                        ]
                    },

                    {
                        nombre: 'Cancelar turno',
                        metodo: 'PATCH',
                        ruta: 'turnos/{turnoId}/cancelar',
                        roles: this.administrativos,
                        campos: [
                            this.campo('turnoId', 'ID del turno')
                        ]
                    },

                    {
                        nombre: 'Eliminar turno cancelado',
                        metodo: 'DELETE',
                        ruta: 'turnos/{turnoId}',
                        roles: this.administrativos,
                        campos: [
                            this.campo('turnoId', 'ID del turno')
                        ]
                    }
                ];

                break;

            // ==================================
            // CONTRATOS
            // ==================================

            case 'contratos':

                this.columnas.set([
                    'id',
                    'clienteId',
                    'servicioId',
                    'formaPagoId',
                    'precioContratado',
                    'fechaInicio',
                    'fechaFin',
                    'estado'
                ]);

                acciones = [

                    {
                        nombre: 'Contratar servicio',
                        metodo: 'POST',
                        ruta: 'contratos',
                        roles: this.cajeros,
                        campos: [
                            this.campo('turnoId', 'ID del turno'),
                            this.campo('clienteId', 'ID del cliente'),
                            this.campo('servicioId', 'ID del servicio'),
                            this.campo(
                                'formaPagoId',
                                'Forma de pago',
                                'pago'
                            ),
                            this.campo(
                                'fechaFin',
                                'Fecha de finalización',
                                'date'
                            )
                        ]
                    },

                    {
                        nombre: 'Cambiar servicio',
                        metodo: 'PATCH',
                        ruta: 'contratos/{contratoId}/cambiar-servicio',
                        roles: this.cajeros,
                        campos: [
                            this.campo('contratoId', 'ID del contrato'),
                            this.campo('turnoId', 'ID del turno'),
                            this.campo('servicioId', 'ID del nuevo servicio')
                        ]
                    },

                    {
                        nombre: 'Cambiar forma de pago',
                        metodo: 'PATCH',
                        ruta: 'contratos/{contratoId}/cambiar-forma-pago',
                        roles: this.cajeros,
                        campos: [
                            this.campo('contratoId', 'ID del contrato'),
                            this.campo('turnoId', 'ID del turno'),
                            this.campo(
                                'formaPagoId',
                                'Nueva forma de pago',
                                'pago'
                            )
                        ]
                    },

                    {
                        nombre: 'Cancelar contrato',
                        metodo: 'PATCH',
                        ruta: 'contratos/{contratoId}/cancelar',
                        roles: this.cajeros,
                        campos: [
                            this.campo('contratoId', 'ID del contrato'),
                            this.campo('turnoId', 'ID del turno'),
                            this.campo('motivo', 'Motivo', 'text')
                        ]
                    }
                ];

                break;

            // ==================================
            // PAGOS
            // ==================================

            case 'pagos':

                this.columnas.set([
                    'id',
                    'contratoId',
                    'clienteId',
                    'cajaId',
                    'cajeroId',
                    'monto',
                    'fechaPago',
                    'referencia'
                ]);

                acciones = [

                    {
                        nombre: 'Registrar pago',
                        metodo: 'POST',
                        ruta: 'pagos',
                        roles: this.cajeros,
                        campos: [
                            this.campo('turnoId', 'ID del turno'),
                            this.campo('contratoId', 'ID del contrato'),
                            this.campo(
                                'formaPagoId',
                                'Forma de pago',
                                'pago'
                            ),
                            this.campo('monto', 'Monto ($)', 'decimal'),
                            this.campo(
                                'referencia',
                                'Referencia',
                                'text',
                                true
                            ),
                            this.campo(
                                'observacion',
                                'Observación',
                                'text',
                                true
                            )
                        ]
                    }
                ];

                break;
        }

        this.acciones.set(
            acciones.filter(accion =>
                accion.roles.includes(
                    this.usuario?.rol ?? ''
                )
            )
        );
    }

    elegirAccion(accion: Accion): void {

        this.formulario = {};
        this.mensaje.set('');
        this.error.set('');

        for (const campo of accion.campos) {
            this.formulario[campo.nombre] = null;
        }

        this.seleccion.set(accion);
    }

    cancelar(): void {

        if (this.guardando()) {
            return;
        }

        this.seleccion.set(null);
        this.formulario = {};
    }

    cargar(): void {

        const moduloActual = this.modulo;

        this.cargando.set(true);

        this.servicio.listar(moduloActual).subscribe({

            next: datos => {

                if (this.modulo !== moduloActual) {
                    return;
                }

                this.registros.set(datos);
                this.cargando.set(false);
            },

            error: error => {

                if (this.modulo !== moduloActual) {
                    return;
                }

                this.error.set(this.obtenerError(error));
                this.cargando.set(false);
            }
        });
    }

    guardar(): void {

        const accion = this.seleccion();

        if (!accion || this.guardando()) {
            return;
        }

        if (!accion.roles.includes(this.usuario?.rol ?? '')) {
            return;
        }

        this.error.set('');
        this.mensaje.set('');

        const datos = { ...this.formulario };

        for (const campo of accion.campos) {

            const valor = datos[campo.nombre];

            if (
                !campo.opcional &&
                (valor === null ||
                 valor === undefined ||
                 String(valor).trim() === '')
            ) {
                this.error.set(
                    `Completa el campo ${campo.etiqueta}.`
                );
                return;
            }

            if (
                campo.tipo === 'number' &&
                valor != null &&
                (!Number.isInteger(Number(valor)) ||
                 Number(valor) <= 0)
            ) {
                this.error.set(
                    `El campo ${campo.etiqueta} debe ser un entero positivo.`
                );
                return;
            }

            if (campo.tipo === 'decimal') {

                const monto = Number(valor);

                if (
                    !Number.isFinite(monto) ||
                    monto <= 0 ||
                    Math.abs(
                        monto * 100 - Math.round(monto * 100)
                    ) > 0.000001
                ) {
                    this.error.set(
                        'Ingresa un monto positivo de máximo dos decimales.'
                    );
                    return;
                }
            }

            if (campo.tipo === 'date' && valor) {

                datos[campo.nombre] =
                    `${valor}T23:59:59-05:00`;
            }
        }

        if (
            !confirm(`¿Confirmas la operación: ${accion.nombre}?`)
        ) {
            return;
        }

        const ruta = accion.ruta.replace(
            /\{(\w+)\}/g,
            (_, clave: string) => {

                const valor = datos[clave];
                delete datos[clave];

                return encodeURIComponent(String(valor));
            }
        );

        const moduloActual = this.modulo;

        this.guardando.set(true);

        this.servicio.ejecutar(
            accion.metodo,
            ruta,
            datos
        ).subscribe({

            next: () => {

                this.guardando.set(false);

                if (this.modulo !== moduloActual) {
                    return;
                }

                this.mensaje.set(
                    'Operación realizada correctamente.'
                );

                this.seleccion.set(null);
                this.formulario = {};

                this.cargar();
            },

            error: error => {

                this.guardando.set(false);
                this.error.set(this.obtenerError(error));
            }
        });
    }

    etiqueta(columna: string): string {

        const etiquetas: Record<string, string> = {
            id: 'ID',
            descripcion: 'Descripción',
            usuarioEnSesionId: 'Cajero en sesión',
            fechaApertura: 'Fecha de apertura',
            cajaId: 'Caja',
            cajeroId: 'Cajero',
            clienteId: 'Cliente',
            tipoAtencion: 'Tipo de atención',
            estado: 'Estado',
            servicioId: 'Servicio',
            formaPagoId: 'Forma de pago',
            precioContratado: 'Precio',
            fechaInicio: 'Fecha inicial',
            fechaFin: 'Fecha final',
            contratoId: 'Contrato',
            monto: 'Monto',
            fechaPago: 'Fecha de pago',
            referencia: 'Referencia'
        };

        return etiquetas[columna] ?? columna;
    }

    formatear(
        registro: Record<string, any>,
        columna: string
    ): string {

        const valor = registro[columna];

        if (columna === 'usuarioEnSesionId') {

            return valor == null
                ? 'Disponible'
                : `Cajero #${valor}`;
        }

        if (valor == null || valor === '') {
            return '—';
        }

        if (
            columna === 'monto' ||
            columna === 'precioContratado'
        ) {

            return new Intl.NumberFormat('es-EC', {
                style: 'currency',
                currency: 'USD'
            }).format(Number(valor));
        }

        if (columna.startsWith('fecha')) {

            return new Date(valor).toLocaleString(
                'es-EC',
                {
                    timeZone: 'America/Guayaquil'
                }
            );
        }

        if (columna === 'formaPagoId') {

            const formas: Record<number, string> = {
                1: 'Efectivo',
                2: 'Transferencia',
                3: 'Tarjeta'
            };

            return formas[Number(valor)] ?? String(valor);
        }

        return String(valor);
    }

    obtenerError(error: any): string {

        if (error.status === 0) {
            return 'No se pudo conectar con el servidor.';
        }

        if (error.status === 401) {
            return 'La sesión ha expirado.';
        }

        if (error.status === 403) {
            return 'No tienes permisos para esta operación.';
        }

        if (error.error?.errors) {

            return Object.values(error.error.errors)
                .flat()
                .join(' ');
        }

        if (typeof error.error === 'string') {
            return error.error;
        }

        return error.error?.mensaje
            || error.error?.message
            || error.error?.title
            || 'No se pudo completar la operación.';
    }
}
