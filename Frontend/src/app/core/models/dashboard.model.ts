export interface Indicador {
    id: number;
    nombre: string;
    turnosAtendidos: number;
}

export interface Sesion {
    id: number;
    nombreUsuario: string;
    rol: string;
    sesionActiva: boolean;
    ultimoInicioSesion: string | null;
}

export interface DashboardRespuesta {
    totalClientes: number;
    totalUsuarios: number;
    totalCajas: number;
    cajasOcupadas: number;
    cajasDisponibles: number;
    cajerosActivos: number;
    gestoresActivos: number;
    usuariosPendientes: number;
    sesionesActivas: number;
    turnosPendientes: number;
    turnosEnAtencion: number;
    turnosAtendidos: number;
    contratosVigentes: number;
    totalPagos: number;
    totalRecaudado: number;
    indicadoresCajas: Indicador[];
    indicadoresCajeros: Indicador[];
    indicadoresGestores: Indicador[];
    sesiones: Sesion[];
}

export interface Menu {
    id: number;
    nombre: string;
    ruta: string;
    icono: string;
    orden: number;
    menuPadreId: number | null;
}