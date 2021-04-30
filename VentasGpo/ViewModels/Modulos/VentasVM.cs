using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Xamarin.Forms;

using Microcharts;
using ChartEntry = Microcharts.Entry;
using AppNutOp.Models;
using VentasGpo.Portable.Helpers;

namespace VentasGpo.ViewModels.Modulos
{
    public class VentasVM : VMBase
    {
        public Command FilterCommand { get; }
        public Command CompsCommand { get; }

        public Command GetDetailsCommand { get; }
        public Command FilterPlaceCommand { get; }
        public Command FilterPlaceSearchCommand { get; }
        public Command CategoriasCommand { get; }

        public Command GetRegionesCommand { get; }
        public Command GetDistritosCommand { get; }
        public Command GetFranquiciasCommand { get; }
        public Command FindFranquiciasCommand { get; }

        public Command ReturnCommand { get; set; }

        readonly string perfil;
        readonly string nivel;
        readonly string user;
        readonly string marca_act;
        public EnumPerfil myPerfil;

        bool isiOS = false;
        bool isAndroid = false;
        int limitBarras = 0;

        List<DataPerfil> DataPerfil;
        List<SalesResultTotal> DataDetails;
        Sales DataSales;
        int CatActual = 1;

        List<SalesByPerfil> ListaGrafica;
        List<ChartEntry> entriesb = new List<ChartEntry>();
        List<ChartEntry> entriesl = new List<ChartEntry>();

        public string RegionChoice = string.Empty;
        public string DistritoChoice = string.Empty;
        public string TiendaChoice = string.Empty;

        public VentasVM()
        {
            ReturnCommand = new Command(() => ExecuteReturnCommand());

            FilterCommand = new Command( async () => await FilterPerfil());
            CompsCommand = new Command(async () => await FilterPerfilComp());
            GetDetailsCommand = new Command(async () => await GetDetails());
            FilterPlaceCommand = new Command(async () => await FilterPlace());
            FilterPlaceSearchCommand = new Command(async () => await FilterPlaceSearch());
            CategoriasCommand = new Command<int>(async (x) => await Categorias(x));

            GetRegionesCommand = new Command( () => GetRegiones() );
            GetDistritosCommand = new Command( () => GetDistritos() );
            GetFranquiciasCommand = new Command( () => GetFranquicias() );
            FindFranquiciasCommand = new Command( () => FindFranquicias() );

            perfil = Application.Current.Properties.ContainsKey("perfil") ? (string)Application.Current.Properties["perfil"] : "";
            nivel = Application.Current.Properties.ContainsKey("nivel") ? (string)Application.Current.Properties["nivel"] : "";
            user = Application.Current.Properties.ContainsKey("user") ? (string)Application.Current.Properties["user"] : "";
            marca_act = Application.Current.Properties.ContainsKey("marca_id") ? (string)Application.Current.Properties["marca_id"] : "";

            isAndroid = Device.RuntimePlatform == Device.Android;
            isiOS = Device.RuntimePlatform == Device.iOS;

            ListaRegiones = new List<string>();
            ListaDetalles = new ObservableCollection<object>();
            ListaGrafica = new List<SalesByPerfil>();

        }

        private void ExecuteReturnCommand()
        {
            MessagingCenter.Send(this, "ReturnActivated");
        }

        #region Filtros Lugares

        async Task FilterPerfil()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Title = "Ventas " + TitleBrand();

                await Task.Run(async () => {
                    DataPerfil = await SalesService.RecuperaDataByPerfil(int.Parse(perfil), user, marca_act);
                    FilterByPerfil(int.Parse(nivel));
                });

                await Task.Run(async () =>
                {
                    string tienda = TiendaChoice;
                    if (!string.IsNullOrEmpty(TiendaChoice))
                    {
                        tienda = FormatSucursalV2(TiendaChoice);
                    }
                    DataSales = await SalesService.GetAllSalesNew(RegionChoice, DistritoChoice, tienda, marca_act);

                    if (DataSales == null || DataSales.ListaNewData.Count == 0)
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {                    
                            await Message.Mensaje("Aviso", "No hay datos para la consulta actual");
                        });
                    }
                    else
                    {
                        ListaGrafica.Clear();
                        ListaGrafica = DataSales.ListaNewData;
                    }

                    Device.BeginInvokeOnMainThread(() => {
                        TouchDetails = false;
                        DataByPerfil();
                    });
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task FilterPerfilComp()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                await Task.Run(async () =>
                {
                    if (IsComp && string.IsNullOrEmpty(TiendaChoice))
                    {
                        DataSales = await SalesService.GetAllSalesNewCom(RegionChoice, DistritoChoice, TiendaChoice, marca_act);
                    }
                    else
                    {
                        IsSucursal = true;
                        IsDetails = true;
                        DataSales = await SalesService.GetAllSalesNew(RegionChoice, DistritoChoice, TiendaChoice, marca_act);
                    }

                    if (DataSales == null)
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await Message.Mensaje("Aviso", "No hay datos para la consulta actual");
                        });
                    }
                    else
                    {
                        ListaGrafica.Clear();
                        ListaGrafica = DataSales.ListaNewData;
                    }

