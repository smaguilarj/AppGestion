using System;
using System.Globalization;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

using VentasGpo.Portable.Services;
using VentasGpo.Portable.Interfaces;
using AppNutOp.DataLayer;
using VentasGpo.Views.Home;

namespace VentasGpo.ViewModels
{
    public class LoginVM : VMBase
    {
        public Command StartSessionCommand { get; }

        public LoginVM()
        {
            StartSessionCommand = new Command(async () => await ExecuteSession());
        }

        async Task ExecuteSession()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
                {
                    await Message.Mensaje("Aviso", "Ingrese los datos para iniciar sesión");

                    IsBusy = false;
                }
                else
                {
                    await Task.Run(async () =>
                    {
                        var result = await UserService.ValidaAcceso(UserName, Password);

                        if (result != null)
                        {
                            SesionUsr SesionUsr = SesionUsr.Instance();
                            await SesionUsr.LoadDataUser(result);

                            // guardar datos local
                            if (IsRemember)
                            {
                                DataConfig.WriteUserData(UserName, Password);
                            }
                            else
                            {
                                DataConfig.ClearfileUsr();
                            }

                            Device.BeginInvokeOnMainThread( () =>
                            {
                                Page page = (Page)Activator.CreateInstance(typeof(MenuPrincipal));
                                Application CurrentApplication = Application.Current;

                                if (CurrentApplication.MainPage is MasterDetailPage master)
                                {
                                    master.Detail = new NavigationPage(page);
                                }
                                //await Navigation.PushAsync(new MenuPrincipal());
                            });
                        }
                        else
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await Message.Mensaje("Aviso", "Verifique su usuario o contraseña");
                            });

                            IsBusy = false;
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                //IsBusy = false;
            }
        }


        string correo;
        public string UserName
        {
            get { return correo; }
            set
            {
                correo = value;
                OnPropertyChanged();
            }
        }

        string pass;
        public string Password
        {
            get { return pass; }
            set
            {
                pass = value;
                OnPropertyChanged();
            }
        }

        bool reco = false;
        public bool IsRemember
        {
            get { return reco; }
            set
            {
                reco = value;
                OnPropertyChanged();
            }
        }
    }
}
