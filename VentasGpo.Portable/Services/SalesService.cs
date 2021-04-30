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
    public class SalesService
    {
        //private static HttpClientHandler handler = DependencyService.Get<IHttpClientHandlerService>().GetInsecureHandler();
        private static HttpClient client = new HttpClient(new NativeMessageHandler());
        private IDataEncrypt Encrypt => DependencyService.Get<IDataEncrypt>();

        public async Task<List<DataPerfil>> RecuperaDataByPerfil(int Perfil, string Usuario, string Marca)
        {
            List<DataPerfil> Respuesta = new List<DataPerfil>();
            string parametros = Constants.BASE_HERDEZ_API + "/Sales/RecuperaDataByPerfil?Perfil=" + Encrypt.Cifrado(Perfil.ToString())
                + "&Usuario=" + Encrypt.Cifrado(Usuario)
                + "&Marca=" + Encrypt.Cifrado(Marca);
            try
            {
                Debug.WriteLine("== url: " + parametros);
                string content = await client.GetStringAsync(parametros);
                Debug.WriteLine("== result: " + content);

                List<Dictionary<string, object>> jresultPromo = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(content);
                if (jresultPromo.Count > 0)
                {
                    foreach (var z in jresultPromo)
                    {
                        //poner validacion para los campos nullos
                        DataPerfil temporal = new DataPerfil
                        {
                            Distritos = Encrypt.DesCifrado(z["Distritos"].ToString()),
                            Tiendas = Encrypt.DesCifrado(z["Tiendas"].ToString()) == "" ? 0 : Convert.ToInt32(Encrypt.DesCifrado(z["Tiendas"].ToString())),
                            NombreTienda = Encrypt.DesCifrado(z["NombreTienda"].ToString()),
                            Regiones = Encrypt.DesCifrado(z["Regiones"].ToString())
                        };

                        Respuesta.Add(temporal);
                    }
                }

                return Respuesta;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("E" + e.ToString());
                Debug.WriteLine("E" + e.InnerException.ToString());
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("E" + ex.Message);
                return null;
            }
        }

        public async Task<Sales> GetAllSalesNew(string Region, string Distrito, string Sucursal, string Marca = "")
        {
            Sales Respuesta = new Sales();
            Respuesta.Resultado = new SalesResultTotal();
            Respuesta.ListaNewData = new List<SalesByPerfil>();
            
            Region = Region == "" ? "" : Encrypt.Cifrado(Region);
            Distrito = Distrito == "" ? "" : Encrypt.Cifrado(Distrito);
            Sucursal = Sucursal == "" ? "" : Encrypt.Cifrado(Sucursal);
            Marca = Marca == "" ? "" : Encrypt.Cifrado(Marca);
            string parametros = Constants.BASE_HERDEZ_API + "/Sales/GetAllSalesNew?Region=" + Region
                + "&Distrito=" + Distrito
                + "&Sucursal=" + Sucursal
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
                Debug.WriteLine("E" + e.Message);
                Debug.WriteLine("E" + e.ToString());
                Debug.WriteLine("E" + e.InnerException.ToString());
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("E" + ex.Message);
                return null;
            }
        }

        public async Task<Sales> GetAllSalesNewCom(string Region, string Distrito, string Sucursal, string Marca = "")
        {
            Sales Respuesta = new Sales();
            Respuesta.Resultado = new SalesResultTotal();
            Respuesta.ListaNewData = new List<SalesByPerfil>();

            Region = Region == "" ? "" : Encrypt.Cifrado(Region);
            Distrito = Distrito == "" ? "" : Encrypt.Cifrado(Distrito);
            Sucursal = Sucursal == "" ? "" : Encrypt.Cifrado(Sucursal);
            Marca = Marca == "" ? "" : Encrypt.Cifrado(Marca);
            string parametros = Constants.BASE_HERDEZ_API + "/Sales/GetAllSalesNewCom?Region=" + Region
                + "&Distrito=" + Distrito
                + "&Sucursal=" + Sucursal
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
                    SalesByPerfil Temp = new SalesByPerfil();
                    Temp.Hora = Convert.ToInt32(Encrypt.DesCifrado(item["Hora"].ToString()));
                    Temp.VentaNeta = Convert.ToDouble(Encrypt.DesCifrado(item["VentaNeta"].ToString()));
                    Temp.Visitas = Convert.ToDouble(Encrypt.DesCifrado(item["Visitas"].ToString()));
                    Temp.TicketPromedio = Convert.ToDouble(Encrypt.DesCifrado(item["TicketPromedio"].ToString()));
                    Temp.Bandera = Encrypt.DesCifrado(item["Bandera"].ToString());

                    Respuesta.ListaNewData.Add(Temp);
                }

                return Respuesta;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("E" + e.Message);
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("E" + ex.Message);
                return null;
            }
        }

        // ---- Datos de origen

        public async Task<List<SalesResultTotal>> GetAllSalesNewAllRegiones(string Marca)
        {
            List<SalesResultTotal> ResultadoTotal = new List<SalesResultTotal>();
            string parametros = Constants.BASE_HERDEZ_API + "/Sales/GetAllSalesNewAllRegiones?Marca=" + Marca;

            try
            {
                Debug.WriteLine("== url: " + parametros);
                string content = await client.GetStringAsync(parametros);
                Debug.WriteLine("== result: " + content);
                List<Dictionary<string, object>> jresultOrigen = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(content);

                if (jresultOrigen.Count > 0)
                {
                    foreach (var item in jresultOrigen)
                    {
                        SalesResultTotal Respuesta = new SalesResultTotal();
                        Respuesta.Region = Encrypt.DesCifrado(item["Region"].ToString());
                        Respuesta.Distrito = Encrypt.DesCifrado(item["Distrito"].ToString());
                        Respuesta.Sucursal = Encrypt.DesCifrado(item["Sucursal"].ToString());
                        Respuesta.PorcentageFC = Encrypt.DesCifrado(item["PorcentageFC"].ToString());
                        Respuesta.PorcentageTicket = Encrypt.DesCifrado(item["PorcentageTicket"].ToString());
                        Respuesta.PorcentageVenta = Encrypt.DesCifrado(item["PorcentageVenta"].ToString());
                        Respuesta.PorcentageVisitas = Encrypt.DesCifrado(item["PorcentageVisitas"].ToString());
                        Respuesta.TicketPromedio = Encrypt.DesCifrado(item["TicketPromedio"].ToString());
                        Respuesta.TotalFC = Encrypt.DesCifrado(item["TotalFC"].ToString());
                        Respuesta.VentaActual = Encrypt.DesCifrado(item["VentaActual"].ToString());
                        Respuesta.VentaFC = Encrypt.DesCifrado(item["VentaFC"].ToString());
                        Respuesta.VisitasTotales = Encrypt.DesCifrado(item["VisitasTotales"].ToString());

                        ResultadoTotal.Add(Respuesta);
                    }
                }

                return ResultadoTotal;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("E" + e.Message);
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("E" + ex.Message);
                return null;
            }
        }

        public async Task<List<SalesResultTotal>> GetAllSalesNewAllDistritos(string Region, string Marca)
        {
            List<SalesResultTotal> ResultadoTotal = new List<SalesResultTotal>();
            Region = Region == "" ? "" : Encrypt.Cifrado(Region);
            Marca = Marca == "" ? "" : Encrypt.Cifrado(Marca);
            string parametros = Constants.BASE_HERDEZ_API + "/Sales/GetAllSalesNewAllDistritos?Region=" + Region
                + "&Marca=" + Marca;

            try
            {
                Debug.WriteLine("== url: " + parametros);
                string content = await client.GetStringAsync(parametros);
                Debug.WriteLine("== result: " + content);
                List<Dictionary<string, object>> jresultOrigen = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(content);

                if (jresultOrigen.Count > 0)
                {
                    foreach (var item in jresultOrigen)
                    {
                        SalesResultTotal Respuesta = new SalesResultTotal
                        {
                            Region = Encrypt.DesCifrado(item["Region"].ToString()),
                            Distrito = Encrypt.DesCifrado(item["Distrito"].ToString()),
                            Sucursal = Encrypt.DesCifrado(item["Sucursal"].ToString()),
                            PorcentageFC = Encrypt.DesCifrado(item["PorcentageFC"].ToString()),
                            PorcentageTicket = Encrypt.DesCifrado(item["PorcentageTicket"].ToString()),
                            PorcentageVenta = Encrypt.DesCifrado(item["PorcentageVenta"].ToString()),
                            PorcentageVisitas = Encrypt.DesCifrado(item["PorcentageVisitas"].ToString()),
                            TicketPromedio = Encrypt.DesCifrado(item["TicketPromedio"].ToString()),
                            TotalFC = Encrypt.DesCifrado(item["TotalFC"].ToString()),
                            VentaActual = Encrypt.DesCifrado(item["VentaActual"].ToString()),
                            VentaFC = Encrypt.DesCifrado(item["VentaFC"].ToString()),
                            VisitasTotales = Encrypt.DesCifrado(item["VisitasTotales"].ToString())
                        };

                        ResultadoTotal.Add(Respuesta);
                    }
                }

                return ResultadoTotal;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("E" + e.Message);
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("E" + ex.Message);
                return null;
            }        
        }

        public async Task<List<SalesResultTotal>> GetAllSalesNewAllSucursales(string Distrito, string Marca)
        {
            List<SalesResultTotal> ResultadoTotal = new List<SalesResultTotal>();
            Distrito = Distrito == "" ? "" : Encrypt.Cifrado(Distrito);
            Marca = Marca == "" ? "" : Encrypt.Cifrado(Marca);
            string parametros = Constants.BASE_HERDEZ_API + "/Sales/GetAllSalesNewAllSucursales?Distrito=" + Distrito
                + "&Marca=" + Marca;

            try
            {
                Debug.WriteLine("== url: " + parametros);
                string content = await client.GetStringAsync(parametros);
                Debug.WriteLine("== result: " + content);
                List<Dictionary<string, object>> jresultOrigen = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(content);

                if (jresultOrigen.Count > 0)
                {
                    foreach (var item in jresultOrigen)
                    {
                        SalesResultTotal Respuesta = new SalesResultTotal
                        {
                            Region = Encrypt.DesCifrado(item["Region"].ToString()),
                            Distrito = Encrypt.DesCifrado(item["Distrito"].ToString()),
                            Sucursal = Encrypt.DesCifrado(item["Sucursal"].ToString()),
                            PorcentageFC = Encrypt.DesCifrado(item["PorcentageFC"].ToString()),
                            PorcentageTicket = Encrypt.DesCifrado(item["PorcentageTicket"].ToString()),
                            PorcentageVenta = Encrypt.DesCifrado(item["PorcentageVenta"].ToString()),
                            PorcentageVisitas = Encrypt.DesCifrado(item["PorcentageVisitas"].ToString()),
                            TicketPromedio = Encrypt.DesCifrado(item["TicketPromedio"].ToString()),
                            TotalFC = Encrypt.DesCifrado(item["TotalFC"].ToString()),
                            VentaActual = Encrypt.DesCifrado(item["VentaActual"].ToString()),
                            VentaFC = Encrypt.DesCifrado(item["VentaFC"].ToString()),
                            VisitasTotales = Encrypt.DesCifrado(item["VisitasTotales"].ToString())
                        };

                        ResultadoTotal.Add(Respuesta);
                    }
                }

                return ResultadoTotal;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("E" + e.Message);
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
