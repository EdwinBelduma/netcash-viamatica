import {
    Component,
    OnInit,
    inject,
    signal,
    computed,
    ChangeDetectorRef
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

import { MantenimientoService } from '../../../core/services/mantenimiento.service';
import { AuthService } from '../../../core/services/auth.service';

import { ImportacionCsv } from '../importacion-csv/importacion-csv';

interface Campo {
    nombre: string;
    etiqueta: string;
    tipo: string;
    requerido?: boolean;
    minimo?: number;
    maximo?: number;
    minNumero?: number;
    paso?: number;
    patron?: string;
    soloCreacion?: boolean;
    opciones?: {
        valor: number;
        texto: string;
    }[];
}

interface Columna {
    nombre: string;
    etiqueta: string;
}

@Component({
    selector: 'app-mantenimiento',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        ImportacionCsv
    ],
    templateUrl: './mantenimiento.html',
    styleUrl: './mantenimiento.scss'
})
export class Mantenimiento implements OnInit {

    private servicio = inject(MantenimientoService);
    private ruta = inject(ActivatedRoute);
    private auth = inject(AuthService);
    private cdr = inject(ChangeDetectorRef);

    usuario = this.auth.obtenerUsuario();

    modulo = '';
    titulo = '';

    campos: Campo[] = [];
    columnas: Columna[] = [];

    registros = signal<Record<string, any>[]>([]);
    busqueda = signal('');
    cargando = signal(false);
    guardando = signal(false);
    formularioVisible = signal(false);

    mensaje = signal('');
    error = signal('');

    formulario: Record<string, any> = {};
    idEdicion: number | null = null;

    filtrados = computed(() => {

        const texto = this.busqueda()
            .trim()
            .toLowerCase();

        return this.registros().filter(registro =>
            this.columnas.some(columna =>
                String(registro[columna.nombre] ?? '')
                    .toLowerCase()
                    .includes(texto)
            )
        );
    });

    ngOnInit(): void {

        this.ruta.data.subscribe(datos => {

            this.modulo = datos['modulo'];
            this.titulo = datos['titulo'];

            this.formularioVisible.set(false);
            this.registros.set([]);
            this.busqueda.set('');
            this.mensaje.set('');
            this.error.set('');

            this.formulario = {};
            this.idEdicion = null;

            this.configurarModulo();
            this.cargar();
        });
    }

    // ==========================================
    // CONFIGURACION DE MODULOS
    // ==========================================

