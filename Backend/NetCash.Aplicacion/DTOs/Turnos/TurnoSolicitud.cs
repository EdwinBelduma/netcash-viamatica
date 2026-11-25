using System.ComponentModel.DataAnnotations;
using NetCash.Dominio.Enumeraciones;

namespace NetCash.Aplicacion.DTOs.Turnos;

public class TurnoSolicitud
{
    [Range(1, int.MaxValue)]
    public int CajaId { get; set; }

    [Range(1, int.MaxValue)]
    public int CajeroId { get; set; }

    [Range(1, int.MaxValue)]
    public int? ClienteId { get; set; }

    [EnumDataType(typeof(TipoAtencion))]
    public TipoAtencion TipoAtencion { get; set; }
}