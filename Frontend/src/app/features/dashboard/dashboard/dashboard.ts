
import {
    Component,
    OnInit,
    computed,
    inject,
    signal
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { AuthService } from '../../../core/services/auth.service';
import { DashboardService } from '../../../core/services/dashboard.service';

import {
    DashboardRespuesta,
    Indicador
} from '../../../core/models/dashboard.model';

@Component({
    selector: 'app-dashboard',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule
    ],
    templateUrl: './dashboard.html',
    styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit {

    private dashboardService = inject(DashboardService);
    private authService = inject(AuthService);

    usuario = this.authService.obtenerUsuario();

    resumen = signal<DashboardRespuesta | null>(null);
    cargando = signal(false);
    error = signal('');

    desde = this.fechaActual();
    hasta = this.fechaActual();

    esAdministrador =
        this.usuario?.rol === 'Administrador';

    esGestor =
        this.usuario?.rol === 'Gestor';

    tarjetas = computed(() => {

        const datos = this.resumen();

        if (!datos) {
            return [];
        }

        const tarjetas = [
            {
                titulo: 'Total recaudado',
                valor: new Intl.NumberFormat('es-EC', {
                    style: 'currency',
                    currency: 'USD'
                }).format(datos.totalRecaudado),
                icono: 'bi-currency-dollar',
                color: 'verde'
            },
            {
                titulo: 'Turnos atendidos',
                valor: datos.turnosAtendidos,
                icono: 'bi-check-circle',
                color: 'azul'
            },
            {
                titulo: 'Turnos pendientes',
                valor: datos.turnosPendientes,
                icono: 'bi-clock-history',
                color: 'amarillo'
            },
            {
                titulo: 'Turnos en atención',
                valor: datos.turnosEnAtencion,
                icono: 'bi-person-workspace',
                color: 'morado'
            },
            {
                titulo: 'Cajas disponibles',
                valor: datos.cajasDisponibles,
                icono: 'bi-wallet2',
                color: 'verde'
            },
            {
                titulo: 'Cajas ocupadas',
                valor: datos.cajasOcupadas,
                icono: 'bi-safe',
                color: 'rojo'
            },
            {
                titulo: 'Contratos vigentes',
                valor: datos.contratosVigentes,
                icono: 'bi-file-earmark-check',
                color: 'azul'
            },
            {
                titulo: 'Pagos registrados',
                valor: datos.totalPagos,
                icono: 'bi-credit-card',
                color: 'morado'
            }
        ];

        if (this.esAdministrador) {
            tarjetas.push(
                {
                    titulo: 'Total clientes',
                    valor: datos.totalClientes,
                    icono: 'bi-people',
                    color: 'azul'
                },
                {
                    titulo: 'Total usuarios',
                    valor: datos.totalUsuarios,
                    icono: 'bi-person-badge',
                    color: 'morado'
                },
                {
                    titulo: 'Usuarios pendientes',
                    valor: datos.usuariosPendientes,
                    icono: 'bi-person-exclamation',
                    color: 'amarillo'
                },
                {
                    titulo: 'Sesiones activas',
                    valor: datos.sesionesActivas,
                    icono: 'bi-person-check',
                    color: 'verde'
                }
            );
        }

        return tarjetas;
    });

    ngOnInit(): void {
        this.cargarDashboard();
    }

    fechaActual(): string {
        return new Intl.DateTimeFormat('en-CA', {
            timeZone: 'America/Guayaquil',
            year: 'numeric',
            month: '2-digit',
            day: '2-digit'
        }).format(new Date());
    }

    cargarDashboard(): void {

        this.error.set('');

        if (!this.desde || !this.hasta) {
            this.error.set(
                'Selecciona ambas fechas.'
            );
            return;
        }

        if (this.desde > this.hasta) {
            this.error.set(
                'La fecha inicial no puede superar la fecha final.'
            );
            return;
        }

        this.cargando.set(true);

        this.dashboardService.obtenerResumen(
            this.desde,
            this.hasta
        ).subscribe({
            next: respuesta => {
                this.resumen.set(respuesta);
                this.cargando.set(false);
            },
            error: () => {
                this.resumen.set(null);
                this.error.set(
                    'No se pudieron cargar los indicadores.'
                );
                this.cargando.set(false);
            }
        });
    }

    porcentaje(
        cantidad: number,
        indicadores: Indicador[]
    ): number {

        const maximo = Math.max(
            1,
            ...indicadores.map(
                x => x.turnosAtendidos
            )
        );

        return cantidad * 100 / maximo;
    }
}
