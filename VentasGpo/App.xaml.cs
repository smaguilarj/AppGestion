using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using VentasGpo.Views.Index;
using VentasGpo.Helpers;
using VentasGpo.Portable.Interfaces;

namespace VentasGpo
{
    public partial class App : Application
    {
        public static int ScreenHeight { get; set; }
        public static string AppVersion { get; set; }

        public static bool IsAndroid { get; set; }
        public static bool IsiOSX { get; set; }

        public App()
        {
            InitializeComponent();

            MainPage = new MasterPage();
        }

        protected override void OnStart()
        {
            Plataforma.GetInit();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        public static IPerfil MyPerfil => DependencyService.Get<IPerfil>();

        private static Message c;
        public static Message Control
        {
            get { return c = c ?? new Message(); }
        }

        private static Distribution d;
        public static Distribution GetAppCenter
        {
            get { return d = d ?? new Distribution(); }
        }

        private static Platform p;
        public static Platform Plataforma
        {
            get { return p = p ?? new Platform(); }
        }
    }
}
