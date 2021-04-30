using System;
using System.Collections.Generic;
using Xamarin.Forms;

using VentasGpo.ViewModels;

namespace VentasGpo.Views.Home
{
    public partial class MyPerfil : ContentPage
    {
        readonly PerfilVM vs;
        public MyPerfil()
        {
            InitializeComponent();
            BindingContext = vs = new PerfilVM();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            vs.LoadDataCommand.Execute(null);
        }
    }
}
