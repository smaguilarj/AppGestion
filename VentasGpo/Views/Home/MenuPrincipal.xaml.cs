using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;

using VentasGpo.ViewModels;
using VentasGpo.Views.Menu.Ventas;
using VentasGpo.Views.Menu.Actividades;
using VentasGpo.Views.Index;
using AppNutOp.Models;

namespace VentasGpo.Views.Home
{
    public partial class MenuPrincipal : ContentPage
    {
        readonly ModulosVM vs;
        public MenuPrincipal()
        {
            InitializeComponent();
            BindingContext = vs = new ModulosVM();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            vs.ModulosCommand.Execute(null);
            MessagingCenter.Send(new MasterPage(), "UpdateMaster");
        }

        async void MenuHome_Tapped(System.Object sender, System.EventArgs e)
        {
            Frame menu = sender as Frame;
            Page page = null;

            var modulo = vs.ListaDeModulos.Where(i => i.IdModulo == menu.ClassId).SingleOrDefault();
            if (modulo.Perfiles != null && modulo.Perfiles.Count != 0)
            {
                vs.GetModuloCommand.Execute(modulo.IdModulo);
            }
            else
            {
                switch (int.Parse(menu.ClassId))
                {
                    case 1:
                        page = Activator.CreateInstance(typeof(MenuVentas)) as Page;
                        break;
                    case 3:
                        //actividades
                        page = Activator.CreateInstance(typeof(MenuActividades)) as Page;
                        break;
                    default:
                        break;
                }

                await App.Plataforma.PushNavigation(Navigation, page);

            }
        }

    }
}
