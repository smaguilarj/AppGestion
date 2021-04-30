using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using ModernHttpClient;
using Xamarin.Forms;

using AppNutOp.Models;
using VentasGpo.Portable.Interfaces;
using VentasGpo.Portable.Helpers;

namespace VentasGpo.Portable.Services
{
    public class UserService
    {
        //private static HttpClientHandler handler = DependencyService.Get<IHttpClientHandlerService>().GetInsecureHandler();
        private static HttpClient client = new HttpClient(new NativeMessageHandler());
        private IDataEncrypt Encrypt => DependencyService.Get<IDataEncrypt>();

        /// <summary>
        /// Validacion usuario login
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<Usuario> ValidaAcceso(string usuario, string password)
        {
            string parametros = Constants.BASE_HERDEZ_API + "/Usuario/Acceso?Usr=" + Encrypt.Cifrado(usuario) + "&Password=" + Encrypt.Cifrado(password);
            
            try
            {
                Debug.WriteLine("== url: " + parametros);
                string content = await client.GetStringAsync(parametros);
                Debug.WriteLine("== result: " + content);

                Usuario Respuesta = JsonConvert.DeserializeObject<Usuario>(content);
                if (Respuesta.Id != null)
                {
                    Usuario item = Respuesta;
                    item.Id = Encrypt.DesCifrado(item.Id);
                    item.NoPersonal = Encrypt.DesCifrado(item.NoPersonal);
                    item.Puesto = Encrypt.DesCifrado(item.Puesto);
                    item.Nombre = Encrypt.DesCifrado(item.Nombre);
                    item.Contrasena = Encrypt.DesCifrado(item.Contrasena);
                    item.Departamento = Encrypt.DesCifrado(item.Departamento);
                    item.Tienda = Encrypt.DesCifrado(item.Tienda);
                    item.FechaIngreso = Encrypt.DesCifrado(item.FechaIngreso);
                    item.Perfil = Encrypt.DesCifrado(item.Perfil);
                    item.Nivel = Encrypt.DesCifrado(item.Nivel);
                    item.Correo = Encrypt.DesCifrado(item.Correo);
                    item.Sexo = Encrypt.DesCifrado(item.Sexo);
                    item.Status = Encrypt.DesCifrado(item.Status);

                    item.JsonMarcas = Encrypt.DesCifrado(item.JsonMarcas);
                    item.JsonModulos = Encrypt.DesCifrado(item.JsonModulos);
                    item.JsonDepartamento = Encrypt.DesCifrado(item.JsonDepartamento);

                    Debug.WriteLine("=x=");
                    Debug.WriteLine("perfil:" + item.Perfil);
                    Debug.WriteLine("nivel:" + item.Nivel);
                    Debug.WriteLine(item.Nombre);
                    Debug.WriteLine(item.Departamento);
                    Debug.WriteLine(item.Tienda);
                    Debug.WriteLine("puesto:" + item.Puesto);
                    Debug.WriteLine("id:" + item.Id);
                    Debug.WriteLine("sexo:" + item.Sexo);
                    Debug.WriteLine("correo:" + item.Correo);
                    Debug.WriteLine("marcas:" + item.JsonMarcas);
                    Debug.WriteLine("=x=");

                    //Respuesta = item;
                    return item;
                }
                else
                    return null;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("" + e.Message.ToString());
                //Debug.WriteLine(e.InnerException.Message);
                Debug.WriteLine(e.InnerException.ToString());
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("" + ex.Message);
                Debug.WriteLine(ex.InnerException.Message);
                return null;
            }
            
        }

    }
}
