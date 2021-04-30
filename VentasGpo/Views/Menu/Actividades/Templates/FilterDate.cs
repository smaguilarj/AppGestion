using System;
using System.Threading;
using System.Globalization;
using Xamarin.Forms;
using System.Collections.Generic;

using VentasGpo.ViewModels.Modulos;
using VentasGpo.Views.Menu.Actividades.Popup;

namespace VentasGpo.Views.Menu.Actividades.Templates
{
    public class FilterDate : Grid
    {
        public static readonly BindableProperty ActividadesProperty = BindableProperty.Create(nameof(Actividades),
            typeof(ActividadesVM), typeof(FilterDate), default(ActividadesVM), BindingMode.TwoWay);
        public ActividadesVM Actividades
        {
            get
            {
                return (ActividadesVM)GetValue(ActividadesProperty);
            }

            set
            {
                SetValue(ActividadesProperty, value);
            }
        }

        Frame FilCal;
        Frame FilBack;
        Frame FilNext;
        Frame FilNow;

        Label FechaFiltro;
        Label Header;

        DateTime FechaHoy;
        DateTime LaFecha;
        DateTime LimitFechaMax;
        DateTime LimitFechaMin;
        string Fecha;
        List<string> ListaFechas;

        public FilterDate()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand;

            FechaHoy = DateTime.Now;
            LaFecha = FechaHoy;
            LimitFechaMax = FechaHoy.AddDays(8);
            LimitFechaMin = FechaHoy.AddDays(-8);
            Fecha = FechaHoy.ToString("dd-MM-y");

            ListaFechas = new List<string>();

            ControlInit();
            LoadBack();

            MessagingCenter.Subscribe<ListFecha, string>(this, "FechaChoice", (sender, args) =>
            {
                DateChoice(args);
            });
        }

        void LoadBack()
        {
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });

            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.5, GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.5, GridUnitType.Star) });

            Children.Add(FilCal, 0, 0);
            Children.Add(FilBack, 1, 0);
            Children.Add(FilNow, 2, 0);
            Children.Add(FilNext, 3, 0);
        }

        #region Fechas

        void DiaAtras(object sender, EventArgs e)
        {
            if (LaFecha > LimitFechaMin)
            {
                LaFecha = LaFecha.AddDays(-1);
                FechaFiltro.Text = LaFecha.ToString("dd-MM-y");
            }

            CleanChoice();
            GetHeader();
            
            Actividades.FechaChoice = LaFecha.ToString("yyyy-MM-dd");
            Actividades.ActividadesCommand.Execute(null);
        }

        void DiaDespues(object sender, EventArgs e)
        {
            if (LaFecha < LimitFechaMax)
            {
                LaFecha = LaFecha.AddDays(1);
                FechaFiltro.Text = LaFecha.ToString("dd-MM-y");
            }

            CleanChoice();
            GetHeader();

            Actividades.FechaChoice = LaFecha.ToString("yyyy-MM-dd");
            Actividades.ActividadesCommand.Execute(null);
        }

        void DiaActual(object sender, EventArgs e)
        {
            CleanChoice();

            Actividades.ActividadesCommand.Execute(null);
        }

        async void GetFechas(object sender, EventArgs e)
        {
            Actividades.GetFechasDisponibles.Execute(null);
            await App.Control.PopListaFechas(Actividades);
        }

        void DateChoice(string Fechilla)
        {
            //var date = Fechilla.Split('-');
            var aux = Fechilla + " " + FechaHoy.ToString("HH:mm:ss");
            var auxFecha = DateTime.ParseExact(aux, "MM-dd-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            int auxDias = (int)Math.Round((auxFecha - FechaHoy).TotalDays);

            LaFecha = FechaHoy.AddDays(auxDias);
            FechaFiltro.Text = LaFecha.ToString("dd-MM-y"); ;

            CleanChoice();
            GetHeader();

            Actividades.FechaChoice = LaFecha.ToString("yyyy-MM-dd");
            Actividades.ActividadesCommand.Execute(null);
        }

        void CleanChoice()
        {
            Actividades.RegionChoice = string.Empty;
            Actividades.DistritoChoice = string.Empty;
            Actividades.TiendaChoice = string.Empty;
            Actividades.ActividadChoice = null;
            Actividades.IdActividadChoice = 0;
            Actividades.IsActividad = false;

            MessagingCenter.Send(this, "ColorDefault", "DR");
        }

        void GetHeader()
        {
            if (LaFecha == FechaHoy)
            {
                Header.Text = "HOY";
            }
            else if((LaFecha - FechaHoy).TotalDays == 1)
            {
                Header.Text = "MAÑANA";
            }
            else if((FechaHoy - LaFecha).TotalDays == 1)
            {
                Header.Text = "AYER";
            }
            else
            {
                var dia = Thread.CurrentThread.CurrentUICulture.DateTimeFormat.GetDayName(LaFecha.DayOfWeek);
                Header.Text = dia.ToUpper();
            }
            Console.WriteLine("Header.Text " + Header.Text);
        }

        #endregion

        #region Styles

        void ControlInit()
        {
            FechaFiltro = new Label()
            {
                Text = Fecha,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = -3
            };
            Header = new Label()
            {
                Text = "HOY",
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Start,
                FontSize = 10
            };

            //calendario
            var tapCale = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1
            };
            tapCale.Tapped += GetFechas;
            FilCal = new Frame()
            {
                BorderColor = (Color)Application.Current.Resources["ColorGreyLight"],
                CornerRadius = 12,
                Padding = 6,
                Content = new Image
                {
                    Source = $"calendar.png",
                    Aspect = Aspect.AspectFit
                }
            };
            FilCal.GestureRecognizers.Add(tapCale);

            //back
            var tapAtras = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1
            };
            tapAtras.Tapped += DiaAtras;
            FilBack = new Frame()
            {
                BorderColor = (Color)Application.Current.Resources["ColorGreyLight"],
                CornerRadius = 12,
                Padding = 5,
                Content = new Image
                {
                    Source = $"arrow_left.png",
                    Aspect = Aspect.AspectFit
                }
            };
            FilBack.GestureRecognizers.Add(tapAtras);

            //next
            var tapDelante = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1
            };
            tapDelante.Tapped += DiaDespues;
            FilNext = new Frame()
            {
                BorderColor = (Color)Application.Current.Resources["ColorGreyLight"],
                CornerRadius = 12,
                Padding = 5,
                Content = new Image
                {
                    Source = $"arrow_right.png",
                    Aspect = Aspect.AspectFit
                }
            };
            FilNext.GestureRecognizers.Add(tapDelante);

            //today
            var tapToday = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1
            };
            tapToday.Tapped += DiaActual;
            Grid grid = new Grid()
            {
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(4, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(6, GridUnitType.Star) });
            grid.Children.Add(Header, 0, 0);
            grid.Children.Add(FechaFiltro, 0, 1);

            FilNow = new Frame()
            {
                BorderColor = (Color)Application.Current.Resources["ColorGreyLight"],
                BackgroundColor = App.MyPerfil.LightColor(),
                CornerRadius = 12,
                Padding = 5,
                VerticalOptions = LayoutOptions.Center,
                Content = grid
            };
            FilNow.GestureRecognizers.Add(tapToday);

        }

        #endregion
    }
}
