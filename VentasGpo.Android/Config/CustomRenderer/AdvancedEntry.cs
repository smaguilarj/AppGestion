using System;
using Android.Content;
using Android.Graphics.Drawables;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using FormsApp.Droid.Config;

[assembly: ExportRenderer(typeof(Entry), typeof(AdvancedEntryRenderer))]
namespace FormsApp.Droid.Config
{
    public class AdvancedEntryRenderer : EntryRenderer
    {
        public AdvancedEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                GradientDrawable gd = new GradientDrawable();
                gd.SetStroke(0, Android.Graphics.Color.Transparent);
                Control.SetBackground(gd);
            }
        }
    }
}