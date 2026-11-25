
using System.Security.Cryptography;
using System.Text;
using NetCash.Aplicacion.Interfaces;

namespace NetCash.Infraestructura.Seguridad;

public class CifradoServicio : ICifradoServicio
{
    private readonly byte[] _clave;

    public CifradoServicio(string claveBase64)
    {
        _clave = Convert.FromBase64String(claveBase64);

        if (_clave.Length != 32)
        {
            throw new ArgumentException(
                "La clave AES debe tener 256 bits.");
        }
    }

    public string Cifrar(string texto)
    {
        var datos = Encoding.UTF8.GetBytes(texto);
        var nonce = RandomNumberGenerator.GetBytes(12);
        var etiqueta = new byte[16];
        var cifrado = new byte[datos.Length];

        using var aes = new AesGcm(_clave, 16);

        aes.Encrypt(nonce, datos, cifrado, etiqueta);

        var resultado = new byte[
            nonce.Length +
            etiqueta.Length +
            cifrado.Length];

        Buffer.BlockCopy(
            nonce, 0, resultado, 0, nonce.Length);

        Buffer.BlockCopy(
            etiqueta, 0, resultado,
            nonce.Length, etiqueta.Length);

        Buffer.BlockCopy(
            cifrado, 0, resultado,
            nonce.Length + etiqueta.Length,
            cifrado.Length);

        return Convert.ToBase64String(resultado);
    }

    public string Descifrar(string textoCifrado)
    {
        var datos = Convert.FromBase64String(textoCifrado);

        if (datos.Length < 28)
        {
            throw new CryptographicException(
                "El contenido cifrado no es valido.");
        }

        var nonce = datos[..12];
        var etiqueta = datos[12..28];
        var cifrado = datos[28..];

        var texto = new byte[cifrado.Length];

        using var aes = new AesGcm(_clave, 16);

        aes.Decrypt(nonce, cifrado, etiqueta, texto);

        return Encoding.UTF8.GetString(texto);
    }
}
