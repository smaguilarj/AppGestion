using System;
using System.Threading.Tasks;
using Xamarin.Forms;

using VentasGpo.ViewModels.Modulos;
using VentasGpo.Portable.Helpers;
using System.Threading;

namespace VentasGpo.Views.Menu.Ventas
{
    public partial class RTVentas : ContentPage
    {
        const uint ExpandAnimationSpeed = 350;
        const uint CollapseAnimationSpeed = 250;
        double pageHeight = 0;

        readonly VentasVM vs;
        readonly Color Light;
        readonly Color Dark;

        public RTVentas()
        {
            InitializeComponent();
            BindingContext = vs = new VentasVM();
            Filtro.Ventas = vs;

            //Grey = (Color)Application.Current.Resources["ColorGreyDark"];
            Light = App.MyPerfil.LightColor();
            Dark = App.MyPerfil.DarkColor();

            //Bloquear lupa en nivel sucursal
            string nivel = Application.Current.Properties.ContainsKey("nivel") ? (string)Application.Current.Properties["nivel"] : "";
            EnumPerfil myPerfil = App.MyPerfil.GetPerfil(Convert.ToInt32(nivel));
            if(myPerfil == EnumPerfil.Tienda)
            {
                Lupa.IsVisible = false;
            }

            //Colors init
            BackLeyenda.BackgroundColor = Dark;
            Visitas.BackgroundColor = Light;
            Ventas.BackgroundColor = Dark;
            Tickets.BackgroundColor = Light;
            Pptos.BackgroundColor = Light;
        }

        protected override void OnAppearing()
        {
            CartPopup.OnExpandTapped += OnExpand;
            CartPopup.OnCollapseTapped += OnCollapse;

            int resta = 60;
            if (App.ScreenHeight > 700)
                resta = 10;
            if (App.ScreenHeight > 800)
                resta = 120;

            pageHeight = App.ScreenHeight - resta;
            CartPopup.TranslationY = pageHeight - CartPopup.HeaderHeight;

            _ = ActivityContinously(Flor, new CancellationToken());

            base.OnAppearing();

            vs.FilterCommand.Execute(null);

            if (App.IsAndroid || App.IsiOSX)
            {
                TitleModal.IsVisible = false;
            }
        }

        protected override void OnDisappearing()
        {
            CartPopup.OnExpandTapped -= OnExpand;
            CartPopup.OnCollapseTapped -= OnCollapse;

            base.OnDisappearing();

            Flor.Rotation = 0;
        }

        #region CartPopUp

        private void OnExpand()
        {
            vs.GetDetailsCommand.Execute(null);

            CartPopupFade.IsVisible = true;
            CartPopupFade.FadeTo(1, ExpandAnimationSpeed, Easing.SinInOut);

            var height = pageHeight - CartPopup.HeaderHeight - 20;
            CartPopup.TranslateTo(0, Height - height, ExpandAnimationSpeed, Easing.SinInOut);
        }

        private void OnCollapse()
        {
            CartPopupFade.FadeTo(0, CollapseAnimationSpeed, Easing.SinInOut);
            CartPopupFade.IsVisible = false;
            CartPopup.TranslateTo(0, pageHeight - CartPopup.HeaderHeight, CollapseAnimationSpeed, Easing.SinInOut);

            vs.ListaDetalles.Clear();
        }

        #endregion

        async Task ActivityContinously(VisualElement element, CancellationToken cancellation)
        {
            while (!cancellation.IsCancellationRequested)
            {
                for (int i = 1; i < 7; i++)
                {
                    //if (element.Rotation >= 360f) element.Rotation = 0;
                    await element.RotateTo(i * (360 / 6), 400, Easing.Linear);
                }
                await element.RotateTo(0, 0);
            }
        }

        #region Regiones

        void Switch_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            var comp = sender as Switch;
            vs.IsComp = comp.IsToggled;

            vs.CompsCommand.Execute(null);
        }

        async void FindSucursal_Tapped(System.Object sender, System.EventArgs e)
        {
            await App.Control.PopListaSearch(vs);
        }

        #endregion

        #region Filter Categorias

        void TapGestureVisitas(System.Object sender, System.EventArgs e)
        {
            //change colors
            Visitas.BackgroundColor = Dark;
            Ventas.BackgroundColor = Light;
            Tickets.BackgroundColor = Light;
            Pptos.BackgroundColor = Light;

            //change datas
            vs.CategoriasCommand.Execute(2);
        }

        void TapGestureVentas(System.Object sender, System.EventArgs e)
        {
            //change colors
            Visitas.BackgroundColor = Light;
            Ventas.BackgroundColor = Dark;
            Tickets.BackgroundColor = Light;
            Pptos.BackgroundColor = Light;

            //change datas
            vs.CategoriasCommand.Execute(1);
        }

        void TapGestureTickets(System.Object sender, System.EventArgs e)
        {
            //change colors
            Visitas.BackgroundColor = Light;
            Ventas.BackgroundColor = Light;
            Tickets.BackgroundColor = Dark;
            Pptos.BackgroundColor = Light;

            //change datas
            vs.CategoriasCommand.Execute(3);
        }

        void TapGesturePresupuesto(System.Object sender, System.EventArgs e)
        {
            //change colors
            Visitas.BackgroundColor = Light;
            Ventas.BackgroundColor = Light;
            Tickets.BackgroundColor = Light;
            Pptos.BackgroundColor = Dark;

            //change datas
            vs.CategoriasCommand.Execute(4);
        }

        #endregion
    }
}
