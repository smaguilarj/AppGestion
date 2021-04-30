using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

using VentasGpo.ViewModels.Modulos;
using AppNutOp.Models;

namespace VentasGpo.Views.Menu.Actividades
{
    public partial class RTActivity : ContentPage
    {
        readonly ActividadesVM vs;
        readonly Color Light;
        readonly Color Dark;

        public RTActivity()
        {
            InitializeComponent();
            BindingContext = vs = new ActividadesVM();
            FiltroPlace.Actividades = vs;
            FiltroDate.Actividades = vs;
            
            Light = App.MyPerfil.LightColor();
            Dark = App.MyPerfil.DarkColor();
        }

        protected override void OnAppearing()
        {
            _ = ActivityContinously(Flor, new CancellationToken());

            base.OnAppearing();

            vs.ActividadesCommand.Execute(null);

            if (App.IsAndroid || App.IsiOSX)
            {
                TitleModal.IsVisible = false;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            
            Flor.Rotation = 0;
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

        void List_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            Console.WriteLine("TAP:" + e.Item.ToString());
            var jsona = JsonConvert.SerializeObject(e.Item);
            var json = JsonConvert.DeserializeObject<AuxDetalles>(jsona);

            int IdAct = json.IdActividad;
            if (IdAct != 0)
            {
                vs.GetActividadCommand.Execute(IdAct);
            }
            else
            {
                SelectLugar(json.Descripcion);
            }
        }

        void SelectLugar(string Item)
        {
            if (string.IsNullOrEmpty(vs.RegionChoice))
            {
                vs.RegionChoice = Item;
                vs.DistritoChoice = string.Empty;
                vs.TiendaChoice = string.Empty;

                MessagingCenter.Send(this, "ColorActive", "R");
            }
            else if (string.IsNullOrEmpty(vs.DistritoChoice))
            {
                vs.DistritoChoice = Item;
                vs.TiendaChoice = string.Empty;

                MessagingCenter.Send(this, "ColorActive", "D");
            }
            else if (string.IsNullOrEmpty(vs.TiendaChoice))
            {
                //vs.TiendaChoice = Item;
                //MessagingCenter.Send(this, "ColorActive", "T");
            }

            vs.FilterPlaceCommand.Execute(null);
        }
    }
}
