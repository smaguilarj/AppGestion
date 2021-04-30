using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using VentasGpo.ViewModels.Modulos;

namespace VentasGpo.Views.Menu.Ventas.Popup
{
    public partial class ListSearch : PopupPage
    {
        readonly VentasVM vs;

        public ListSearch(VentasVM ve)
        {
            InitializeComponent();
            BindingContext = vs = ve;

            LaLista.HeightRequest = 0;

            MessagingCenter.Subscribe<VentasVM, List<string>>(this, "LaLista", (sender, args) => {
                LaLista.ItemsSource = null;
                LaLista.ItemsSource = args;
                LaLista.HeightRequest = (args.Count * 20) + (args.Count * 10);
            });
        }

        async void OnClose(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();

            vs.Height = 0;
            vs.ListaRegiones.Clear();

            Search.Text = string.Empty;
            LaLista.ItemsSource = null;
            LaLista.HeightRequest = 0;
        }

        async void Lista_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            if (e.Item.ToString() != "Sin resultados")
            {
                MessagingCenter.Send(this, "Colorful");

                vs.TiendaChoice = e.Item.ToString();
                vs.FilterPlaceSearchCommand.Execute(null);

                await PopupNavigation.Instance.PopAsync();
                vs.Height = 0;
                vs.ListaRegiones.Clear();

                Search.Text = string.Empty;
                LaLista.ItemsSource = null;
                LaLista.HeightRequest = 0;
            }
        }
    }
}
