using System;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Collections.Generic;

using VentasGpo.Helpers;
using VentasGpo.Portable.Helpers;
using VentasGpo.ViewModels.Modulos;
using VentasGpo.Views.Menu.Actividades.Popup;

namespace VentasGpo.Views.Menu.Actividades.Templates
{
    public class FilterPlace : Grid
    {
        public static readonly BindableProperty FiltroProperty = BindableProperty.Create(nameof(Filtro),
            typeof(object), typeof(FilterPlace), default(object), BindingMode.TwoWay);
        public object Filtro
        {
            get
            {
                return (object)GetValue(FiltroProperty);
            }

            set
            {
                SetValue(FiltroProperty, value);
            }
        }

        public static readonly BindableProperty ActividadesProperty = BindableProperty.Create(nameof(Actividades),
            typeof(ActividadesVM), typeof(FilterPlace), default(ActividadesVM), BindingMode.TwoWay);
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

        Frame FiltroNacional;
        Frame FiltroRegional;
        Frame FiltroDistrital;

        Label Nacional;
        Label Regional;
        Label Distrital;

        int FilNacional;
        int FilRegional;
        int FilDistrital;

        Color Light;
        Color Black;

        EnumPerfil myPerfil;
        List<Departamento> myZonas;

        public FilterPlace()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand;
            ControlInit();
            ColorInit();

            MessagingCenter.Subscribe<ListFiltro, string>(this, "ColorDefault", (sender, args) =>
            {
                ColorDefault(args);
            });

            MessagingCenter.Subscribe<FilterDate, string>(this, "ColorDefault", (sender, args) =>
            {
                ColorDefault(args);
            });

            MessagingCenter.Subscribe<ListFiltro, string>(this, "ColorActive", (sender, args) =>
            {
                ColorActive(args);
            });

