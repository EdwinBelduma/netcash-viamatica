using System.ComponentModel.DataAnnotations;

namespace NetCash.Aplicacion.DTOs.Clientes;

public class ClienteSolicitud
{
    [Required]
    [StringLength(100)]
    public string Nombres { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Apellidos { get; set; } = string.Empty;

    [Required]
    [RegularExpression(
        @"^(\d{10}|\d{13})$",
        ErrorMessage = "Ingrese una identificacion de 10 o 13 digitos.")]
    public string Identificacion { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(120)]
    public string Correo { get; set; } = string.Empty;

    [Required]
    [RegularExpression(
        @"^\d{10}$",
        ErrorMessage = "El telefono debe contener 10 digitos.")]
    public string Telefono { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Direccion { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ReferenciaDireccion { get; set; } = string.Empty;
}