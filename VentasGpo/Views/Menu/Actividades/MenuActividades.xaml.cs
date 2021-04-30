using System;
using System.Collections.Generic;
using Xamarin.Forms;

using VentasGpo.ViewModels;

namespace VentasGpo.Views.Menu.Actividades
{
    public partial class MenuActividades : ContentPage
    {
        public MenuActividades()
        {
            InitializeComponent();
            BindingContext = new MarcasVM();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (App.IsAndroid || App.IsiOSX)
            {
                TitleModal.IsVisible = false;
            }
        }

        async void MenuBrands_Tapped(System.Object sender, System.EventArgs e)
        {
            Frame menu = sender as Frame;
            Console.WriteLine(menu.ClassId);

            //int marca_id = int.Parse(menu.ClassId);
            Application.Current.Properties["marca_id"] = menu.ClassId;
            await Application.Current.SavePropertiesAsync();

            Page page = (Page)Activator.CreateInstance(typeof(RTActivity));

            await App.Plataforma.PushNavigation(Navigation, page);
        }
    }
}
