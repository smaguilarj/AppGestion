using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Xamarin.Forms;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using AppNutOp.Models;
using VentasGpo.Helpers;
using VentasGpo.Portable.Helpers;

namespace VentasGpo.ViewModels
{
    public class ModulosVM : VMBase
    {
        public Command ModulosCommand { get; }
        public Command GetModuloCommand { get; }

        PerfilModulo Ventas;
        PerfilModulo Operaciones;
        PerfilModulo Actividades;

        List<Modulo> ListaInicial;

        public ModulosVM()
        {
            ModulosCommand = new Command( () =>  GetModulos());
            GetModuloCommand = new Command<string>( (x) => GetModulo(x));
            InitModulos();
        }

        void GetModulos()
        {
            IsBusy = true;

            try
            {
                var name = Application.Current.Properties.ContainsKey("name") ? (string)Application.Current.Properties["name"] : "";
                var sexo = Application.Current.Properties.ContainsKey("sexo") ? (string)Application.Current.Properties["sexo"] : "";
                Nombre = "Bienvenid" + (string.Equals(sexo, "F") ? "a" : "o") + " " + name;

                string perfil = Application.Current.Properties.ContainsKey("nivel") ? (string)Application.Current.Properties["nivel"] : "";
                var myPerfil = MyPerfil.GetPerfil(Convert.ToInt32(perfil));

                string modulos = Application.Current.Properties.ContainsKey("modulos") ? (string)Application.Current.Properties["modulos"] : "";
                ListaInicial = JsonConvert.DeserializeObject<List<Modulo>>(modulos);

                ListaDeModulos = new ObservableCollection<PerfilModulo>();
                if (!string.IsNullOrEmpty(modulos) && ListaInicial != null && myPerfil != EnumPerfil.Tienda)
                {
                    foreach (Modulo mid in ListaInicial)
                    {
                        var mdo = DataModule(mid.modulo_id);
                        if (mdo != null)
                        {
                            ListaDeModulos.Add(mdo);
                        }
                    }
                }
                else
                {
                    //las tiendas ven ventas por default
                    if(myPerfil == EnumPerfil.Tienda)
                        ListaDeModulos.Add(Ventas);
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

        void GetModulo(string IdModulo)
        {
            List<PerfilModulo> perfilModulos = new List<PerfilModulo>();
            var modulo = ListaDeModulos.Where(i => i.IdModulo == IdModulo).SingleOrDefault();
            foreach(PerfilModulo pmod in modulo.Perfiles)
            {
                var ext = ListaInicial.Where(x => x.modulo_id.ToString() == pmod.IdModulo).SingleOrDefault();
                if(ext != null)
                {
                    perfilModulos.Add(pmod);
                }
            }

            ListaDeModulos = new ObservableCollection<PerfilModulo>(perfilModulos);
            Nombre = modulo.Descripcion + " Grupo Herdez";
        }

        PerfilModulo DataModule(int modulo_id)
        {
            PerfilModulo modulo;
            switch (modulo_id)
            {
                case 1:
                    modulo = Ventas;
                    break;
                case 2:
                    modulo = Operaciones;
                    break;
                default:
                    modulo = null;
                    break;
            }

            return modulo;
        }

        void InitModulos()
        {
            Actividades = new PerfilModulo()
            {
                IdModulo = "3",
                Descripcion = "Actividades",
                Image = $"capa.png",
                IdModuloPadre = "2"
            };

            Ventas = new PerfilModulo()
            {
                IdModulo = "1",
                Descripcion = "Ventas",
                Image = $"ventas.png"
            };

            Operaciones = new PerfilModulo()
            {
                IdModulo = "2",
                Descripcion = "Operaciones",
                Image = $"opera.png"
            };
            Operaciones.Perfiles = new List<PerfilModulo>() { Actividades };
        }

        ObservableCollection<PerfilModulo> lalista;
        public ObservableCollection<PerfilModulo> ListaDeModulos
        {
            get { return lalista; }
            set
            {
                lalista = value;
                OnPropertyChanged();
            }
        }

        string name;
        public string Nombre
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

    }

}
