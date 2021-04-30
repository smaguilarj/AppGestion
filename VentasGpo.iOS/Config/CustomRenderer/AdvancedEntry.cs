using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using FormsApp.iOS.Config;

[assembly: ExportRenderer(typeof(Entry), typeof(AdvancedEntryRenderer))]
namespace FormsApp.iOS.Config
{
    public class AdvancedEntryRenderer : EntryRenderer
    {
        public AdvancedEntryRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.BackgroundColor = UIColor.FromRGBA(1,1,1,0);
                Control.BorderStyle = UITextBorderStyle.None;
            }
        }
    }
}
