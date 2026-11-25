namespace NetCash.Aplicacion.DTOs.Pagos;

public class PagoRespuesta
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public int ContratoId { get; set; }

    public int CajaId { get; set; }

    public int CajeroId { get; set; }

    public int FormaPagoId { get; set; }

    public decimal Monto { get; set; }

    public DateTime FechaPago { get; set; }

    public string? Referencia { get; set; }

    public string? Observacion { get; set; }
}