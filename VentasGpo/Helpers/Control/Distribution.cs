using System;
using Xamarin.Forms;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
//using Microsoft.AppCenter.Distribute;

using VentasGpo.Portable.Helpers;
using VentasGpo.Views.Popup;

namespace VentasGpo.Helpers
{
    public class Distribution
    {
        public Distribution()
        {
            //MessagingCenter.Subscribe<MensajePop>(this, "UpdateApp", (sender) =>
            //{
            //    Distribute.NotifyUpdateAction(UpdateAction.Update);
            //});
        }

        public void InitAppCenter()
        {
            Debug.WriteLine("Entro a Init AppCenter");
            AppCenter.Start("android=" + Constants.APPCENTER_KEY_ANDROID + ";",
                typeof(Analytics),
                typeof(Crashes));
        }

        public async Task InitAppCenterAsync()
        {
            //Distribute.UpdateTrack = UpdateTrack.Private;
            //Distribute.ReleaseAvailable = OnReleaseAvailable;

            Debug.WriteLine("Entro a Init AppCenter");
            AppCenter.Start("android=" + Constants.APPCENTER_KEY_ANDROID + ";",
                typeof(Analytics),
                typeof(Crashes));

            //bool enabled = await Distribute.IsEnabledAsync();
            //Console.WriteLine("Enabled Distribute " + enabled);
        }

        /*bool OnReleaseAvailable(ReleaseDetails releaseDetails)
        {
            string versionName = releaseDetails.ShortVersion;
            string versionCodeOrBuildNumber = releaseDetails.Version;
            string releaseNotes = releaseDetails.ReleaseNotes;
            Uri releaseNotesUrl = releaseDetails.ReleaseNotesUrl;

            var title = "La versión " + versionName + " se encuentra disponible";
            Task answer;

            Debug.WriteLine("Hi Release");
            if (releaseDetails.MandatoryUpdate)
            {
                //answer = App.Current.MainPage.DisplayAlert(title, releaseNotes, "Instalar ahora");
                answer = App.Control.Mensaje(title, releaseNotes, "Instalar ahora", 1);
            }
            else
            {
                //answer = App.Current.MainPage.DisplayAlert(title, releaseNotes, "Instalar ahora", "Tal vez después...");
                answer = App.Control.Mensaje(title, releaseNotes, "Instalar ahora", "Tal vez después...", 1);
            }

            answer.ContinueWith((task) =>
            {
                if (releaseDetails.MandatoryUpdate || (task as Task<bool>).Result)
                {
                    Distribute.NotifyUpdateAction(UpdateAction.Update);
                }
                else
                {
                    Distribute.NotifyUpdateAction(UpdateAction.Postpone);
                }
            });

            return true;
        }*/
    }
}
