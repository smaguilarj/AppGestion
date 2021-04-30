using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

using VentasGpo.Portable.Interfaces;
using VentasGpo.ViewModels;
using System.Threading;

namespace VentasGpo.Views.Index
{
    public partial class Login : ContentPage
    {
        readonly LoginVM vs;

        public Login()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            BindingContext = vs = new LoginVM();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var _platform = DependencyService.Get<IDataConfig>();
            string texto = _platform.ReadUserData();

            if (texto != "")
            {
                string[] Parts = texto.Split('|');
                name.Text = Parts[0];
                password.Text = Parts[1];
                RememberMe.IsChecked = true;
            }

            _ = ActivityContinously(Flor, new CancellationToken());
        }

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

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Flor.Rotation = 0;
            //Navigation.RemovePage(this);
        }

        async void OnButtonClicked(object sender, EventArgs args)
        {
            if (string.IsNullOrEmpty(name.Text) || string.IsNullOrEmpty(password.Text))
            {
                await App.Control.Mensaje("Alerta", "Ingrese los datos para iniciar sesión");
            }
            else
            {
                vs.StartSessionCommand.Execute(null);
            }
        }


    }
}
