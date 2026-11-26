export interface ImportacionRespuesta {
  totalRegistros: number;
  registrosImportados: number;
  registrosRechazados: number;
  errores: string[];
}

export type TipoImportacion = 'usuarios' | 'clientes';