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
    public class ActivitiesService
    {
        //private static HttpClientHandler handler = DependencyService.Get<IHttpClientHandlerService>().GetInsecureHandler();
        private static HttpClient client = new HttpClient(new NativeMessageHandler());
        private IDataEncrypt Encrypt => DependencyService.Get<IDataEncrypt>();

        public async Task<List<Activities>> GetAllActivities(string Marca, string Region, string Distrito, string Fecha)
        {
            Region = Region == "" ? "" : Encrypt.Cifrado(Region);
            Distrito = Distrito == "" ? "" : Encrypt.Cifrado(Distrito);
            Fecha = Fecha == "" ? "" : Encrypt.Cifrado(Fecha);
            Marca = Marca == "" ? "" : Encrypt.Cifrado(Marca);
            string parametros = Constants.BASE_HERDEZ_API + "/Activities/GetAllActivities?Region=" + Region
                + "&Distrito=" + Distrito
                + "&Fecha=" + Fecha
                + "&Marca=" + Marca;

            try
            {
                Debug.WriteLine("== url: " + parametros);
                string content = await client.GetStringAsync(parametros);
                Debug.WriteLine("== result: " + content);

                List<Activities> ListActivities = new List<Activities>();
                List<Dictionary<string, object>> jresultOrigen = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(content);

                if (jresultOrigen.Count > 0)
                {
                    foreach (var item in jresultOrigen)
                    {
                        Activities Respuesta = new Activities()
                        {
                            Id = Convert.ToInt32(Encrypt.DesCifrado(item["Id"].ToString())),
                            Nombre = Encrypt.DesCifrado(item["Nombre"].ToString()),
                            Descripcion = Encrypt.DesCifrado(item["Descripcion"].ToString()),
                            FechaRegistro = Encrypt.DesCifrado(item["FechaRegistro"].ToString()),
                            FechaInicio = Encrypt.DesCifrado(item["FechaInicio"].ToString()),
                            FechaTermino = Encrypt.DesCifrado(item["FechaTermino"].ToString()),
                            Estatus = Convert.ToInt32(Encrypt.DesCifrado(item["Estatus"].ToString())),
                            ModoTiendas = Encrypt.DesCifrado(item["ModoTiendas"].ToString()),
                            ModoZona = Encrypt.DesCifrado(item["ModoZona"].ToString()),
                            NumTiendasDone = Convert.ToInt32(Encrypt.DesCifrado(item["NumTiendasDone"].ToString())),
                            NumTiendasTarget = Convert.ToInt32(Encrypt.DesCifrado(item["NumTiendasTarget"].ToString())),
                        };

                        if (item["Tiendas"] != null)
                            Respuesta.Tiendas = ListaTiendas(item["Tiendas"].ToString());
                        //if (item["Distritos"] != null)
                        //    Respuesta.Distritos = ListaDistritos(item["Distritos"].ToString());
                        //if (item["Regiones"] != null)
                        //    Respuesta.Regiones = ListaRegiones(item["Regiones"].ToString());
                        if (item["Usuarios"] != null)
                            Respuesta.Usuarios = ListaUsuarios(item["Usuarios"].ToString());

                        ListActivities.Add(Respuesta);
                    }
                }

                return (ListActivities.Count > 0) ? ListActivities : null;

            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("H" + e.Message);
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("E" + ex.Message);
                return null;
            }
        }

        private List<Tienda> ListaTiendas(string tiendas)
        {
            List<Tienda> tiendas1 = new List<Tienda>();
            
            List<Dictionary<string, object>> jresultTiendas = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(tiendas);
            if (jresultTiendas.Count > 0)
            {
                foreach (var item in jresultTiendas)
                {
                    Tienda tienda = new Tienda()
                    {
                        Id = Convert.ToInt32(Encrypt.DesCifrado(item["Id"].ToString())),
                        Nombre = Encrypt.DesCifrado(item["Nombre"].ToString()),
                        Distrito = Encrypt.DesCifrado(item["Distrito"].ToString()),
                        Region = Encrypt.DesCifrado(item["Region"].ToString()),
                        IdMarca = Convert.ToInt32(Encrypt.DesCifrado(item["IdMarca"].ToString()))
                    };
                    tiendas1.Add(tienda);
                }
            }

            return (tiendas1.Count > 0) ? tiendas1 : null;
        }

        private List<Distrito> ListaDistritos(string tiendas)
        {
            List<Distrito> tiendas1 = new List<Distrito>();

            List<Dictionary<string, object>> jresultTiendas = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(tiendas);
            if (jresultTiendas.Count > 0)
            {
                foreach (var item in jresultTiendas)
                {
                    Distrito tienda = new Distrito()
                    {
                        Nombre = Encrypt.DesCifrado(item["Nombre"].ToString()),
                        Region = Encrypt.DesCifrado(item["Region"].ToString()),
                        IdMarca = Convert.ToInt32(Encrypt.DesCifrado(item["IdMarca"].ToString()))
                    };
                    tiendas1.Add(tienda);
                }
            }

            return (tiendas1.Count > 0) ? tiendas1 : null;
        }

        private List<Regione> ListaRegiones(string tiendas)
        {
            List<Regione> tiendas1 = new List<Regione>();

            List<Dictionary<string, object>> jresultTiendas = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(tiendas);
            if (jresultTiendas.Count > 0)
            {
                foreach (var item in jresultTiendas)
                {
                    Regione tienda = new Regione()
                    {
                        Nombre = Encrypt.DesCifrado(item["Nombre"].ToString()),
                        IdMarca = Convert.ToInt32(Encrypt.DesCifrado(item["IdMarca"].ToString()))
                    };
                    tiendas1.Add(tienda);
                }
            }

            return (tiendas1.Count > 0) ? tiendas1 : null;
        }

        private List<UserActividad> ListaUsuarios(string tiendas)
        {
            List<UserActividad> tiendas1 = new List<UserActividad>();

            List<Dictionary<string, object>> jresultTiendas = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(tiendas);
            if (jresultTiendas.Count > 0)
            {
                foreach (var item in jresultTiendas)
                {
                    UserActividad tienda = new UserActividad()
                    {
                        Tienda = Encrypt.DesCifrado(item["Tienda"].ToString()),
                        IdTienda = Convert.ToInt32(Encrypt.DesCifrado(item["IdTienda"].ToString())),
                        IdActividad = Convert.ToInt32(Encrypt.DesCifrado(item["IdActividad"].ToString())),
                        NoEmpleado = Encrypt.DesCifrado(item["NoEmpleado"].ToString()),
                        Nombre = Encrypt.DesCifrado(item["Nombre"].ToString()),
                        FechaEntrega = Encrypt.DesCifrado(item["FechaEntrega"].ToString())
                    };
                    tiendas1.Add(tienda);
                }
            }

            return (tiendas1.Count > 0) ? tiendas1 : null;
        }

        public async Task<Sales> GetActivity(string Marca, string Region, string Distrito, string IdActividad)
        {
            Sales Respuesta = new Sales();
            Respuesta.Resultado = new SalesResultTotal();
            Respuesta.ListaNewData = new List<SalesByPerfil>();

            Region = Region == "" ? "" : Encrypt.Cifrado(Region);
            Distrito = Distrito == "" ? "" : Encrypt.Cifrado(Distrito);
            IdActividad = IdActividad == "" ? "" : Encrypt.Cifrado(IdActividad);
            Marca = Marca == "" ? "" : Encrypt.Cifrado(Marca);
            string parametros = Constants.BASE_HERDEZ_API + "/Activities/GetActivity?Region=" + Region
                + "&Distrito=" + Distrito
                + "&IdActividad=" + IdActividad
                + "&Marca=" + Marca;

            try
            {
                Debug.WriteLine("== url: " + parametros);
                string content = await client.GetStringAsync(parametros);
                Debug.WriteLine("== result: " + content);
                //Respuesta = JsonConvert.DeserializeObject<List<ModelForecast>>(content);

                NewSalesTemp jresultPromo = JsonConvert.DeserializeObject<NewSalesTemp>(content);

                //******VENTA*****
                Respuesta.Resultado.VentaActual = Encrypt.DesCifrado(jresultPromo.Resultado["VentaActual"].ToString());
                Respuesta.Resultado.PorcentageVenta = Encrypt.DesCifrado(jresultPromo.Resultado["PorcentageVenta"].ToString());
                //******VENTA*****

                //******VISITAS*****
                Respuesta.Resultado.VisitasTotales = Encrypt.DesCifrado(jresultPromo.Resultado["VisitasTotales"].ToString());
                Respuesta.Resultado.PorcentageVisitas = Encrypt.DesCifrado(jresultPromo.Resultado["PorcentageVisitas"].ToString());
                //******VISITAS*****

                //******TPROMEDIO*****
                Respuesta.Resultado.TicketPromedio = Encrypt.DesCifrado(jresultPromo.Resultado["TicketPromedio"].ToString());
                Respuesta.Resultado.PorcentageTicket = Encrypt.DesCifrado(jresultPromo.Resultado["PorcentageTicket"].ToString());
                //******TPROMEDIO*****

                //******FORECAST*****
                Respuesta.Resultado.VentaFC = Encrypt.DesCifrado(jresultPromo.Resultado["VentaFC"].ToString());
                Respuesta.Resultado.TotalFC = Encrypt.DesCifrado(jresultPromo.Resultado["TotalFC"].ToString());
                Respuesta.Resultado.PorcentageFC = Encrypt.DesCifrado(jresultPromo.Resultado["PorcentageFC"].ToString());
                //******FORECAST*****


                foreach (var item in jresultPromo.ListaNewData)
                {
                    SalesByPerfil Temp = new SalesByPerfil
                    {
                        Hora = Convert.ToInt32(Encrypt.DesCifrado(item["Hora"].ToString())),
                        VentaNeta = Convert.ToDouble(Encrypt.DesCifrado(item["VentaNeta"].ToString())),
                        Visitas = Convert.ToDouble(Encrypt.DesCifrado(item["Visitas"].ToString())),
                        TicketPromedio = Convert.ToDouble(Encrypt.DesCifrado(item["TicketPromedio"].ToString())),
                        Bandera = Encrypt.DesCifrado(item["Bandera"].ToString())
                    };

                    Respuesta.ListaNewData.Add(Temp);
                }

                return Respuesta;

            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("H" + e.Message);
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("E" + ex.Message);
                return null;
            }
        }
    }
}
