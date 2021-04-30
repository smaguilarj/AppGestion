using System;
using System.IO;

using VentasGpo.iOS.Config;
using VentasGpo.Portable.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(DataConfig))]
namespace VentasGpo.iOS.Config
{
    public class DataConfig : IDataConfig
    {
        public DataConfig()
        {
        }

        public string GetBase()
        {
            string folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string fileNameConfig = Path.Combine(folderPath, "config.xml");
            string texto = "";

            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "temp.xml");
            File.WriteAllText(fileName, "texto de prueba");
            if (File.Exists(fileName))
            {
                texto = File.ReadAllText(fileName);
            }

            if (File.Exists(fileNameConfig))
            {
                texto = File.ReadAllText(fileNameConfig);
            }
            return texto;
        }

        public string ReadUserData()
        {
            string texto = "";
            try
            {
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "temp.xml");
                if (File.Exists(fileName))
                {
                    texto = File.ReadAllText(fileName);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return texto;
        }

        public void ClearfileUsr()
        {
            try
            {
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "temp.xml");
                string texto = "";
                File.WriteAllText(fileName, "");

                if (File.Exists(fileName))
                {
                    texto = File.ReadAllText(fileName);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void WriteUserData(string user, string pass)
        {
            try
            {
                ClearfileUsr();
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "temp.xml");
                string texto = "";
                File.WriteAllText(fileName, user + "|" + pass);

                if (File.Exists(fileName))
                {
                    texto = File.ReadAllText(fileName);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
