using System;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using VentasGpo.DataObjects;
using VentasGpo.Helpers;

namespace VentasGpo.ViewModels
{
    public class MarcasVM : VMBase
    {
        public Command ReturnCommand { get; set; }

        public MarcasVM()
        {
            ReturnCommand = new Command(() => ExecuteReturnCommand());

            string marcas = Application.Current.Properties.ContainsKey("marcas") ? (string)Application.Current.Properties["marcas"] : "";
            var lista = JsonConvert.DeserializeObject<List<Empresa>>(marcas);

            ListaDeMarcas = new ObservableCollection<Marca>();

            if (!string.IsNullOrEmpty(marcas) && lista != null)
            {
                foreach (Empresa mid in lista)
                {
                    var brand = DataBrand(mid.marca_id);
                    if (brand != null)
                    {
                        ListaDeMarcas.Add(brand);
                    }
                }
            }
        }

        Marca DataBrand(int marca_id)
        {
            Marca marca = new Marca
            {
                IdMarca = marca_id
            };

            switch (marca_id)
            {
                case 1:
                    marca.Descripcion = "Nutrisa";
                    marca.Image = $"logoNut.png";
                    break;
                case 2:
                    marca.Descripcion = "Lavazza";
                    marca.Image = $"lavazza.png";
                    break;
                case 3:
                    marca.Descripcion = "Cielito";
                    marca.Image = $"cielito.png";
                    break;
                default:
                    marca = null;
                    break;
            }

            return marca;
        }

        private void ExecuteReturnCommand()
        {
            MessagingCenter.Send(this, "ReturnActivated");
        }

        ObservableCollection<Marca> lalista;
        public ObservableCollection<Marca> ListaDeMarcas
        {
            get { return lalista; }
            set
            {
                lalista = value;
                OnPropertyChanged();
            }
        }
    }

}
