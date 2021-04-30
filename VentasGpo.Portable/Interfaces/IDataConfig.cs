using System;
namespace VentasGpo.Portable.Interfaces
{
    public interface IDataConfig
    {
        string GetBase();
        void WriteUserData(string user, string pass);
        string ReadUserData();
        void ClearfileUsr();
    }
}
