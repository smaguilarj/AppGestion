using System;
using System.Collections.Generic;
using Xamarin.Forms;

using VentasGpo.Portable.Helpers;

namespace VentasGpo.Views.Home
{
    public partial class AboutDe : ContentPage
    {
        public AboutDe()
        {
            InitializeComponent();

            string dev = (Constants._DEV == true) ? "DEV - " : "";
            version.Text = dev + "Versión " + App.AppVersion;
        }
    }
}
