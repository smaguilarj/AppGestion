using System;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Threading.Tasks;

namespace VentasGpo.Helpers
{
    public class Platform
    {
        public Platform()
        {
        }

        public void GetInit()
        {
            App.IsAndroid = Device.RuntimePlatform == Device.Android;

            if (App.IsAndroid)
            {
                App.IsiOSX = false;
                App.GetAppCenter.InitAppCenter();
            }
            else
            {
                ValidateModeliOS();
            }
        }

        private void ValidateModeliOS()
        {
            if (DeviceInfo.DeviceType == DeviceType.Physical)
            {
                //validar si es iphone X o mayor
                string model = DeviceInfo.Model.Substring(6);
                string[] iphone = model.Split(',');

                int id1 = Convert.ToInt32(iphone[0]);
                int id2 = Convert.ToInt32(iphone[1]);
                
                if (id1 > 10)
                {
                    App.IsiOSX = true;
                }
                else if (id1 == 10)
                {
                    //diferenciar de iphone 8
                    //iphone 8 => (10,1 10,4) iphone 8 plus => (10,2 10,5)
                    //iphone x => (10,3 10,6)
                    App.IsiOSX = (id2 == 3 || id2 == 6);
                }
                else
                {
                    App.IsiOSX = false;
                }
            }
            else
            {
                App.IsiOSX = true;
            }
        }

        public async Task PushNavigation(INavigation navigation, Page page)
        {
            if (App.IsAndroid)
            {
                await navigation.PushAsync(page);
            }
            else
            {
                if (App.IsiOSX)
                {
                    Application CurrentApplication = Application.Current;
                    if (CurrentApplication.MainPage is MasterDetailPage master)
                    {
                        master.Detail = new NavigationPage(page);
                    }
                }
                else
                {
                    await navigation.PushModalAsync(page);
                }
            }
        }
    }
}
