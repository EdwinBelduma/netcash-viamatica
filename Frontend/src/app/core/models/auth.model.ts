export interface LoginSolicitud {
    nombreUsuario: string;
    contrasena: string;
}

export interface Usuario {
    id: number;
    nombreUsuario: string;
    nombres: string;
    apellidos: string;
    rol: string;
}

export interface LoginRespuesta {
    token: string;
    fechaExpiracion: string;
    usuario: Usuario;
}