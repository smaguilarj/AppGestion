using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Xamarin.Forms;

using Microcharts;
using ChartEntry = Microcharts.Entry;

using AppNutOp.Models;
using VentasGpo.Portable.Helpers;

namespace VentasGpo.ViewModels.Modulos
{
    public class ActividadesVM : VMBase
    {
        public Command ActividadesCommand { get; }
        public Command GetActividadCommand { get; }
        public Command FilterPlaceCommand { get; }

        public Command GetRegionesCommand { get; }
        public Command GetDistritosCommand { get; }

        public Command GetFechasDisponibles { get; }

        public Command ReturnCommand { get; set; }

        string MyMarca = string.Empty;
        string MyRegion = string.Empty;
        string MyDistrito = string.Empty;
        string MiPerfil = string.Empty;
        string MiNivel = string.Empty;
        string User;

        public string RegionChoice = string.Empty;
        public string DistritoChoice = string.Empty;
        public string TiendaChoice = string.Empty;
        public string FechaChoice = string.Empty;

        List<ChartEntry> entriesp = new List<ChartEntry>();
        List<ChartEntry> entriesd = new List<ChartEntry>();

        bool NameLargo = false;
        int CatActual = 1;
        string[] Colores = { "#3CC8F9", "#16CFB3", "#951DF2", "#F5F222", "#3676D9", "#C836D9", "#F89225", "#97D300", "#2A2793", "#FD73C7",
                             "#3CC8F9", "#16CFB3", "#951DF2", "#F5F222", "#3676D9", "#C836D9", "#F89225", "#97D300", "#2A2793", "#FD73C7"};
        List<Activities> ListaActividades;
        List<Tienda> ListaTiendasFiltro;
        List<DataPerfil> DataPerfil;

        public EnumPerfil myPerfil;
        public Activities ActividadChoice;
        public int IdActividadChoice;

        DateTime FechaHoy;
        DateTime LimitFechaMax;
        DateTime LimitFechaMin;

        public ActividadesVM()
        {
            ReturnCommand = new Command(() => ExecuteReturnCommand());

            ActividadesCommand = new Command(async () => await ListarActividades());
            GetActividadCommand = new Command<int>(async (x) => await GetActividad(x));
            FilterPlaceCommand = new Command(async () => await FilterPlace());

            GetRegionesCommand = new Command(() => GetRegiones());
            GetDistritosCommand = new Command(() => GetDistritos());

            GetFechasDisponibles = new Command( () => GetFechas());

            User = Application.Current.Properties.ContainsKey("user") ? (string)Application.Current.Properties["user"] : "";
            MiPerfil = Application.Current.Properties.ContainsKey("perfil") ? (string)Application.Current.Properties["perfil"] : "";
            MiNivel = Application.Current.Properties.ContainsKey("nivel") ? (string)Application.Current.Properties["nivel"] : "";
            MyMarca = Application.Current.Properties.ContainsKey("marca_id") ? (string)Application.Current.Properties["marca_id"] : "";

            ListaActividades = new List<Activities>();
            ListaTiendasFiltro = new List<Tienda>();
            ListaDetalles = new List<AuxDetalles>();
            ListaLugares = new List<string>();
            ListaFechas = new ObservableCollection<object>();

            FechaHoy = DateTime.Now;
            LimitFechaMax = FechaHoy.AddDays(8);
            LimitFechaMin = FechaHoy.AddDays(-8);
            FechaChoice = FechaHoy.ToString("yyyy-MM-dd");

            ActividadChoice = new Activities();
            IdActividadChoice = 0;
            FechaActividad = string.Empty;
        }

        private void ExecuteReturnCommand()
        {
            MessagingCenter.Send(this, "ReturnActivated");
        }

        async Task ListarActividades()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                CatActual = 1;

