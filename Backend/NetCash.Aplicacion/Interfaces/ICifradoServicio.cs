namespace NetCash.Aplicacion.Interfaces;

public interface ICifradoServicio
{
    string Cifrar(string texto);

    string Descifrar(string textoCifrado);
}