            MessagingCenter.Subscribe<RTActivity, string>(this, "ColorActive", (sender, args) =>
            {
                ColorActive(args);
            });
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == FiltroProperty.PropertyName)
            {
                ColumnDefinitions.Clear();
                Children.Clear();

                var values = (object[])Filtro;
                FilNacional = (int)values[0];
                FilRegional = (int)values[1];
                FilDistrital = (int)values[2];

                LoadBack();
                ColorInit();
            }
        }

        void LoadBack()
        {
            int botones = 3;
            if (FilNacional == 0) { botones--; }
            if (FilRegional == 0) { botones--; }
            if (FilDistrital == 0) { botones--; }

            Console.WriteLine("== " + botones);
            Console.WriteLine("N_" + FilNacional);
            Console.WriteLine("R_" + FilRegional);
            Console.WriteLine("D_" + FilDistrital);

            double aux = 10 / botones;
            for (var i = 0; i < botones; i++)
            {
                ColumnDefinitions.Add(
                    new ColumnDefinition { Width = new GridLength(aux, GridUnitType.Star) }
                );
            }

            if (botones == 3)
                Children.Add(FiltroNacional, 0, 0);

            if (botones >= 2)
            {
                if (FilRegional == 1)
                    Children.Add(FiltroRegional, botones - 2, 0);
                else if (FilNacional == 1 && FilRegional == 0)
                    Children.Add(FiltroNacional, botones - 2, 0);
            }

            Children.Add(FiltroDistrital, botones - 1, 0);
        }

        #region Funciones Botones

        void ClickedNacional(System.Object sender, System.EventArgs e)
        {
            Frame button = sender as Frame;
            button.BackgroundColor = Light;
            Nacional.TextColor = Color.White;

            if (FilRegional == 1)
                Actividades.RegionChoice = string.Empty;

            Actividades.DistritoChoice = string.Empty;
            Actividades.TiendaChoice = string.Empty;

            ColorDefault("DR");

            Actividades.GetActividadCommand.Execute(Actividades.ActividadChoice.Id);
        }

        async void ClickedRegional(System.Object sender, System.EventArgs e)
        {
            Frame button = sender as Frame;
            button.BackgroundColor = Light;
            Regional.TextColor = Color.White;

            ColorDefault("D");

            if (myPerfil == EnumPerfil.Regional && myZonas != null && myZonas.Count == 1)
            {
                Actividades.DistritoChoice = string.Empty;
                Actividades.TiendaChoice = string.Empty;

                Actividades.FilterPlaceCommand.Execute(null);
            }
            else
            {
                Actividades.GetRegionesCommand.Execute(null);
                await App.Control.PopListaFiltro("Región", Actividades);
            }
        }

        async void ClickedDistrital(System.Object sender, System.EventArgs e)
        {
            Frame button = sender as Frame;
            button.BackgroundColor = Light;
            Distrital.TextColor = Color.White;

            ColorDefault("T");

            if (myPerfil == EnumPerfil.Distrital && myZonas != null && myZonas.Count == 1)
            {
                Actividades.TiendaChoice = string.Empty;

                Actividades.FilterPlaceCommand.Execute(null);
            }
            else
            {
                if (!string.IsNullOrEmpty(Actividades.RegionChoice))
                {
                    Actividades.GetDistritosCommand.Execute(null);
                    await App.Control.PopListaFiltro("Distrito", Actividades);
                }
                else
                {
                    ColorDefault("D");
                    await App.Control.Mensaje("Aviso", "Selecciona una región");
                }
            }
        }

        #endregion

        #region Styles

        void ColorDefault(string bandera)
        {
            if (bandera.Contains("D"))
            {
                FiltroDistrital.BackgroundColor = Color.White;
                Distrital.TextColor = Black;
            }

            if (bandera.Contains("R"))
            {
                FiltroRegional.BackgroundColor = Color.White;
                Regional.TextColor = Black;
            }
        }

        void ColorActive(string bandera)
        {
            if (bandera.Contains("R"))
            {
                FiltroRegional.BackgroundColor = Light;
                Regional.TextColor = Color.White;
            }

            if (bandera.Contains("D"))
            {
                FiltroDistrital.BackgroundColor = Light;
                Distrital.TextColor = Color.White;
            }
        }

        void ControlInit()
        {
            Black = Color.Black;
            Light = App.MyPerfil.LightColor();

            string depas = Application.Current.Properties.ContainsKey("zonas") ? (string)Application.Current.Properties["zonas"] : "";
            myZonas = JsonConvert.DeserializeObject<List<Departamento>>(depas);

            Nacional = new Label
            {
                Text = "Nacional",
                FontSize = 12,
                HorizontalTextAlignment = TextAlignment.Center
            };
            Regional = new Label
            {
                Text = "Región",
                FontSize = 12,
                HorizontalTextAlignment = TextAlignment.Center
            };
            Distrital = new Label
            {
                Text = "Distrito",
                FontSize = 12,
                HorizontalTextAlignment = TextAlignment.Center
            };

            //nacional
            var tapNac = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1
            };
            tapNac.Tapped += ClickedNacional;
            FiltroNacional = new Frame()
            {
                BorderColor = (Color)Application.Current.Resources["ColorGreyLight"],
                BackgroundColor = Color.White,
                CornerRadius = 12,
                Padding = 5,
                Content = new StackLayout
                {
                    Children =
                    {
                        new Image
                        {
                            Source = $"PanalPais.png",
                            Aspect = Aspect.AspectFit,
                            HeightRequest = 25,
                            WidthRequest = 25
                        },
                        Nacional
                    }
                }
            };
            FiltroNacional.GestureRecognizers.Add(tapNac);

            //regional
            var tapReg = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1
            };
            tapReg.Tapped += ClickedRegional;
            FiltroRegional = new Frame()
            {
                BorderColor = (Color)Application.Current.Resources["ColorGreyLight"],
                BackgroundColor = Color.White,
                CornerRadius = 12,
                Padding = 5,
                Content = new StackLayout
                {
                    Children =
                    {
                        new Image
                        {
                            Source = $"PanalRegion.png",
                            Aspect = Aspect.AspectFit,
                            HeightRequest = 25,
                            WidthRequest = 25
                        },
                        Regional
                    }
                }
            };
            FiltroRegional.GestureRecognizers.Add(tapReg);

            //distrital
            var tapDis = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1
            };
            tapDis.Tapped += ClickedDistrital;
            FiltroDistrital = new Frame()
            {
                BorderColor = (Color)Application.Current.Resources["ColorGreyLight"],
                BackgroundColor = Color.White,
                CornerRadius = 12,
                Padding = 5,
                Content = new StackLayout
                {
                    Children =
                    {
                        new Image
                        {
                            Source = $"PanalDistrito.png",
                            Aspect = Aspect.AspectFit,
                            HeightRequest = 25,
                            WidthRequest = 25
                        },
                        Distrital
                    }
                }
            };
            FiltroDistrital.GestureRecognizers.Add(tapDis);

        }

        void ColorInit()
        {
            var perfil = Application.Current.Properties.ContainsKey("nivel") ? (string)Application.Current.Properties["nivel"] : "";
            int my_nivel = int.Parse(perfil);
            myPerfil = App.MyPerfil.GetPerfil(my_nivel);

            Console.WriteLine("dd" + myPerfil.ToString());
            switch (myPerfil)
            {
                case EnumPerfil.Regional: //regional nutrisa
                    FiltroRegional.BackgroundColor = Light;
                    Regional.TextColor = Color.White;
                    break;
                case EnumPerfil.Distrital:
                    FiltroDistrital.BackgroundColor = Light;
                    Distrital.TextColor = Color.White;
                    break;
                case EnumPerfil.Tienda:
                    break;
                default:
                    //4 director
                    // 7,8,10 nacionales
                    FiltroNacional.BackgroundColor = Light;
                    Nacional.TextColor = Color.White;
                    break;
            }
        }

        #endregion
    }
}
