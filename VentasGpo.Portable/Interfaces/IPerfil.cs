using System;
using Xamarin.Forms;
using VentasGpo.Portable.Helpers;

namespace VentasGpo.Portable.Interfaces
{
    public interface IPerfil
    {
        EnumPerfil GetPerfil(int perfil);
        Color DarkColor();
        Color LightColor();
    }
}
