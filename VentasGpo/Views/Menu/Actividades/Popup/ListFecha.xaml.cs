using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

using VentasGpo.ViewModels.Modulos;

namespace VentasGpo.Views.Menu.Actividades.Popup
{
    public partial class ListFecha : PopupPage
    {
        readonly ActividadesVM vs;

        public ListFecha(ActividadesVM ve)
        {
            InitializeComponent();

            Titulo.Text = "Fecha";
            BindingContext = vs = ve;
        }

        async void OnClose(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        async void TapDate_Tapped(System.Object sender, System.EventArgs e)
        {
            Frame fecha = sender as Frame;
            MessagingCenter.Send(this, "FechaChoice", fecha.ClassId.ToString());

            await PopupNavigation.Instance.PopAsync();
            //vs.Height = 0;
            //vs.ListaLugares.Clear();
        }

    }
}