    configurarModulo(): void {

        switch (this.modulo) {

            // ==================================
            // CLIENTES
            // ==================================

            case 'clientes':

                this.campos = [
                    {
                        nombre: 'nombres',
                        etiqueta: 'Nombres',
                        tipo: 'text',
                        requerido: true,
                        maximo: 100
                    },
                    {
                        nombre: 'apellidos',
                        etiqueta: 'Apellidos',
                        tipo: 'text',
                        requerido: true,
                        maximo: 100
                    },
                    {
                        nombre: 'identificacion',
                        etiqueta: 'Identificación',
                        tipo: 'text',
                        requerido: true,
                        patron: '[0-9]{10,13}',
                        minimo: 10,
                        maximo: 13
                    },
                    {
                        nombre: 'correo',
                        etiqueta: 'Correo electrónico',
                        tipo: 'email',
                        requerido: true,
                        maximo: 120
                    },
                    {
                        nombre: 'telefono',
                        etiqueta: 'Teléfono',
                        tipo: 'tel',
                        requerido: true,
                        patron: '09[0-9]{8,18}',
                        minimo: 10,
                        maximo: 20
                    },
                    {
                        nombre: 'direccion',
                        etiqueta: 'Dirección',
                        tipo: 'text',
                        requerido: true,
                        minimo: 20,
                        maximo: 100
                    },
                    {
                        nombre: 'referenciaDireccion',
                        etiqueta: 'Referencia de dirección',
                        tipo: 'text',
                        requerido: true,
                        minimo: 20,
                        maximo: 100
                    }
                ];

                this.columnas = [
                    {
                        nombre: 'identificacion',
                        etiqueta: 'Identificación'
                    },
                    {
                        nombre: 'nombres',
                        etiqueta: 'Nombres'
                    },
                    {
                        nombre: 'apellidos',
                        etiqueta: 'Apellidos'
                    },
                    {
                        nombre: 'correo',
                        etiqueta: 'Correo'
                    },
                    {
                        nombre: 'telefono',
                        etiqueta: 'Teléfono'
                    }
                ];

                break;

            // ==================================
            // USUARIOS
            // ==================================

            case 'usuarios':

                this.campos = [
                    {
                        nombre: 'nombreUsuario',
                        etiqueta: 'Nombre de usuario',
                        tipo: 'text',
                        requerido: true,
                        minimo: 8,
                        maximo: 20,
                        patron: '(?=.*[A-Za-z])(?=.*[0-9])[A-Za-z0-9]{8,20}'
                    },
                    {
                        nombre: 'nombres',
                        etiqueta: 'Nombres',
                        tipo: 'text',
                        requerido: true,
                        maximo: 100
                    },
                    {
                        nombre: 'apellidos',
                        etiqueta: 'Apellidos',
                        tipo: 'text',
                        requerido: true,
                        maximo: 100
                    },
                    {
                        nombre: 'identificacion',
                        etiqueta: 'Identificación',
                        tipo: 'text',
                        requerido: true
                    },
                    {
                        nombre: 'correo',
                        etiqueta: 'Correo electrónico',
                        tipo: 'email',
                        requerido: true
                    },
                    {
                        nombre: 'contrasena',
                        etiqueta: 'Contraseña',
                        tipo: 'password',
                        requerido: true,
                        soloCreacion: true,
                        minimo: 8,
                        maximo: 30,
                        patron: '(?=.*[A-Z])(?=.*[0-9]).{8,30}'
                    },
                    {
                        nombre: 'rolId',
                        etiqueta: 'Rol',
                        tipo: 'select',
                        requerido: true,
                        opciones: [
                            {
                                valor: 2,
                                texto: 'Gestor'
                            },
                            {
                                valor: 3,
                                texto: 'Cajero'
                            }
                        ]
                    }
                ];

                this.columnas = [
                    {
                        nombre: 'nombreUsuario',
                        etiqueta: 'Usuario'
                    },
                    {
                        nombre: 'nombres',
                        etiqueta: 'Nombres'
                    },
                    {
                        nombre: 'apellidos',
                        etiqueta: 'Apellidos'
                    },
                    {
                        nombre: 'correo',
                        etiqueta: 'Correo'
                    },
                    {
                        nombre: 'rol',
                        etiqueta: 'Rol'
                    },
                    {
                        nombre: 'estado',
                        etiqueta: 'Estado'
                    }
                ];

                break;

            // ==================================
            // CAJAS
            // ==================================

            case 'cajas':

                this.campos = [
                    {
                        nombre: 'descripcion',
                        etiqueta: 'Descripción de la caja',
                        tipo: 'text',
                        requerido: true,
                        maximo: 50
                    }
                ];

                this.columnas = [
                    {
                        nombre: 'descripcion',
                        etiqueta: 'Descripción'
                    },
                    {
                        nombre: 'usuarioEnSesionId',
                        etiqueta: 'Cajero en sesión'
                    },
                    {
                        nombre: 'fechaApertura',
                        etiqueta: 'Fecha de apertura'
                    }
                ];

                break;

            // ==================================
            // SERVICIOS
            // ==================================

            case 'servicios':

                this.campos = [
                    {
                        nombre: 'nombre',
                        etiqueta: 'Nombre del servicio',
                        tipo: 'text',
                        requerido: true,
                        maximo: 100
                    },
                    {
                        nombre: 'descripcion',
                        etiqueta: 'Descripción',
                        tipo: 'text',
                        requerido: true,
                        maximo: 150
                    },
                    {
                        nombre: 'velocidadMbps',
                        etiqueta: 'Velocidad (Mbps)',
                        tipo: 'number',
                        requerido: true,
                        minNumero: 1,
                        paso: 1
                    },
                    {
                        nombre: 'precio',
                        etiqueta: 'Precio mensual ($)',
                        tipo: 'number',
                        requerido: true,
                        minNumero: 0.01,
                        paso: 0.01
                    }
                ];

                this.columnas = [
                    {
                        nombre: 'nombre',
                        etiqueta: 'Servicio'
                    },
                    {
                        nombre: 'descripcion',
                        etiqueta: 'Descripción'
                    },
                    {
                        nombre: 'velocidadMbps',
                        etiqueta: 'Velocidad (Mbps)'
                    },
                    {
                        nombre: 'precio',
                        etiqueta: 'Precio ($)'
                    }
                ];

                break;

            // ==================================
            // EQUIPOS
            // ==================================

            case 'equipos':

                this.campos = [
                    {
                        nombre: 'nombre',
                        etiqueta: 'Nombre del equipo',
                        tipo: 'text',
                        requerido: true,
                        maximo: 100
                    },
                    {
                        nombre: 'descripcion',
                        etiqueta: 'Descripción',
                        tipo: 'text',
                        requerido: true
                    },
                    {
                        nombre: 'servicioId',
                        etiqueta: 'Servicio asociado',
                        tipo: 'select',
                        requerido: true,
                        opciones: []
                    }
                ];

                this.columnas = [
                    {
                        nombre: 'nombre',
                        etiqueta: 'Equipo'
                    },
                    {
                        nombre: 'descripcion',
                        etiqueta: 'Descripción'
                    },
                    {
                        nombre: 'servicioId',
                        etiqueta: 'Servicio asociado'
                    }
                ];

                this.cargarServicios();

                break;

            default:

                this.campos = [];
                this.columnas = [];

                break;
        }
    }

