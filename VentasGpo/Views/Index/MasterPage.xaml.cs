using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

using VentasGpo.Helpers;
using VentasGpo.Views.Home;
using VentasGpo.Portable.Helpers;
using VentasGpo.ViewModels;
using VentasGpo.ViewModels.Modulos;

namespace VentasGpo.Views.Index
{
    public partial class MasterPage : MasterDetailPage
    {
        readonly List<MenuMaster> Menu = new List<MenuMaster>() {
            new MenuMaster {Pagina = 1, Titulo = "Inicio", Tipo = typeof(MenuPrincipal)},
            new MenuMaster {Pagina = 2, Titulo = "Mi perfil", Tipo = typeof(MyPerfil) },
            new MenuMaster {Pagina = 3, Titulo = "Ayuda", Tipo = typeof(Ayuda) },
            new MenuMaster {Pagina = 4, Titulo = "Acerca de", Tipo = typeof(AboutDe) },
            new MenuMaster {Pagina = 5, Titulo = "Cerrar sesión", Tipo = typeof(Login)}
        };
        public ObservableCollection<MenuMaster> ListaMenu { get; set; }

        public MasterPage()
        {
            InitializeComponent();

            IsGestureEnabled = false;
            Detail = new NavigationPage(new Login());

            string dev = (Constants._DEV == true) ? "DEV - " : "";
            Version.Text = dev + "v" + App.AppVersion;

            ListaMenu = new ObservableCollection<MenuMaster>(Menu);
            ListaDeMenu.ItemsSource = ListaMenu;
        
            MessagingCenter.Subscribe<MasterPage>(this, "UpdateMaster", (sender) =>
            {
                IsGestureEnabled = true;
                Nombre.Text = Application.Current.Properties.ContainsKey("name") ? (string)Application.Current.Properties["name"] : "";
                Puesto.Text = Application.Current.Properties.ContainsKey("puesto") ? (string)Application.Current.Properties["puesto"] : "";
            });

            //Return iOS
            MessagingCenter.Subscribe<MarcasVM>(this, "ReturnActivated", async (sender) =>
            {
                await Navigation.PopModalAsync();
            });

            MessagingCenter.Subscribe<ActividadesVM>(this, "ReturnActivated", async (sender) =>
            {
                await Navigation.PopModalAsync();
            });

            MessagingCenter.Subscribe<VentasVM>(this, "ReturnActivated", async (sender) =>
            {
                await Navigation.PopModalAsync();
            });

        }

        private void Menu_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var menu = e.SelectedItem as MenuMaster;
            if (menu != null)
            {
                IsPresented = false;
                if (menu.Pagina == 5) LogOut();

                Detail = new NavigationPage((Page)Activator.CreateInstance(menu.Tipo));

                ((ListView)sender).SelectedItem = null;
            }
        }

        void LogOut()
        {
            IsGestureEnabled = false;

            Application.Current.Properties.Clear();
        }

        protected override bool OnBackButtonPressed()
        {
            return false;
        }
    }
}