                await Task.Run(async () => {
                    DataPerfil = await SalesService.RecuperaDataByPerfil(int.Parse(MiPerfil), User, MyMarca);
                    FilterByPerfil(int.Parse(MiNivel));
                });

                await Task.Run(async () => {
                    ListaActividades = await ActivitiesService.GetAllActivities(MyMarca, MyRegion, MyDistrito, FechaChoice);
                    if (ListaActividades != null)
                        ListaActividades.ForEach(ite => Debug.WriteLine(ite.Id + "-" + ite.Nombre));
                    else
                        ListaActividades = new List<Activities>();

                    Device.BeginInvokeOnMainThread(() => {
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

                await Task.Run(() => {

                    FiltrarTiendas();

                    Device.BeginInvokeOnMainThread(() => {
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
            LugarFirst = string.IsNullOrEmpty(TiendaChoice) ? "MÉXICO " : RegionChoice + " ";
            LugarSecond = string.IsNullOrEmpty(TiendaChoice) ? ((!string.IsNullOrEmpty(RegionChoice)) ? RegionChoice : "") : DistritoChoice;
            LugarThird = string.IsNullOrEmpty(TiendaChoice) ? ((!string.IsNullOrEmpty(DistritoChoice)) ? DistritoChoice : "") : TiendaChoice;

            switch (myPerfil)
            {
                case EnumPerfil.Nacional:
                    if (MyMarca == "1")
                        FilterButtons = new object[] { 1, 1, 1 };
                    else
                        FilterButtons = new object[] { 1, 0, 1 };
                    break;
                case EnumPerfil.Regional:
                    FilterButtons = new object[] { 0, 1, 1 };
                    break;
                case EnumPerfil.Distrital:
                    FilterButtons = new object[] { 0, 0, 1 };
                    break;
                default:
                    break;
            }

            string fecha;
            //double porc = 0;
            //AuxLugar auxLugar;

            //modo de vista en seccion
            switch (CatActual)
            {
                case 1:
                    //general, todas actividades
                    Title = "Actividades";
                    //auxLugar = GetDataLugar(ListaActividades);
                    //porc = GetPorcentaje(auxLugar.NumTiendasDone, auxLugar.NumTiendasTarget);

                    FechaActividad = "Actividades del día: " + ListaActividades.Count;

                    IsActividad = false;
                    break;
                case 2:
                    //toco una actividad
                    Title = ActividadChoice.Nombre;
                    ListaTiendasFiltro.Clear();
                    ListaTiendasFiltro = ActividadChoice.Tiendas.Where(p => p.IdMarca == Convert.ToInt32(MyMarca)).ToList();
                    //auxLugar = GetDataLugar(ListaTiendasFiltro, ActividadChoice.Usuarios);
                    //porc = GetPorcentaje(auxLugar.NumTiendasDone, auxLugar.NumTiendasTarget);
                    
                    fecha = (string.Equals(ActividadChoice.FechaInicio, ActividadChoice.FechaTermino)) ? ActividadChoice.FechaTermino : ActividadChoice.FechaInicio + " a " + ActividadChoice.FechaTermino;
                    FechaActividad = "Fecha de entrega: " + fecha;

                    IsActividad = true;
                    break;
                case 3:
                    //toco filtro de lugares
                    Title = ActividadChoice.Nombre;
                    //auxLugar = GetDataLugar(ListaTiendasFiltro, ActividadChoice.Usuarios);
                    //porc = GetPorcentaje(auxLugar.NumTiendasDone, auxLugar.NumTiendasTarget);

                    fecha = (string.Equals(ActividadChoice.FechaInicio, ActividadChoice.FechaTermino)) ? ActividadChoice.FechaTermino : ActividadChoice.FechaInicio + " a " + ActividadChoice.FechaTermino;
                    FechaActividad = "Fecha de entrega: " + fecha;

                    IsActividad = true;
                    break;
            }

            GetListaDetails();
            IsAllActividad = !IsActividad;
            //Porcentaje = porc.ToString("N2");
            //PorcentajeColor = (porc > 0) ? Color.Green : Color.Red;

        }

        void FilterByPerfil(int perfilUser)
        {
            myPerfil = MyPerfil.GetPerfil(perfilUser);

            switch (myPerfil)
            {
                case EnumPerfil.Regional:
                    // Region nutrisa
                    MyRegion = Application.Current.Properties.ContainsKey("depa") ? (string)Application.Current.Properties["depa"] : "";
                    RegionChoice = MyRegion;
                    break;
                case EnumPerfil.Distrital:
                    //distrital
                    RegionBrand();
                    MyDistrito = Application.Current.Properties.ContainsKey("depa") ? (string)Application.Current.Properties["depa"] : "";
                    DistritoChoice = MyDistrito;
                    break;
                case EnumPerfil.Tienda:
                    //tienda
                    myPerfil = EnumPerfil.Tienda;
                    break;
                default:
                    // 7,10 nacional/regional lavazza-cielito
                    // 8 nacional nutrisa
                    // 4 director
                    if (MyMarca != "1")
                        RegionBrand();
                    break;
            }
        }

        async Task GetActividad(int act)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                await Task.Run(() =>
                {
                    CatActual = 2;
                    ActividadChoice = GetActividadOfLista(act);
                    IdActividadChoice = ActividadChoice.Id;

                    if(myPerfil == EnumPerfil.Regional || myPerfil == EnumPerfil.Distrital)
                    {
                        FiltrarTiendas();
                    }
                    else
                    {
                        RegionChoice = string.Empty;
                        DistritoChoice = string.Empty;
                        TiendaChoice = string.Empty;

                        if (MyMarca != "1")
                            RegionBrand();
                    }

                    Device.BeginInvokeOnMainThread(() => {
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

        void FiltrarTiendas()
        {
            CatActual = 3;

            ListaTiendasFiltro.Clear();
            ListaTiendasFiltro = ActividadChoice.Tiendas.Where(p => p.Region == RegionChoice).ToList();
            if (!string.IsNullOrEmpty(DistritoChoice))
            {
                ListaTiendasFiltro = ListaTiendasFiltro.Where(x => x.Distrito == DistritoChoice).ToList();
            }

            /*if (!string.IsNullOrEmpty(TiendaChoice))
            {
                string[] split = TiendaChoice.Split("-".ToCharArray());
                int id_tienda = Convert.ToInt32(split[0].TrimEnd());
                ListaTiendasFiltro = ListaTiendasFiltro.Where(y => y.Id == id_tienda).ToList();
            }*/
        }

        void GetFechas()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                ListaFechas.Clear();
                for (var i = LimitFechaMin; i <= LimitFechaMax; i = i.AddDays(1))
                {
                    string nombre = "";
                    if (i == FechaHoy)
                    {
                        nombre = "hoy";
                    }
                    else if ((i - FechaHoy).TotalDays == 1)
                    {
                        nombre = "mañana";
                    }
                    else if ((FechaHoy - i).TotalDays == 1)
                    {
                        nombre = "ayer";
                    }
                    else
                    {
                        var dia = Thread.CurrentThread.CurrentUICulture.DateTimeFormat.GetDayName(i.DayOfWeek);
                        nombre = dia;
                    }

                    ListaFechas.Add(new {
                        Dia = i.ToString("dd"),
                        DiaColor = (i == FechaHoy) ? Color.White.ToHex() : Color.Black.ToHex(),
                        DiaName = nombre,
                        Mes = Thread.CurrentThread.CurrentUICulture.DateTimeFormat.GetMonthName(i.Month) + " " + i.Year,
                        Hoy = (i == FechaHoy) ? "#E25050" : "#CBCAC8",
                        FechaSelect = i.ToString("MM-dd-yyyy")
                    });
                }

                Height = (ListaFechas.Count * 20) + (ListaFechas.Count * 10);
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

        void GetListaDetails()
        {
            ListaDetalles.Clear();
            GetListaPorcentaje();
        }

        #region Operaciones auxiliares

        void RegionBrand()
        {
            switch (MyMarca)
            {
                case "1":
                    var listaRegion = GetListaRegiones();
                    MyRegion = (listaRegion.Count > 0) ? listaRegion[0].Regiones.Trim() : string.Empty;
                    break;
                case "2":
                    MyRegion = "LAVAZZA";
                    break;
                case "3":
                    MyRegion = "CIELITO QUERIDO CAFÉ";
                    break;
                default:
                    break;
            }

            RegionChoice = MyRegion;
        }

        //Para mostrar en front
        string FormatSucursal(string tiendaCompleta)
        {
            string choice;
            if (MyMarca == "3")
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

        Activities GetActividadOfLista(int IdActividad)
        {
            Activities activities = ListaActividades.Where(p => p.Id == IdActividad).SingleOrDefault();

            return activities;
        }

        void GetListaPorcentaje()
        {
            NameLargo = false;

            if (IdActividadChoice != 0)
            {
                if (string.IsNullOrEmpty(RegionChoice))
                {
                    var list = GetLugaresRegion(ActividadChoice.Tiendas, ActividadChoice.Usuarios);
                    var lugares = SumarLugares(new List<object>(list));

                    LoadDetalles(lugares);
                }
                else if (string.IsNullOrEmpty(DistritoChoice))
                {
                    var list = GetLugaresDistrito(ListaTiendasFiltro, ActividadChoice.Usuarios);
                    var lugares = SumarLugares(new List<object>(list));

                    LoadDetalles(lugares);
                }
                else if (string.IsNullOrEmpty(TiendaChoice))
                {
                    var list = GetLugaresSucursal(ListaTiendasFiltro, ActividadChoice.Usuarios);
                    var lugares = SumarLugares(new List<object>(list));

                    LoadDetalles(lugares, true);
                }
            }
            else
            {
                foreach (Activities item in ListaActividades)
                {
                    var listtiendas = item.Tiendas.Where(p => p.IdMarca == Convert.ToInt32(MyMarca)).ToList();
                    if (myPerfil == EnumPerfil.Regional)
                    {
                        listtiendas = listtiendas.Where(p => p.Region == RegionChoice).ToList();
                    }
                    else if (myPerfil == EnumPerfil.Distrital)
                    {
                        listtiendas = listtiendas.Where(p => p.Region == RegionChoice &&
                            p.Distrito == DistritoChoice).ToList();
                    }

                    var lugares = GetDataLugar(listtiendas, item.Usuarios);
                    double porc = GetPorcentaje(lugares.NumTiendasDone, lugares.NumTiendasTarget);

                    Color color = Color.FromHex("DCBA00");
                    if (porc == 100) color = Color.Green;
                    if (porc < 90) color = Color.Red;

                    if(item.Nombre.Length > 21) { NameLargo = true; }

                    ListaDetalles.Add(new AuxDetalles()
                    {
                        IdActividad = item.Id,
                        Descripcion = item.Nombre,
                        Porcentaje = porc,
                        PorcentajeTxt = porc.ToString("N2") + "%",
                        PorcentajeColor = color
                    });
                }
            }

            int val_height = (NameLargo) ? 45 : 30;
            ListaDetalles = ListaDetalles.OrderByDescending(i => i.Porcentaje).ToList();
            Height = (ListaDetalles.Count * val_height) + (ListaDetalles.Count * 14);
        }

        void LoadDetalles(List<AuxLugar> lugares, bool isTienda = false)
        {
            foreach (AuxLugar item in lugares)
            {
                double porc = GetPorcentaje(item.NumTiendasDone, item.NumTiendasTarget);
                string porcentaje = porc.ToString("N2") + "%";

                Color color = Color.FromHex("DCBA00");
                if (porc == 100) color = Color.Green;
                if (porc < 90 ) color = Color.Red;

                if (isTienda)
                {
                    porcentaje = porc == 100 ? "✓" : "✘";
                }
                if (item.Nombre.Length > 21) { NameLargo = true; }

                ListaDetalles.Add(new AuxDetalles()
                {
                    IdActividad = 0,
                    Descripcion = item.Nombre.TrimEnd(),
                    Porcentaje = porc,
                    PorcentajeTxt = porcentaje,
                    PorcentajeColor = color
                });
            }
        }

        double GetPorcentaje(double done, double target)
        {
            var porc = (target != 0) ? (Convert.ToDouble(done) * 100) / Convert.ToDouble(target) : 0;
            return Math.Round(porc, 2);
        }

        AuxLugar GetDataLugar(List<Tienda> DataTiendas, List<UserActividad> DataUsers)
        {
            int auxDone = 0, auxTarget = 0;
            auxTarget = DataTiendas.Count;

            if (DataUsers != null && DataUsers.Count > 0)
            {
                foreach (Tienda tienda1 in DataTiendas)
                {
                    var users = DataUsers.Where(p => p.IdTienda == tienda1.Id).SingleOrDefault();
                    if (users != null)
                        auxDone++;
                }
            }

            var al = new AuxLugar
            {
                Nombre = "",
                NumTiendasDone = auxDone,
                NumTiendasTarget = auxTarget
            };

            return al;
        }

        AuxLugar GetDataLugar(List<Activities> DataActividades)
        {
            int auxDone = 0, auxTarget = 0;
            foreach (Activities activities in DataActividades)
            {
                var lugares = GetDataLugar(activities.Tiendas, activities.Usuarios);

                auxDone += lugares.NumTiendasDone;
                auxTarget += lugares.NumTiendasTarget;
            }

            var al = new AuxLugar
            {
                Nombre = "",
                NumTiendasDone = auxDone,
                NumTiendasTarget = auxTarget
            };

            return al;
        }

        #endregion


        #region Operaciones Extras

        ObservableCollection<object> GetLugaresRegion(List<Tienda> DataTiendas, List<UserActividad> DataUsers)
        {
            int auxDone = 0, auxTarget = 0;
            ObservableCollection<object> lista = new ObservableCollection<object>();

            var Regiones = GetListaRegiones(DataTiendas);
            foreach (Tienda tienda in Regiones)
            {
                auxDone = 0;
                var tiendas = GetListaTiendas(DataTiendas, tienda.Region);
                auxTarget = tiendas.Count;

                if (DataUsers != null && DataUsers.Count > 0)
                {
                    foreach (Tienda tienda1 in tiendas)
                    {
                        var users = DataUsers.Where(p => p.IdTienda == tienda1.Id).SingleOrDefault();
                        if (users != null)
                            auxDone++;
                    }
                }

                lista.Add(new
                {
                    Nombre = tienda.Region,
                    NumTiendasDone = auxDone,
                    NumTiendasTarget = auxTarget
                });
            }

            return lista;
        }

        ObservableCollection<object> GetLugaresDistrito(List<Tienda> DataTiendas, List<UserActividad> DataUsers)
        {
            int auxDone = 0, auxTarget = 0;
            ObservableCollection<object> lista = new ObservableCollection<object>();

            var Distritos = GetListaDistritos(DataTiendas);
            foreach (Tienda tienda in Distritos)
            {
                auxDone = 0;
                var tiendas = GetListaTiendas(DataTiendas, tienda.Region, tienda.Distrito);
                auxTarget = tiendas.Count;
                //tiendas.ForEach(ite => Debug.WriteLine(ite.Id + "-" + ite.Nombre));

                if (DataUsers != null && DataUsers.Count > 0)
                {
                    foreach (Tienda tienda1 in tiendas)
                    {
                        var users = DataUsers.Where(p => p.IdTienda == tienda1.Id).SingleOrDefault();
                        if (users != null)
                            auxDone++;
                    }
                }

                lista.Add(new
                {
                    Nombre = tienda.Distrito,
                    NumTiendasDone = auxDone,
                    NumTiendasTarget = auxTarget
                });
            }

            return lista;
        }

        ObservableCollection<object> GetLugaresSucursal(List<Tienda> DataTiendas, List<UserActividad> DataUsers)
        {
            int auxDone = 0, auxTarget = 0;
            ObservableCollection<object> lista = new ObservableCollection<object>();

            var Sucursales = GetListaSucursales(DataTiendas);
            auxTarget = 1;
            foreach (Tienda tienda in Sucursales)
            {
                auxDone = 0;
                if (DataUsers != null && DataUsers.Count > 0)
                {
                    var users = DataUsers.Where(p => p.IdTienda == tienda.Id).SingleOrDefault();
                    if (users != null)
                        auxDone++;
                }

                lista.Add(new
                {
                    Nombre = FormatSucursal(tienda.Id + " - " + tienda.Nombre),
                    NumTiendasDone = auxDone,
                    NumTiendasTarget = auxTarget
                });
            }

            return lista;
        }

        List<AuxLugar> SumarLugares(List<object> lista)
        {
            //Debug.WriteLine("tod--" + lista.Count);
            List<AuxLugar> lugars = new List<AuxLugar>();

            foreach (object item in lista)
            {
                var json = JsonConvert.SerializeObject(item);
                var aux = JsonConvert.DeserializeObject<AuxLugar>(json);

                var tmp = lugars.Where(p => p.Nombre == aux.Nombre).SingleOrDefault();
                if (tmp != null)
                {
                    foreach (var mc in lugars.Where(x => x.Nombre == aux.Nombre))
                    {
                        mc.NumTiendasDone += aux.NumTiendasDone;
                        mc.NumTiendasTarget += aux.NumTiendasTarget;
                    }
                }
                else
                {
                    lugars.Add(aux);
                }
            }

            //Debug.WriteLine(lugars.Count);
            //lugars.ForEach(ite => Debug.WriteLine(ite.Nombre + "-" + ite.NumTiendasTarget + " " +ite.NumTiendasDone));

            return lugars;
        }

        List<Tienda> GetListaTiendas(List<Tienda> DataTiendas, string Region)
        {
            List<Tienda> listaRegion = (from p in DataTiendas
                                        where p.Region.Trim() == Region
                                        group p by new { p.Id, p.Nombre } into grupo
                                        where grupo.Count() >= 1
                                        select new Tienda()
                                        {
                                            Id = grupo.Key.Id,
                                            Nombre = grupo.Key.Nombre
                                        }).OrderBy(x => x.Region).Distinct().ToList();

            return listaRegion;
        }

        List<Tienda> GetListaTiendas(List<Tienda> DataTiendas, string Region, string Distrito)
        {
            List<Tienda> listaRegion = (from p in DataTiendas
                                        where p.Region.Trim() == Region
                                        where p.Distrito == Distrito
                                        group p by new { p.Id, p.Nombre } into grupo
                                        where grupo.Count() >= 1
                                        select new Tienda()
                                        {
                                            Id = grupo.Key.Id,
                                            Nombre = grupo.Key.Nombre
                                        }).OrderBy(x => x.Region).Distinct().ToList();

            return listaRegion;
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
                Debug.WriteLine(ActividadChoice.Tiendas.Count);
                if (ActividadChoice.Tiendas.Count > 0)
                {
                    var listaRegion = GetListaRegiones(ActividadChoice.Tiendas);
                    Debug.WriteLine(listaRegion.Count);

                    ListaLugares.Clear();
                    foreach (Tienda item in listaRegion)
                    {
                        var tmp = DataPerfil.Where(x => x.Regiones.Trim() == item.Region.Trim()).FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(item.Region) && tmp != null)
                        {
                            Debug.WriteLine(item.Region);
                            ListaLugares.Add(item.Region);
                        }
                    }

                    Height = (ListaLugares.Count * 20) + (ListaLugares.Count * 10);
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
                if (ActividadChoice.Tiendas.Count > 0)
                {
                    var listaDistrito = GetListaDistritos(ActividadChoice.Tiendas);

                    ListaLugares.Clear();
                    foreach (Tienda item in listaDistrito)
                    {
                        var tmp = DataPerfil.Where(x => x.Distritos.Trim() == item.Distrito.Trim()).FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(item.Distrito) && tmp != null)
                        {
                            Debug.WriteLine(item.Distrito + "|");
                            ListaLugares.Add(item.Distrito.Trim());
                        }
                    }

                    Height = (ListaLugares.Count * 20) + (ListaLugares.Count * 10);
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

        List<Tienda> GetListaRegiones(List<Tienda> DataTiendas)
        {
            List<Tienda> listaRegion = (from p in DataTiendas
                                        group p by p.Region into grupo
                                        where grupo.Count() >= 1
                                        select new Tienda()
                                        {
                                            Region = grupo.Key,
                                        }).OrderBy(x => x.Region).Distinct().ToList();

            return listaRegion;
        }

        List<Tienda> GetListaDistritos(List<Tienda> DataTiendas)
        {
            List<Tienda> listaRegion = (from p in DataTiendas
                                        where p.Region == RegionChoice
                                        group p by new { p.Region, p.Distrito } into grupo
                                        where grupo.Count() >= 1
                                        select new Tienda()
                                        {
                                            Distrito = grupo.Key.Distrito,
                                            Region = grupo.Key.Region
                                        }).OrderBy(x => x.Distrito).Distinct().ToList();

            return listaRegion;
        }

        List<Tienda> GetListaSucursales(List<Tienda> DataTiendas)
        {
            List<Tienda> listaRegion = (from p in DataTiendas
                                        where p.Region == RegionChoice
                                        where p.Distrito == DistritoChoice
                                        group p by new { p.Id, p.Nombre} into grupo
                                        where grupo.Count() >= 1
                                        select new Tienda()
                                        {
                                            Id = grupo.Key.Id,
                                            Nombre = grupo.Key.Nombre
                                        }).ToList();

            return listaRegion;
        }

        #endregion


        #region Variables

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

        string fec;
        public string FechaActividad
        {
            get { return fec; }
            set
            {
                fec = value;
                OnPropertyChanged();
            }
        }

        bool act;
        public bool IsActividad
        {
            get { return act; }
            set
            {
                act = value;
                OnPropertyChanged();
            }
        }

        bool det = true;
        public bool IsAllActividad
        {
            get { return det; }
            set
            {
                det = value;
                OnPropertyChanged();
            }
        }

        // -- Mostrar barra detalles

        List<AuxDetalles> lalista;
        public List<AuxDetalles> ListaDetalles
        {
            get { return lalista; }
            set
            {
                lalista = value;
                OnPropertyChanged();
            }
        }

        List<string> lalista2;
        public List<string> ListaLugares
        {
            get { return lalista2; }
            set
            {
                lalista2 = value;
                OnPropertyChanged();
            }
        }

        ObservableCollection<object> lalista3;
        public ObservableCollection<object> ListaFechas
        {
            get { return lalista3; }
            set
            {
                lalista3 = value;
                OnPropertyChanged();
            }
        }

        Chart cl;
        public Chart ChartPastel
        {
            get { return cl; }
            set
            {
                cl = value;
                OnPropertyChanged();
            }
        }

        Chart cd;
        public Chart ChartDonas
        {
            get { return cd; }
            set
            {
                cd = value;
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

        #endregion
    }
}
