using System;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Collections.Generic;

using VentasGpo.ViewModels.Modulos;
using VentasGpo.Portable.Helpers;
using VentasGpo.Helpers;
using VentasGpo.Views.Menu.Ventas.Popup;

namespace VentasGpo.Views.Menu.Ventas.Templates
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

        public static readonly BindableProperty VentasProperty = BindableProperty.Create(nameof(Ventas),
            typeof(VentasVM), typeof(FilterPlace), default(VentasVM), BindingMode.TwoWay);
        public VentasVM Ventas
        {
            get
            {
                return (VentasVM)GetValue(VentasProperty);
            }

            set
            {
                SetValue(VentasProperty, value);
            }
        }

        int FilNacional;
        int FilRegional;
        int FilDistrital;

        Frame FiltroNacional;
        Frame FiltroRegional;
        Frame FiltroDistrital;
        Frame FiltroSucursal;

        Label Nacional;
        Label Regional;
        Label Distrital;
        Label Sucursal;

        Color Light;
        Color Black;

        EnumPerfil myPerfil;
        List<Departamento> myZonas;

        public FilterPlace()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand;

            ControlInit();
            ColorInit();

            MessagingCenter.Subscribe<ListSearch>(this, "Colorful", (sender) =>
            {
                FiltroRegional.BackgroundColor = Light;
                Regional.TextColor = Color.White;

                FiltroDistrital.BackgroundColor = Light;
                Distrital.TextColor = Color.White;

                FiltroSucursal.BackgroundColor = Light;
                Sucursal.TextColor = Color.White;
            });

            MessagingCenter.Subscribe<ListFiltro, string>(this, "ColorDefault", (sender, args) =>
            {
                ColorDefault(args);
            });

            MessagingCenter.Subscribe<ListFiltro, string>(this, "ColorActive", (sender, args) =>
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
            }
        }

        void LoadBack()
        {
            int botones = 4;
            if (FilNacional == 0) { botones--;  }
            if (FilRegional == 0) { botones--;  }
            if (FilDistrital == 0) { botones--;  }

            Console.WriteLine("== "+botones);
            Console.WriteLine("N_"+FilNacional);
            Console.WriteLine("R_"+FilRegional);
            Console.WriteLine("D_"+FilDistrital);

            double aux = 10 / botones;
            for (var i = 0; i < botones; i++)
            {
                ColumnDefinitions.Add(
                    new ColumnDefinition { Width = new GridLength(aux, GridUnitType.Star) }
                );
            }

            if (botones == 4)
                Children.Add(FiltroNacional, 0, 0);

            if (botones >= 3)
            {
                if(FilRegional == 1)
                    Children.Add(FiltroRegional, botones - 3, 0);
                else if(FilNacional == 1 && FilRegional == 0)
                    Children.Add(FiltroNacional, botones - 3, 0);
            }

            if (botones > 1)
                Children.Add(FiltroDistrital, botones - 2, 0);

            Children.Add(FiltroSucursal, botones - 1, 0);
        }

        #region Functions Botones

        void ClickedNacional(System.Object sender, System.EventArgs e)
        {
            if(FilRegional == 1)
                Ventas.RegionChoice = string.Empty;

            Ventas.DistritoChoice = string.Empty;
            Ventas.TiendaChoice = string.Empty;

            Ventas.IsComp = false;
            Ventas.IsSucursal = true;
            Ventas.IsDetails = true;

            ColorDefault("DR");

            //vs.FilterCommand.Execute(null);
            Ventas.FilterPlaceCommand.Execute(null);
        }

        async void ClickedRegional(System.Object sender, System.EventArgs e)
        {
            Frame button = sender as Frame;
            button.BackgroundColor = Light;
            Regional.TextColor = Color.White;

            ColorDefault("D");

            if (myPerfil == EnumPerfil.Regional && myZonas != null && myZonas.Count == 1)
            {
                Ventas.DistritoChoice = string.Empty;
                Ventas.TiendaChoice = string.Empty;

                Ventas.IsComp = false;
                Ventas.IsSucursal = true;
                Ventas.IsDetails = true;

                Ventas.FilterPlaceCommand.Execute(null);
            }
            else
            {
                Ventas.GetRegionesCommand.Execute(null);
                await App.Control.PopListaFiltro("Región", Ventas);
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
                Ventas.TiendaChoice = string.Empty;

                Ventas.IsComp = false;
                Ventas.IsSucursal = true;
                Ventas.IsDetails = true;

                Ventas.FilterPlaceCommand.Execute(null);
            }
            else
            {
                if (!string.IsNullOrEmpty(Ventas.RegionChoice))
                {
                    Ventas.GetDistritosCommand.Execute(null);
                    await App.Control.PopListaFiltro("Distrito", Ventas);
                }
                else
                {
                    ColorDefault("D");
                    await App.Control.Mensaje("Aviso", "Selecciona una región");
                }
            }
        }

        async void ClickedTienda(System.Object sender, System.EventArgs e)
        {
            Frame button = sender as Frame;
            button.BackgroundColor = Light;
            Sucursal.TextColor = Color.White;

            if (myPerfil == EnumPerfil.Tienda)
            {
                Ventas.FilterPlaceCommand.Execute(null);
            }
            else
            {
                if (!string.IsNullOrEmpty(Ventas.DistritoChoice))
                {
                    Ventas.GetFranquiciasCommand.Execute(null);
                    await App.Control.PopListaFiltro("Tienda", Ventas);
                }
                else
                {
                    ColorDefault("T");
                    await App.Control.Mensaje("Aviso", "Selecciona un distrito");
                }
            }
        }

        #endregion

        #region Styles

        void ColorDefault(string bandera)
        {
            FiltroSucursal.BackgroundColor = Color.White;
            Sucursal.TextColor = Black;

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
            if (bandera.Contains("D"))
            {
                FiltroDistrital.BackgroundColor = Light;
                Distrital.TextColor = Color.White;
            }

            if (bandera.Contains("T"))
            {
                FiltroSucursal.BackgroundColor = Light;
                Sucursal.TextColor = Color.White;
            }
        }

        void ControlInit()
        {
            string marca_act = Application.Current.Properties.ContainsKey("marca_id") ? (string)Application.Current.Properties["marca_id"] : "";

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
            Sucursal = new Label
            {
                Text = "Tienda",
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

            //sucursal
            var tapTie = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1
            };
            tapTie.Tapped += ClickedTienda;

            string icono = (marca_act == "1") ? $"helado.png" : $"cafe.png";
            FiltroSucursal = new Frame()
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
                            Source = icono,
                            Aspect = Aspect.AspectFit,
                            HeightRequest = 30,
                            WidthRequest = 30,
                            Margin = new Thickness(0,0,0,-4)
                        },
                        Sucursal
                    }
                }
            };
            FiltroSucursal.GestureRecognizers.Add(tapTie);

            Black = Color.Black;
            Light = App.MyPerfil.LightColor();

            string depas = Application.Current.Properties.ContainsKey("zonas") ? (string)Application.Current.Properties["zonas"] : "";
            myZonas = JsonConvert.DeserializeObject<List<Departamento>>(depas);
        }

        void ColorInit()
        {
            var perfil = Application.Current.Properties.ContainsKey("nivel") ? (string)Application.Current.Properties["nivel"] : "";
            int my_nivel = int.Parse(perfil);
            myPerfil = App.MyPerfil.GetPerfil(my_nivel);

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
                    FiltroSucursal.BackgroundColor = Light;
                    Sucursal.TextColor = Color.White;
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
