using System;
using Xamarin.Forms;
using VentasGpo.Portable.Interfaces;

[assembly: Dependency(typeof(VentasGpo.Portable.Helpers.FilterPerfil))]
namespace VentasGpo.Portable.Helpers
{
    public class FilterPerfil : IPerfil
    {
        public EnumPerfil GetPerfil(int nivel)
        {
            EnumPerfil myPerfil;

            switch (nivel)
            {
                case 2: //regional nutrisa
                    myPerfil = EnumPerfil.Regional;
                    break;
                case 3: //distrital:
                    myPerfil = EnumPerfil.Distrital;
                    break;
                case 4: //sucursal
                    myPerfil = EnumPerfil.Tienda;
                    break;
                default:
                    // nacionales
                    myPerfil = EnumPerfil.Nacional;
                    break;
            }

            return myPerfil;
        }

        public Color DarkColor()
        {
            Color DarkColor;
            if (Application.Current.Properties.ContainsKey("marca_id"))
            {
                var cc = (string)Application.Current.Properties["marca_id"];
                switch (cc)
                {
                    case "3":
                        DarkColor = (Color)Application.Current.Resources["CielitoDark"];
                        break;
                    case "2":
                        DarkColor = (Color)Application.Current.Resources["LavazzaDark"];
                        break;
                    default:
                        //nutrisa
                        DarkColor = (Color)Application.Current.Resources["NutrisaDark"];
                        break;
                }
            }
            else
            {
                //nutrisa
                DarkColor = (Color)Application.Current.Resources["NutrisaDark"];
            }

            return DarkColor;
        }

        public Color LightColor()
        {
            Color LightColor;
            if (Application.Current.Properties.ContainsKey("marca_id"))
            {
                var cc = (string)Application.Current.Properties["marca_id"];
                switch (cc)
                {
                    case "3":
                        LightColor = (Color)Application.Current.Resources["CielitoLight"];
                        break;
                    case "2":
                        LightColor = (Color)Application.Current.Resources["LavazzaLight"];
                        break;
                    default:
                        //nutrisa
                        LightColor = (Color)Application.Current.Resources["NutrisaLight"];
                        break;
                }
            }
            else
            {
                //nutrisa
                LightColor = (Color)Application.Current.Resources["NutrisaLight"];
            }

            return LightColor;
        }
    }
}
