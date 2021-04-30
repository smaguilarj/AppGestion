using System;
using System.Diagnostics;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Collections.Generic;

using VentasGpo.Helpers;
using VentasGpo.Portable.Helpers;

namespace VentasGpo.ViewModels
{
    public class PerfilVM : VMBase
    {
        public Command LoadDataCommand { get; }

        public PerfilVM()
        {
            LoadDataCommand = new Command(() => GetPerfil());
        }

        void GetPerfil()
        {
            IsBusy = true;

            try
            {
                var name = Application.Current.Properties.ContainsKey("name") ? (string)Application.Current.Properties["name"] : "";
                var puesto = Application.Current.Properties.ContainsKey("puesto") ? (string)Application.Current.Properties["puesto"] : "";
                string nivel = Application.Current.Properties.ContainsKey("nivel") ? (string)Application.Current.Properties["nivel"] : "";
                string marcas = Application.Current.Properties.ContainsKey("marcas") ? (string)Application.Current.Properties["marcas"] : "";
                var lista = JsonConvert.DeserializeObject<List<Empresa>>(marcas);

                Nombre = name;
                Puesto = puesto;

                //color marca
                if (!string.IsNullOrEmpty(marcas) && lista != null)
                {
                    foreach (Empresa mid in lista)
                    {
                        ColorBrand(mid.marca_id);
                    }
                }

                //nivel perfil
                var perfil_user = MyPerfil.GetPerfil(int.Parse(nivel));
                TipoPerfil = perfil_user.ToString();

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

        void ColorBrand(int marca_id)
        {
            switch (marca_id)
            {
                case 1:
                    ColorNutrisa = Color.FromHex("#E25050").ToHex();
                    break;
                case 2:
                    ColorLavazza = Color.FromHex("#E25050").ToHex();
                    break;
                case 3:
                    ColorCielito = Color.FromHex("#E25050").ToHex();
                    break;
                default:
                    break;
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

        string puesto;
        public string Puesto
        {
            get { return puesto; }
            set
            {
                puesto = value;
                OnPropertyChanged();
            }
        }

        string type;
        public string TipoPerfil
        {
            get { return type; }
            set
            {
                type = value;
                OnPropertyChanged();
            }
        }

        string nutri = Color.FromHex("#9D9999").ToHex();
        public string ColorNutrisa
        {
            get { return nutri; }
            set
            {
                nutri = value;
                OnPropertyChanged();
            }
        }

        string moyo = Color.FromHex("#9D9999").ToHex();
        public string ColorMoyo
        {
            get { return moyo; }
            set
            {
                moyo = value;
                OnPropertyChanged();
            }
        }

        string lava = Color.FromHex("#9D9999").ToHex();
        public string ColorLavazza
        {
            get { return lava; }
            set
            {
                lava = value;
                OnPropertyChanged();
            }
        }

        string cielo = Color.FromHex("#9D9999").ToHex();
        public string ColorCielito
        {
            get { return cielo; }
            set
            {
                cielo = value;
                OnPropertyChanged();
            }
        }
    }
}
