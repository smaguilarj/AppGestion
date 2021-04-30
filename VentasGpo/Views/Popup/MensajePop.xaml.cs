using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace VentasGpo.Views.Popup
{
    public partial class MensajePop : PopupPage
    {
        public MensajePop(string titulo, string mensaje, string accept = "", string cancel = "", int origen = 0)
        {
            InitializeComponent();

            Titulo.Text = titulo;
            Mensaje.Text = mensaje;

            if (string.IsNullOrEmpty(accept))
            {
                //Alert default
                CloseButton.IsVisible = false;
                AcceptButton.Text = "Aceptar";

                AcceptButton.Clicked += OnClose;
            }
            else
            {
                if (!string.IsNullOrEmpty(cancel))
                {
                    CloseButton.Text = cancel;
                    CloseButton.Clicked += OnClose;
                }
                else
                {
                    CloseButton.IsVisible = false;
                }

                AcceptButton.Text = accept;
                OnActionOrigen(origen);
            }
        }

        void OnActionOrigen(int origen)
        {
            switch (origen)
            {
                case 1:
                    // update version app center
                    AcceptButton.Clicked += OnAcceptInstall;
                    break;
                default:
                    AcceptButton.Clicked += OnClose;
                    break;
            }
        }

        async void OnClose(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        async void OnAcceptInstall(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, "UpdateApp");
            await PopupNavigation.Instance.PopAsync();
        }
    }
}
