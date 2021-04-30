using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

using VentasGpo.ViewModels.Modulos;

namespace VentasGpo.Views.Menu.Actividades.Popup
{
    public partial class ListFiltro : PopupPage
    {
        readonly ActividadesVM vs;

        public ListFiltro(string titulo, ActividadesVM ve)
        {
            InitializeComponent();

            Titulo.Text = titulo;
            BindingContext = vs = ve;
        }

        async void OnClose(object sender, EventArgs e)
        {
            ColorFilterPlace();
            await PopupNavigation.Instance.PopAsync();
        }

        async void Lista_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            //Console.WriteLine("TAP:"+e.Item.ToString());
            if (string.Equals(Titulo.Text, "Región"))
            {
                vs.RegionChoice = e.Item.ToString();
                vs.DistritoChoice = string.Empty;
                vs.TiendaChoice = string.Empty;
            }
            else if (string.Equals(Titulo.Text, "Distrito"))
            {
                vs.DistritoChoice = e.Item.ToString();
                vs.TiendaChoice = string.Empty;
            }
            else if (string.Equals(Titulo.Text, "Tienda"))
            {
                vs.TiendaChoice = e.Item.ToString();
            }

            await PopupNavigation.Instance.PopAsync();
            vs.Height = 0;
            vs.ListaLugares.Clear();

            vs.FilterPlaceCommand.Execute(null);
        }

        void ColorFilterPlace()
        {
            if (string.Equals(Titulo.Text, "Región"))
            {
                if (string.IsNullOrEmpty(vs.RegionChoice))
                    MessagingCenter.Send(this, "ColorDefault", "R");

                if (!string.IsNullOrEmpty(vs.DistritoChoice))
                    MessagingCenter.Send(this, "ColorActive", "D");
                if (!string.IsNullOrEmpty(vs.TiendaChoice))
                    MessagingCenter.Send(this, "ColorActive", "T");
            }
            else if (string.Equals(Titulo.Text, "Distrito"))
            {
                if (string.IsNullOrEmpty(vs.DistritoChoice))
                    MessagingCenter.Send(this, "ColorDefault", "D");

                if (!string.IsNullOrEmpty(vs.TiendaChoice))
                    MessagingCenter.Send(this, "ColorActive", "T");
            }
            else if (string.Equals(Titulo.Text, "Tienda"))
            {
                if (string.IsNullOrEmpty(vs.TiendaChoice))
                    MessagingCenter.Send(this, "ColorDefault", "T");
            }
        }
    }
}
