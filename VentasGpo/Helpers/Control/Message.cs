using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;

using VentasGpo.ViewModels.Modulos;
using VentasGpo.Views.Popup;

namespace VentasGpo.Helpers
{
    public class Message
    {
        public Message()
        {
        }

        /// <summary>
        /// Alerta Default
        /// </summary>
        /// <param name="titulo"></param>
        /// <param name="mensaje"></param>
        /// <returns></returns>
        public async Task Mensaje(string titulo, string mensaje)
        {
            var propertiedPopup = new MensajePop(titulo, mensaje)
            {
                BackgroundColor = Color.FromRgba(0, 0, 0, 0.7),
                CloseWhenBackgroundIsClicked = false,
            };

            await PopupNavigation.Instance.PushAsync(propertiedPopup);
        }

        /// <summary>
        /// Alert accept
        /// </summary>
        /// <param name="titulo"></param>
        /// <param name="mensaje"></param>
        /// <param name="accept"></param>
        /// <param name="origen"></param>
        /// <returns></returns>
        public async Task Mensaje(string titulo, string mensaje, string accept, int origen = 0)
        {
            var propertiedPopup = new MensajePop(titulo, mensaje, accept, "", origen)
            {
                BackgroundColor = Color.FromRgba(0, 0, 0, 0.7),
                CloseWhenBackgroundIsClicked = false,
            };

            await PopupNavigation.Instance.PushAsync(propertiedPopup);
        }

        /// <summary>
        /// Alert accept / cancel
        /// </summary>
        /// <param name="titulo"></param>
        /// <param name="mensaje"></param>
        /// <param name="accept"></param>
        /// <param name="cancel"></param>
        /// <param name="origen"></param>
        /// <returns></returns>
        public async Task Mensaje(string titulo, string mensaje, string accept, string cancel, int origen = 0)
        {
            var propertiedPopup = new MensajePop(titulo, mensaje, accept, cancel, origen)
            {
                BackgroundColor = Color.FromRgba(0, 0, 0, 0.7),
                CloseWhenBackgroundIsClicked = false,
            };

            await PopupNavigation.Instance.PushAsync(propertiedPopup);
        }

        #region Ventas

        /// <summary>
        /// Lista de opciones de regiones
        /// </summary>
        /// <param name="titulo"></param>
        /// <param name="vs"></param>
        /// <returns></returns>
        public async Task PopListaFiltro(string titulo, VentasVM vs)
        {
            var propertiedPopup = new Views.Menu.Ventas.Popup.ListFiltro(titulo, vs)
            {
                BackgroundColor = Color.FromRgba(0, 0, 0, 0.7),
                CloseWhenBackgroundIsClicked = false,
            };

            await PopupNavigation.Instance.PushAsync(propertiedPopup);
        }

        /// <summary>
        /// Lista de opciones de la busqueda
        /// </summary>
        /// <param name="vs"></param>
        /// <returns></returns>
        public async Task PopListaSearch(VentasVM vs)
        {
            var propertiedPopup = new Views.Menu.Ventas.Popup.ListSearch(vs)
            {
                BackgroundColor = Color.FromRgba(0, 0, 0, 0.7),
                CloseWhenBackgroundIsClicked = false,
            };

            await PopupNavigation.Instance.PushAsync(propertiedPopup);
        }

        #endregion

        #region Actividades

        /// <summary>
        /// Lista de opciones de regiones
        /// </summary>
        /// <param name="titulo"></param>
        /// <param name="vs"></param>
        /// <returns></returns>
        public async Task PopListaFiltro(string titulo, ActividadesVM vs)
        {
            var propertiedPopup = new Views.Menu.Actividades.Popup.ListFiltro(titulo, vs)
            {
                BackgroundColor = Color.FromRgba(0, 0, 0, 0.7),
                CloseWhenBackgroundIsClicked = false,
            };

            await PopupNavigation.Instance.PushAsync(propertiedPopup);
        }

        /// <summary>
        /// Lista de las fechas disponibles a consultar
        /// </summary>
        /// <param name="vs"></param>
        /// <returns></returns>
        public async Task PopListaFechas(ActividadesVM vs)
        {
            var propertiedPopup = new Views.Menu.Actividades.Popup.ListFecha(vs)
            {
                BackgroundColor = Color.FromRgba(0, 0, 0, 0.7),
                CloseWhenBackgroundIsClicked = false,
            };

            await PopupNavigation.Instance.PushAsync(propertiedPopup);
        }

        #endregion
    }
}