    // ==========================================
    // PERMISOS SEGUN ROL
    // ==========================================

    puedeModificar(): boolean {

        const rol = this.usuario?.rol;

        if (this.modulo === 'usuarios') {
            return rol === 'Administrador';
        }

        if (this.modulo === 'clientes') {
            return rol === 'Administrador'
                || rol === 'Cajero';
        }

        return rol === 'Administrador'
            || rol === 'Gestor';
    }

    puedeEliminar(): boolean {

        const rol = this.usuario?.rol;

        if (
            this.modulo === 'usuarios' ||
            this.modulo === 'clientes'
        ) {
            return rol === 'Administrador';
        }

        return rol === 'Administrador'
            || rol === 'Gestor';
    }

    // ==========================================
    // CARGAR SERVICIOS PARA EQUIPOS
    // ==========================================

    cargarServicios(): void {

        this.servicio.listar('servicios').subscribe({

            next: respuesta => {

                if (this.modulo !== 'equipos') {
                    return;
                }

                const campo = this.campos.find(
                    x => x.nombre === 'servicioId'
                );

                if (campo) {

                    campo.opciones = respuesta.map(
                        servicio => ({
                            valor: servicio['id'],
                            texto: servicio['nombre']
                        })
                    );

                    this.campos = [...this.campos];
                    this.cdr.markForCheck();
                }
            },

            error: error => {
                this.error.set(this.obtenerError(error));
            }
        });
    }

    // ==========================================
    // LISTAR REGISTROS
    // ==========================================