                    Device.BeginInvokeOnMainThread(() => {
                        TouchDetails = false;
                        DataByPerfil();
                    });
                });

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task FilterPlace()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                await Task.Run(async () =>
                {
                    string tienda = TiendaChoice;
                    if (!string.IsNullOrEmpty(TiendaChoice))
                    {
                        tienda = FormatSucursalV2(TiendaChoice);

                        IsComp = false;
                        IsSucursal = false;
                        IsDetails = false;

                        DataSales = await SalesService.GetAllSalesNew(RegionChoice, DistritoChoice, tienda, marca_act);
                    }
                    else
                    {
                        if (IsComp)
                        {
                            DataSales = await SalesService.GetAllSalesNewCom(RegionChoice, DistritoChoice, tienda, marca_act);
                        }
                        else
                        {
                            IsSucursal = true;
                            IsDetails = true;
                            DataSales = await SalesService.GetAllSalesNew(RegionChoice, DistritoChoice, tienda, marca_act);
                        }
                    }

                    if (DataSales.ListaNewData.Count == 0)
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await Message.Mensaje("Aviso", "No hay datos para la consulta actual");
                        });
                    }
                    else
                    {
                        ListaGrafica.Clear();
                        ListaGrafica = DataSales.ListaNewData;
                    }

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        TouchDetails = false;
                        DataByPerfil();
                    });
                    
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task FilterPlaceSearch()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                await Task.Run(async () =>
                {
                    string tienda = TiendaChoice;
                    if (!string.IsNullOrEmpty(TiendaChoice))
                    {
                        tienda = FormatSucursalV2(TiendaChoice);

                        IsComp = false;
                        IsSucursal = false;
                        IsDetails = false;

                        await Task.WhenAll(
                            FindRegiones(tienda)
                        );

                        DataSales = await SalesService.GetAllSalesNew(RegionChoice, DistritoChoice, tienda, marca_act);
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await Message.Mensaje("Aviso", "No hay datos para la consulta actual");
                        });
                    }

                    if (DataSales.ListaNewData.Count == 0)
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await Message.Mensaje("Aviso", "No hay datos para la consulta actual");
                        });
                    }
                    else
                    {
                        ListaGrafica.Clear();
                        ListaGrafica = DataSales.ListaNewData;
                    }

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        TouchDetails = false;
                        DataByPerfil();
                    });

                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task Categorias(int cat)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                await Task.Run( () =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        CatActual = cat;
                        DataByPerfil();
                    });
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        void DataByPerfil()
        {
            Debug.WriteLine("R="+RegionChoice);
            Debug.WriteLine("D="+DistritoChoice);
            Debug.WriteLine("T="+TiendaChoice);

            LugarFirst = string.IsNullOrEmpty(TiendaChoice) ? "MÉXICO " : Leyendas(1) + " ";
            LugarSecond = string.IsNullOrEmpty(TiendaChoice) ? ((!string.IsNullOrEmpty(RegionChoice)) ? RegionChoice : "") : DistritoChoice;
            LugarThird = string.IsNullOrEmpty(TiendaChoice) ? ((!string.IsNullOrEmpty(DistritoChoice)) ? DistritoChoice : "") : TiendaChoice;

            switch (myPerfil)
            {
                case EnumPerfil.Nacional:
                    if(marca_act == "1")
                        FilterButtons = new object[] { 1, 1, 1, 1 };
                    else
                        FilterButtons = new object[] { 1, 0, 1, 1 };
                    break;
                case EnumPerfil.Regional:
                    FilterButtons = new object[]{ 0, 1, 1, 1 };
                    break;
                case EnumPerfil.Distrital:
                    FilterButtons = new object[] { 0, 0, 1, 1 };
                    break;
                case EnumPerfil.Tienda:
                    FilterButtons = new object[] { 0, 0, 0, 1 };

                    IsComp = false;
                    IsSucursal = false;
                    break;
                default:
                    break;
            }

            IsDetails = (string.IsNullOrEmpty(TiendaChoice)) ? true : false;
            IsGraphic = true;

            if (DataSales != null && DataSales.ListaNewData.Count > 0)
            {
                string aux;
                //categorias
                switch (CatActual) {
                    case 1:
                        Title = "Ventas " + TitleBrand();
                        aux = (DataSales.Resultado.VentaActual == "NaN") ? "0" : DataSales.Resultado.VentaActual;
                        Leyenda = "Venta actual: $" + double.Parse(aux).ToString("N2");
                        Porcentaje = double.Parse(DataSales.Resultado.PorcentageVenta).ToString("N2");

                        GraphVentas();
                        break;
                    case 2:
                        Title = "Visitas";
                        Leyenda = "Visitas total: " + DataSales.Resultado.VisitasTotales;
                        Porcentaje = double.Parse(DataSales.Resultado.PorcentageVisitas).ToString("N2");

                        GraphVisitas();
                        break;
                    case 3:
                        Title = "Ticket promedio";
                        aux = (DataSales.Resultado.TicketPromedio == "NaN") ? "0" : DataSales.Resultado.TicketPromedio;
                        Leyenda = "T. P.: " + double.Parse(aux).ToString("N2");
                        Porcentaje = double.Parse(DataSales.Resultado.PorcentageTicket).ToString("N2");

                        GraphTickets();
                        break;
                    case 4:
                        IsDetails = false;
                        IsGraphic = false;
                        
                        Title = "Presupuesto";
                        aux = (DataSales.Resultado.VentaFC == "NaN") ? "0" : DataSales.Resultado.VentaFC;
                        Leyenda = "Ppto: $" + double.Parse(aux).ToString("N2");
                        Porcentaje = double.Parse(DataSales.Resultado.PorcentageFC).ToString("N2");
                        break;
                }

                Porcentaje = (Porcentaje == "NaN") ? Porcentaje = "0.00" : Porcentaje;
                PorcentajeColor = (double.Parse(Porcentaje) > 0) ? Color.Green : Color.Red;
            }
            else
            {
                IsGraphic = false;

                Leyenda = string.Empty;
                Porcentaje = string.Empty;
                PorcentajeColor = Color.White;
            }
        }

        void FilterByPerfil(int perfilUser)
        {
            myPerfil = MyPerfil.GetPerfil(perfilUser);

            switch (myPerfil)
            {
                case EnumPerfil.Regional:
                    // Region nutrisa
                    RegionChoice = Application.Current.Properties.ContainsKey("depa") ? (string)Application.Current.Properties["depa"] : "";
                    break;
                case EnumPerfil.Distrital:
                    //distrital
                    RegionBrand();
                    DistritoChoice = Application.Current.Properties.ContainsKey("depa") ? (string)Application.Current.Properties["depa"] : "";
                    break;
                case EnumPerfil.Tienda:
                    //tienda
                    myPerfil = EnumPerfil.Tienda;
                    RegionBrand();

                    var listaDistrito = GetListaDistritos();
                    DistritoChoice = (listaDistrito.Count > 0) ? listaDistrito[0].Distritos.Trim() : string.Empty;

                    var listaTienda = GetListaSucursales();
                    string tienda = Application.Current.Properties.ContainsKey("depa") ? (string)Application.Current.Properties["depa"] : "";
                    var NombreNumeroTienda = listaTienda.Where(x => x.Tiendas == int.Parse(tienda)).Select(x => x.Tiendas.Value.ToString().Trim() + " - " + x.NombreTienda.Trim()).FirstOrDefault();
                    TiendaChoice = FormatSucursal(NombreNumeroTienda);
                    break;
                default:
                    // 7,10 nacional/regional lavazza-cielito
                    // 8 nacional nutrisa
                    // 4 director

                    if(marca_act != "1")
                        RegionBrand();
                    break;
            }
        }

        string TitleBrand()
        {
            string mm = "";
            switch (marca_act)
            {
                case "1":
                    mm = "Nutrisa";
                    break;
                case "2":
                    mm = "Lavazza";
                    break;
                case "3":
                    mm = "Cielito";
                    break;
                default:
                    break;
            }
            return mm;
        }

        void RegionBrand()
        {
            switch (marca_act)
            {
                case "1":
                    var listaRegion = GetListaRegiones();
                    RegionChoice = (listaRegion.Count > 0) ? listaRegion[0].Regiones.Trim() : string.Empty;
                    break;
                case "2":
                    RegionChoice = "LAVAZZA";
                    break;
                case "3":
                    RegionChoice = "CIELITO QUERIDO CAFÉ";
                    break;
                default:
                    break;
            }
        }

        string Leyendas(int lugar)
        {
            string choice;
            switch (lugar)
            {
                case 1:
                    if (marca_act == "3")
                    {
                        //cielito
                        choice = "CQC";
                    }
                    else
                    {
                        choice = RegionChoice;
                    }
                    break;
                default:
                    choice = "";
                    break;
            }

            return choice;
        }

        //Para mostrar en front
        string FormatSucursal(string tiendaCompleta)
        {
            string choice;
            if (marca_act == "3")
            {
                //cielito
                string[] split = tiendaCompleta.Split("-".ToCharArray());
                string tienda = split[0].TrimEnd();
                int idTienda = Convert.ToInt32(tienda) - 99999;
                choice = idTienda.ToString() + " - " + split[1].ToUpper().TrimEnd();
            }
            else
            {
                choice = tiendaCompleta.ToUpper();
            }

            return choice;
        }

        //Previo envio de servicio
        string FormatSucursalV2(string tiendaCompleta)
        {
            string[] split = tiendaCompleta.Split("-".ToCharArray());
            string tienda = split[0].TrimEnd();
            if (marca_act == "3")
            {
                //cielito
                int idTienda = Convert.ToInt32(tienda) + 99999;
                tienda = idTienda.ToString();
            }

            return tienda;
        }

        #endregion

        #region Detalles lugares

        async Task GetDetails()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                //List<SalesResultTotal> DataOrigenes = new List<SalesResultTotal>();
                ListaDetalles.Clear();

                await Task.Run(async () =>
                {
                    if (string.IsNullOrEmpty(RegionChoice))
                    {
                        if (TouchDetails == false)
                        {
                            DataDetails = await SalesService.GetAllSalesNewAllRegiones(marca_act);
                        }

                        DetailsType(DataDetails, 1);                     
                    }
                    else if (string.IsNullOrEmpty(DistritoChoice))
                    {
                        if (TouchDetails == false)
                        {
                            DataDetails = await SalesService.GetAllSalesNewAllDistritos(RegionChoice, marca_act);
                            //DataDetails.ForEach(ite => Debug.WriteLine(ite.Region + "-" + ite.Distrito));
                            DataDetails = DataDetails.Where(x => x.Region.Trim() == RegionChoice).ToList();
                        }
                        DetailsType(DataDetails, 2);
                    }
                    else if (string.IsNullOrEmpty(TiendaChoice))
                    {
                        if (TouchDetails == false)
                        {
                            DataDetails = await SalesService.GetAllSalesNewAllSucursales(DistritoChoice, marca_act);
                            //DataDetails.ForEach(ite => Debug.WriteLine(ite.Region + "-" + ite.Distrito));
                            DataDetails = DataDetails.Where(x => x.Region.Trim() == RegionChoice && x.Distrito.Trim() == DistritoChoice).ToList();
                        }
                        DetailsType(DataDetails, 3);
                    }

                    TouchDetails = true;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        void DetailsType(List<SalesResultTotal> DataOrigenes, int place)
        {
            SalesResultTotal[] OrigenesOrden = new SalesResultTotal[] { };

            switch (CatActual)
            {
                case 1:
                    OrigenesOrden = DataOrigenes.OrderByDescending(i => double.Parse(i.PorcentageVenta == "" || i.PorcentageVenta == "NaN" ? "0" : i.PorcentageVenta)).ToArray();
                    Device.BeginInvokeOnMainThread(() => {
                        GetVentas(OrigenesOrden, place);
                    });
                    break;
                case 2:
                    OrigenesOrden = DataOrigenes.OrderByDescending(i => double.Parse(i.PorcentageVisitas == "" || i.PorcentageVisitas == "NaN" ? "0" : i.PorcentageVisitas)).ToArray();
                    Device.BeginInvokeOnMainThread(() => {
                        GetVisitas(OrigenesOrden, place);
                    });
                    break;
                case 3:
                    OrigenesOrden = DataOrigenes.OrderByDescending(i => double.Parse(i.PorcentageTicket == "" || i.PorcentageTicket == "NaN" ? "0" : i.PorcentageTicket)).ToArray();
                    Device.BeginInvokeOnMainThread(() => {
                        GetTickets(OrigenesOrden, place);
                    });
                    break;
            }
        }

        void GetVentas(SalesResultTotal[] origens, int place)
        {
            foreach (SalesResultTotal item in origens)
            {
                var porc = (item.PorcentageVenta == "NaN") ? "0" : item.PorcentageVenta.Trim();

                ListaDetalles.Add(new
                {
                    Descripcion = ObtenerName(item, place),
                    Porcentaje = double.Parse(porc).ToString("N2"),
                    PorcentajeColor = item.PorcentageVenta.Contains("-") ? Color.Red : Color.Green
                });
            }

            Height = (ListaDetalles.Count * 30) + (ListaDetalles.Count * 10);
        }

        void GetVisitas(SalesResultTotal[] origens, int place)
        {
            foreach (SalesResultTotal item in origens)
            {
                var porc = (item.PorcentageVisitas == "NaN") ? "0" : item.PorcentageVisitas.Trim();

                ListaDetalles.Add(new
                {
                    Descripcion = ObtenerName(item, place),
                    Porcentaje = double.Parse(porc).ToString("N2"),
                    PorcentajeColor = item.PorcentageVisitas.Contains("-") ? Color.Red : Color.Green
                });
            }

            Height = (ListaDetalles.Count * 30) + (ListaDetalles.Count * 10);
        }

        void GetTickets(SalesResultTotal[] origens, int place)
        {
            foreach (SalesResultTotal item in origens)
            {
                var porc = (item.PorcentageTicket == "NaN") ? "0" : item.PorcentageTicket.Trim();

                ListaDetalles.Add(new
                {
                    Descripcion = ObtenerName(item, place),
                    Porcentaje = double.Parse(porc).ToString("N2"),
                    PorcentajeColor = item.PorcentageTicket.Contains("-") ? Color.Red : Color.Green
                });
            }

            Height = (ListaDetalles.Count * 30) + (ListaDetalles.Count * 10);
        }

        private string ObtenerName(SalesResultTotal item, int place)
        {
            if (place == 1)
            {
                return item.Region.Trim();
            }
            else if (place == 2)
            {
                return item.Distrito.Trim();
            }
            else {
                string s = item.Sucursal;
                bool result = int.TryParse(s, out int i);
                if (result == true)
                {
                    var listaTienda = GetListaSucursales();

                    var NombreNumeroTienda = listaTienda.Where(x => x.Tiendas == int.Parse(item.Sucursal)).Select(x => x.Tiendas.Value.ToString().Trim() + " - " + x.NombreTienda.Trim()).FirstOrDefault();
                    return NombreNumeroTienda;
                }
                else
                {
                    return FormatSucursal(item.Sucursal.Trim());
                }
            }
        }

        #endregion

        #region Listas de lugares

        void GetRegiones()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Debug.WriteLine(DataPerfil.Count);
                if (DataPerfil.Count > 0)
                {
                    var listaRegion = GetListaRegiones();

                    //Debug.WriteLine(JsonConvert.SerializeObject(listaRegion));
                    Debug.WriteLine(listaRegion.Count);

                    ListaRegiones.Clear();
                    foreach (DataPerfil item in listaRegion) 
                    {
                        if (!string.IsNullOrWhiteSpace(item.Regiones))
                        {
                            Debug.WriteLine(item.Regiones);
                            ListaRegiones.Add(item.Regiones);
                        }
                    }
 
                    Height = (ListaRegiones.Count * 20) + (ListaRegiones.Count * 10);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        void GetDistritos()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Debug.WriteLine(DataPerfil.Count);
                if (DataPerfil.Count > 0)
                {
                    var listaDistrito = GetListaDistritos();

                    //Debug.WriteLine(JsonConvert.SerializeObject(listaDistrito));
                    Debug.WriteLine(listaDistrito.Count);

                    ListaRegiones.Clear();
                    foreach (DataPerfil item in listaDistrito)
                    {
                        if (!string.IsNullOrWhiteSpace(item.Distritos))
                        {
                            Debug.WriteLine(item.Distritos + "|");
                            ListaRegiones.Add(item.Distritos.Trim());
                        }
                    }
                    
                    Height = (ListaRegiones.Count * 20) + (ListaRegiones.Count * 10);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        void GetFranquicias()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Debug.WriteLine(DataPerfil.Count);
                if (DataPerfil.Count > 0)
                {
                    var listaTienda = GetListaSucursales();

                    //Debug.WriteLine(JsonConvert.SerializeObject(listaTienda));
                    Debug.WriteLine(listaTienda.Count);

                    ListaRegiones.Clear();
                    foreach (DataPerfil item in listaTienda)
                    {
                        if (!string.IsNullOrWhiteSpace(item.NombreTienda))
                        {
                            //Debug.WriteLine(item.NombreTienda + "|");
                            ListaRegiones.Add(FormatSucursal(item.Tiendas + " - " + item.NombreTienda.Trim()));
                        }
                    }
                    Height = (ListaRegiones.Count * 20) + (ListaRegiones.Count * 10);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        void FindFranquicias()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Debug.WriteLine(TiendaSearch);
                if (DataPerfil.Count > 0 && !string.IsNullOrEmpty(TiendaSearch))
                {
                    var listaTienda = GetBusquedaSucursales();
                    Debug.WriteLine(listaTienda.Count);

                    ListaRegiones.Clear();
                    if (listaTienda.Count > 0)
                    {
                        foreach (DataPerfil item in listaTienda)
                        {
                            if (!string.IsNullOrWhiteSpace(item.NombreTienda))
                            {
                                Debug.WriteLine(item.NombreTienda + "|");
                                ListaRegiones.Add(FormatSucursal(item.Tiendas + " - " + item.NombreTienda.Trim()));
                            }
                        }
                    }
                    else
                    {
                        ListaRegiones.Add("Sin resultados");
                    }
                }
                else
                {
                    ListaRegiones.Add("Sin resultados");
                }

                Height = (ListaRegiones.Count * 20) + (ListaRegiones.Count * 10);
                MessagingCenter.Send(this, "LaLista", ListaRegiones);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task FindRegiones(string tiendaId)
        {
            try
            {
                if (DataPerfil.Count > 0)
                {
                    await Task.Run(() =>
                    {
                        var listaRegiones = GetBusquedaRegiones(tiendaId);
                        Debug.WriteLine(listaRegiones.Count);

                        ListaRegiones.Clear();
                        if (listaRegiones.Count > 0)
                        {
                            RegionChoice = listaRegiones[0].Regiones.Trim();
                            DistritoChoice = listaRegiones[0].Distritos.Trim();
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        List<DataPerfil> GetListaRegiones()
        {
            List<DataPerfil> listaRegion = (from p in DataPerfil
                                                         group p by p.Regiones into grupo
                                                         where grupo.Count() >= 1
                                                         select new DataPerfil()
                                                         {
                                                             Regiones = grupo.Key,
                                                         }).OrderBy(x => x.Regiones).ToList();
            listaRegion = listaRegion.Distinct().ToList();

            return listaRegion;
        }

        List<DataPerfil> GetListaDistritos()
        {
            List<DataPerfil> listaDistrito = (from p in DataPerfil
                                                           where p.Regiones.Trim() == RegionChoice
                                                           group p by p.Distritos into grupo
                                                           where grupo.Count() >= 1
                                                           select new DataPerfil()
                                                           {
                                                               Distritos = grupo.Key
                                                           }).OrderBy(x => x.Distritos).ToList();

            //listaDistrito = listaDistrito.Where(x => x.Regiones == RegionChoice).Distinct().ToList();
            listaDistrito = listaDistrito.Distinct().ToList();

            return listaDistrito;
        }

        List<DataPerfil> GetListaSucursales()
        {
            List<DataPerfil> listaTienda = (from p in DataPerfil
                                                         where p.Regiones.Trim() == RegionChoice
                                                         where p.Distritos.Trim() == DistritoChoice
                                                         group p by new { p.Tiendas, p.NombreTienda } into grupo
                                                         where grupo.Count() >= 1
                                                         select new DataPerfil()
                                                         {
                                                             Tiendas = grupo.Key.Tiendas,
                                                             NombreTienda = grupo.Key.NombreTienda
                                                         }).ToList();

            listaTienda = listaTienda.Distinct().ToList();
            return listaTienda;
        }

        List<DataPerfil> GetBusquedaSucursales()
        {
            List<DataPerfil> listaTienda = (from p in DataPerfil
                                                         where p.NombreTienda.ToUpper().Contains(TiendaSearch.ToUpper().TrimEnd())
                                                         group p by new { p.Tiendas, p.NombreTienda } into grupo
                                                         where grupo.Count() >= 1
                                                         select new DataPerfil()
                                                         {
                                                             Tiendas = grupo.Key.Tiendas,
                                                             NombreTienda = grupo.Key.NombreTienda,
                                                         }).ToList();

            listaTienda = listaTienda.Distinct().ToList();
            return listaTienda;
        }

        List<DataPerfil> GetBusquedaRegiones(string tiendaId)
        {
            List<DataPerfil> listaTienda = (from p in DataPerfil
                                                         where p.Tiendas == int.Parse(tiendaId)
                                                         group p by new { p.Distritos, p.Regiones } into grupo
                                                         where grupo.Count() >= 1
                                                         select new DataPerfil()
                                                         {
                                                             Distritos = grupo.Key.Distritos,
                                                             Regiones = grupo.Key.Regiones,
                                                         }).ToList();

            listaTienda = listaTienda.Distinct().ToList();
            return listaTienda;
        }

        #endregion

        #region Graficas

        void InitGraph()
        {
            ChartLine = new LineChart()
            {
                AnimationProgress = 6000,
                LineMode = LineMode.Straight,
                BackgroundColor = SkiaSharp.SKColor.Parse(Color.Transparent.ToHex()),
                LineAreaAlpha = 1,
                ValueLabelOrientation = Orientation.Vertical,
                LabelTextSize = 19,
            };

            ChartBar = new BarChart() {
                AnimationProgress = 3000,
                BackgroundColor = SkiaSharp.SKColor.Parse(Color.White.ToHex()),
                ValueLabelOrientation = Orientation.Horizontal,
                LabelTextSize = (isAndroid) ? 15 : 19,
            };

            limitBarras = (isiOS) ? 28 : 26;

            entriesb.Clear();
            entriesl.Clear();
        }

        void LastElementGraph(double valor, string graph)
        {
            var valueb = (graph == "l") ? valor : 0;
            var valuel = (graph == "l") ? 0 : valor;

            entriesb.Add(new ChartEntry(Convert.ToSingle(valueb))
            {
                Color = SkiaSharp.SKColor.Parse(Color.Transparent.ToHex()),
                TextColor = SkiaSharp.SKColor.Parse(Color.Transparent.ToHex()),
                ValueLabel = valueb.ToString()
            });
            entriesl.Add(new ChartEntry(Convert.ToSingle(valuel))
            {
                Color = SkiaSharp.SKColor.Parse(Color.Transparent.ToHex()),
                TextColor = SkiaSharp.SKColor.Parse(Color.Transparent.ToHex()),
                ValueLabel = valuel.ToString()
            });
        }

        void LimitBarras(bool isHoraCero)
        {
            //limitar barras en caso que año anterior tiene datos en hora 0
            if (isHoraCero)
            {
                var lim = limitBarras / 2;
                entriesb = entriesb.Skip(Math.Max(0, entriesb.Count() - lim)).ToList();
                entriesl = entriesl.Skip(Math.Max(0, entriesl.Count() - lim)).ToList();
            }
        }

        void GraphVentas()
        {
            InitGraph();

            var cb = ListaGrafica.OrderByDescending(i => i.VentaNeta).Where(e => e.Bandera == "0").First();
            var cl = ListaGrafica.OrderByDescending(i => i.VentaNeta).Where(e => e.Bandera == "1").First();
            int hora = 0;
            string max = (cl.VentaNeta >= cb.VentaNeta) ? "l" : "b";
            double maxValue = (max == "l") ? cl.VentaNeta : cb.VentaNeta;

            int c = 0;
            bool isHoraCero = false;
            foreach (SalesByPerfil item in ListaGrafica)
            {
                c++;
                if (item.Bandera == "1")
                {
                    //lineas
                    hora = item.Hora;
                    var bar = ListaGrafica.Where(i => i.Bandera == "0").Where(e => e.Hora == hora).First();
                    var div = Convert.ToDouble(bar.VentaNeta / 1000);
                    string valor = (div >= 1) ? div.ToString("N2") + "k" : bar.VentaNeta.ToString();

                    if(hora == 0 && item.VentaNeta > 0) isHoraCero = true;

                    entriesl.Add(new ChartEntry(Convert.ToSingle(item.VentaNeta))
                    {
                        Color = SkiaSharp.SKColor.Parse(Color.Red.ToHex()), //color grafica
                        ValueLabel = "  " + valor, //superior
                        TextColor = SkiaSharp.SKColor.Parse("#009ACC"), //color label
                    });

                    //Debug.WriteLine("L:"+ item.Hora.ToString()+"_"+item.VentaNeta);
                }
                else
                {
                    entriesb.Add(new ChartEntry(Convert.ToSingle(item.VentaNeta))
                    {
                        Color = SkiaSharp.SKColor.Parse("#009ACC"), //1ab2ff
                        ValueLabel = item.Hora.ToString(),
                        TextColor = SkiaSharp.SKColor.Parse("#9D9999")
                    });
                    //Debug.WriteLine("B:" + item.Hora.ToString()+"_"+item.VentaNeta);
                }

                if (c >= limitBarras && isHoraCero == false)
                    break;
            }

            //año anterior hora 0
            LimitBarras(isHoraCero);

            //ultimo elemento para nivelar barras con lineas
            LastElementGraph(maxValue, max);

            Debug.WriteLine("CountBarra-" + entriesb.Count);
            Debug.WriteLine("CountLine-" + entriesl.Count);

            ChartLine.Entries = entriesl;
            ChartBar.Entries = entriesb;
        }

        void GraphVisitas()
        {
            InitGraph();

            var cb = ListaGrafica.OrderByDescending(i => i.Visitas).Where(e => e.Bandera == "0").First();
            var cl = ListaGrafica.OrderByDescending(i => i.Visitas).Where(e => e.Bandera == "1").First();
            int hora = 0;
            string max = (cl.Visitas >= cb.Visitas) ? "l" : "b";
            double maxValue = (max == "l") ? cl.Visitas : cb.Visitas;

            int c = 0;
            bool isHoraCero = false;
            foreach (SalesByPerfil item in ListaGrafica)
            {
                c++;
                if (item.Bandera == "1")
                {
                    hora = item.Hora;
                    var bar = ListaGrafica.Where(i => i.Bandera == "0").Where(e => e.Hora == hora).First();
                    var div = Convert.ToDouble(bar.Visitas / 1000);
                    string valor = (div >= 1) ? div.ToString("N2") + "k" : bar.Visitas.ToString();

                    if (hora == 0 && item.Visitas > 0) isHoraCero = true;

                    entriesl.Add(new ChartEntry(Convert.ToSingle(item.Visitas))
                    {
                        Color = SkiaSharp.SKColor.Parse(Color.Red.ToHex()),
                        ValueLabel = "  " + valor, //superior
                        TextColor = SkiaSharp.SKColor.Parse("#009ACC"),
                    });

                    //Debug.WriteLine("L:" + item.Hora.ToString() + "_" + item.Visitas);
                }
                else
                {
                    entriesb.Add(new ChartEntry(Convert.ToSingle(item.Visitas))
                    {
                        Color = SkiaSharp.SKColor.Parse("#009ACC"), //1ab2ff
                        ValueLabel = item.Hora.ToString(),
                        TextColor = SkiaSharp.SKColor.Parse("#9D9999")
                    });
                    //Debug.WriteLine("B:" + item.Hora.ToString() + "_" + item.Visitas);
                }

                if (c >= limitBarras && isHoraCero == false)
                    break;
            }

            //año anterior hora 0
            LimitBarras(isHoraCero);

            //ultimo elemento para nivelas barras con lineas
            LastElementGraph(maxValue, max);

            Debug.WriteLine("CountBarra-" + entriesb.Count);
            Debug.WriteLine("CountLine-" + entriesl.Count);

            ChartLine.Entries = entriesl;
            ChartBar.Entries = entriesb;
        }

        void GraphTickets()
        {
            InitGraph();

            var cb = ListaGrafica.OrderByDescending(i => i.TicketPromedio).Where(e => e.Bandera == "0").First();
            var cl = ListaGrafica.OrderByDescending(i => i.TicketPromedio).Where(e => e.Bandera == "1").First();
            int hora = 0;
            string max = (cl.TicketPromedio >= cb.TicketPromedio) ? "l" : "b";
            double maxValue = (max == "l") ? cl.TicketPromedio : cb.TicketPromedio;

            int c = 0;
            bool isHoraCero = false;
            foreach (SalesByPerfil item in ListaGrafica)
            {
                c++;
                if (item.Bandera == "1")
                {
                    hora = item.Hora;
                    var bar = ListaGrafica.Where(i => i.Bandera == "0").Where(e => e.Hora == hora).First();
                    var div = Convert.ToDouble(bar.TicketPromedio / 1000);
                    string valor = (div >= 1) ? div.ToString("N2") + "k" : bar.TicketPromedio.ToString("N2");

                    if (hora == 0 && item.TicketPromedio > 0) isHoraCero = true;

                    entriesl.Add(new ChartEntry(Convert.ToSingle(item.TicketPromedio))
                    {
                        Color = SkiaSharp.SKColor.Parse(Color.Red.ToHex()),
                        ValueLabel = "  " + valor, //superior
                        TextColor = SkiaSharp.SKColor.Parse("#009ACC"),
                    });

                    //Debug.WriteLine("L:" + item.Hora.ToString() + "_" + item.TicketPromedio);
                }
                else
                {
                    entriesb.Add(new ChartEntry(Convert.ToSingle(item.TicketPromedio))
                    {
                        Color = SkiaSharp.SKColor.Parse("#009ACC"), //1ab2ff
                        ValueLabel = item.Hora.ToString(),
                        TextColor = SkiaSharp.SKColor.Parse("#9D9999")
                    });
                    //Debug.WriteLine("B:" + item.Hora.ToString() + "_" + item.TicketPromedio);
                }

                if (c >= limitBarras && isHoraCero == false)
                    break;
            }

            //año anterior hora 0
            LimitBarras(isHoraCero);

            //ultimo elemento para nivelas barras con lineas
            LastElementGraph(maxValue, max);

            Debug.WriteLine("CountBarra-" + entriesb.Count);
            Debug.WriteLine("CountLine-" + entriesl.Count);

            ChartLine.Entries = entriesl;
            ChartBar.Entries = entriesb;
        }

        #endregion

        #region variables

        ObservableCollection<object> lalista;
        public ObservableCollection<object> ListaDetalles
        {
            get { return lalista; }
            set
            {
                lalista = value;
                OnPropertyChanged();
            }
        }

        Chart cb;
        public Chart ChartBar
        {
            get { return cb; }
            set
            {
                cb = value;
                OnPropertyChanged();
            }
        }

        Chart cl;
        public Chart ChartLine
        {
            get { return cl; }
            set
            {
                cl = value;
                OnPropertyChanged();
            }
        }

        // -- List Filtros

        List<string> lalista2;
        public List<string> ListaRegiones
        {
            get { return lalista2; }
            set
            {
                lalista2 = value;
                OnPropertyChanged();
            }
        }

        int height;
        public int Height
        {
            get { return height; }
            set
            {
                height = value;
                OnPropertyChanged();
            }
        }

        string search;
        public string TiendaSearch
        {
            get { return search; }
            set
            {
                search = value;
                OnPropertyChanged();
            }
        }

        // -- Labels Place

        string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged();
            }
        }

        string name1;
        public string LugarFirst
        {
            get { return name1; }
            set
            {
                name1 = value;
                OnPropertyChanged();
            }
        }

        string name2;
        public string LugarSecond
        {
            get { return name2; }
            set
            {
                name2 = value;
                OnPropertyChanged();
            }
        }

        string name3;
        public string LugarThird
        {
            get { return name3; }
            set
            {
                name3 = value;
                OnPropertyChanged();
            }
        }

        // -- Labels Total

        string lab;
        public string Leyenda
        {
            get { return lab; }
            set
            {
                lab = value;
                OnPropertyChanged();
            }
        }

        string por;
        public string Porcentaje
        {
            get { return por; }
            set
            {
                por = value;
                OnPropertyChanged();
            }
        }

        Color porc = Color.White;
        public Color PorcentajeColor
        {
            get { return porc; }
            set
            {
                porc = value;
                OnPropertyChanged();
            }
        }

        // -- Buttons

        bool graph = true;
        public bool IsGraphic
        {
            get { return graph; }
            set
            {
                graph = value;
                OnPropertyChanged();
            }
        }

        object[] ff;
        public object[] FilterButtons
        {
            get { return ff; }
            set
            {
                ff = value;
                OnPropertyChanged();
            }
        }

        // -- Comparables

        bool comp = false;
        public bool IsComp
        {
            get { return comp; }
            set
            {
                comp = value;
                OnPropertyChanged();
            }
        }

        // -- Lectura tienda

        bool suc = true;
        public bool IsSucursal
        {
            get { return suc; }
            set
            {
                suc = value;
                OnPropertyChanged();
            }
        }

        // -- Mostrar barra detalles

        bool det = true;
        public bool IsDetails
        {
            get { return det; }
            set
            {
                det = value;
                OnPropertyChanged();
            }
        }

        // -- Vio detalles

        bool deta = false;
        public bool TouchDetails
        {
            get { return deta; }
            set
            {
                deta = value;
                OnPropertyChanged();
            }
        }

        bool fil = false;
        public bool ChangeFilter
        {
            get { return fil; }
            set
            {
                fil = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}
