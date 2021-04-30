using System;
namespace VentasGpo.Portable.Interfaces
{
    public interface IDataEncrypt
    {
        string Cifrado(string source);
        string DesCifrado(string encodedText);
    }
}