    cargar(): void {

        this.cargando.set(true);

        const moduloActual = this.modulo;

        this.servicio.listar(moduloActual).subscribe({

            next: respuesta => {

                if (this.modulo !== moduloActual) {
                    return;
                }

                this.registros.set(respuesta);
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

    // ==========================================
    // NUEVO REGISTRO
    // ==========================================

    nuevo(): void {

        if (!this.puedeModificar()) {
            return;
        }

        this.idEdicion = null;
        this.formulario = {};

        this.mensaje.set('');
        this.error.set('');

        for (const campo of this.campos) {

            this.formulario[campo.nombre] =
                campo.tipo === 'select' ||
                campo.tipo === 'number'
                    ? null
                    : '';
        }

        this.formularioVisible.set(true);
    }

    // ==========================================
    // EDITAR REGISTRO
    // ==========================================

    editar(registro: Record<string, any>): void {

        if (!this.puedeModificar()) {
            return;
        }

        if (
            this.modulo === 'usuarios' &&
            registro['rol'] === 'Administrador'
        ) {
            this.error.set(
                'El administrador inicial no se modifica desde este formulario.'
            );
            return;
        }

        this.error.set('');
        this.mensaje.set('');

        const moduloActual = this.modulo;

        this.servicio.obtener(
            moduloActual,
            registro['id']
        ).subscribe({

            next: respuesta => {

                if (this.modulo !== moduloActual) {
                    return;
                }

                this.idEdicion = registro['id'];
                this.formulario = {};

                for (const campo of this.campos) {

                    if (!campo.soloCreacion) {

                        this.formulario[campo.nombre] =
                            respuesta[campo.nombre] ??
                            (campo.tipo === 'select' ||
                             campo.tipo === 'number'
                                ? null
                                : '');
                    }
                }

                if (
                    this.modulo === 'usuarios' &&
                    !this.formulario['rolId']
                ) {

                    const roles: Record<string, number> = {
                        Administrador: 1,
                        Gestor: 2,
                        Cajero: 3
                    };

                    this.formulario['rolId'] =
                        roles[respuesta['rol']] ?? null;
                }

                this.formularioVisible.set(true);
            },

            error: error => {
                this.error.set(this.obtenerError(error));
            }
        });
    }

    // ==========================================
    // CANCELAR FORMULARIO
    // ==========================================

    cancelar(): void {

        this.formularioVisible.set(false);
        this.formulario = {};
        this.idEdicion = null;
    }

    // ==========================================
    // GUARDAR REGISTRO
    // ==========================================

    guardar(): void {

        if (
            this.guardando() ||
            !this.puedeModificar()
        ) {
            return;
        }

        this.error.set('');
        this.mensaje.set('');

        const datos = {
            ...this.formulario
        };

        if (this.modulo === 'servicios') {

            if (
                !Number.isInteger(datos['velocidadMbps']) ||
                datos['velocidadMbps'] <= 0
            ) {
                this.error.set(
                    'La velocidad debe ser un número entero positivo.'
                );
                return;
            }

            const precio = Number(datos['precio']);

            if (
                !Number.isFinite(precio) ||
                precio <= 0 ||
                Math.abs(precio * 100 -
                    Math.round(precio * 100)) > 0.000001
            ) {
                this.error.set(
                    'El precio debe ser positivo y tener máximo dos decimales.'
                );
                return;
            }
        }

        this.guardando.set(true);

        const moduloActual = this.modulo;

        const solicitud = this.idEdicion === null

            ? this.servicio.crear(
                moduloActual,
                datos
            )

            : this.servicio.actualizar(
                moduloActual,
                this.idEdicion,
                datos
            );

        solicitud.subscribe({

            next: () => {

                this.guardando.set(false);

                if (this.modulo !== moduloActual) {
                    return;
                }

                this.mensaje.set(
                    'Registro guardado correctamente.'
                );

                this.cancelar();
                this.cargar();
            },

            error: error => {

                this.guardando.set(false);

                this.error.set(
                    this.obtenerError(error)
                );
            }
        });
    }

    // ==========================================
    // ELIMINACION LOGICA
    // ==========================================

    eliminar(registro: Record<string, any>): void {

        if (!this.puedeEliminar()) {
            return;
        }

        if (
            this.modulo === 'usuarios' &&
            registro['id'] === this.usuario?.id
        ) {

            this.error.set(
                'No puedes eliminar tu propio usuario.'
            );

            return;
        }

        if (!confirm('¿Deseas eliminar este registro?')) {
            return;
        }

        this.error.set('');
        this.mensaje.set('');

        const moduloActual = this.modulo;

        this.servicio.eliminar(
            moduloActual,
            registro['id']
        ).subscribe({

            next: () => {

                if (this.modulo !== moduloActual) {
                    return;
                }

                this.mensaje.set(
                    'Registro eliminado correctamente.'
                );

                this.cargar();
            },

            error: error => {

                this.error.set(
                    this.obtenerError(error)
                );
            }
        });
    }

    // ==========================================
    // APROBAR USUARIO
    // ==========================================

    aprobar(registro: Record<string, any>): void {

        if (this.usuario?.rol !== 'Administrador') {
            return;
        }

        if (!confirm('¿Deseas aprobar este usuario?')) {
            return;
        }

        this.error.set('');
        this.mensaje.set('');

        this.servicio.aprobarUsuario(
            registro['id']
        ).subscribe({

            next: () => {

                this.mensaje.set(
                    'Usuario aprobado correctamente.'
                );

                this.cargar();
            },

            error: error => {

                this.error.set(
                    this.obtenerError(error)
                );
            }
        });
    }

    // ==========================================
    // MOSTRAR VALORES EN LA TABLA
    // ==========================================

    formatearValor(
        registro: Record<string, any>,
        columna: string
    ): string {

        const valor = registro[columna];

        if (columna === 'usuarioEnSesionId') {
            return valor == null
                ? 'Disponible'
                : `Cajero #${valor}`;
        }

        if (
            valor === null ||
            valor === undefined ||
            valor === ''
        ) {
            return '—';
        }

        if (columna === 'precio') {

            return new Intl.NumberFormat('es-EC', {
                style: 'currency',
                currency: 'USD'
            }).format(Number(valor));
        }

        if (columna === 'fechaApertura') {

            return new Date(valor).toLocaleString(
                'es-EC',
                {
                    timeZone: 'America/Guayaquil'
                }
            );
        }

        if (columna === 'servicioId') {

            const campo = this.campos.find(
                x => x.nombre === 'servicioId'
            );

            return campo?.opciones?.find(
                x => x.valor === Number(valor)
            )?.texto ?? `Servicio #${valor}`;
        }

        if (columna === 'estado') {

            const estados: Record<string, string> = {
                PendienteAprobacion: 'Pendiente de aprobación',
                Activo: 'Activo',
                Inactivo: 'Inactivo',
                Bloqueado: 'Bloqueado'
            };

            return estados[String(valor)] ?? String(valor);
        }

        return String(valor);
    }

    // ==========================================
    // MANEJO DE ERRORES
    // ==========================================

    obtenerError(error: any): string {

        if (error.status === 0) {
            return 'No se pudo conectar con el servidor.';
        }

        if (error.status === 401) {
            return 'Tu sesión ha expirado. Inicia sesión nuevamente.';
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