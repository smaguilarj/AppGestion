using System;
using Xamarin.Forms;

namespace VentasGpo.Helpers
{
    public class RoundedFrame : Frame
    {
        public static new readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof(RoundedFrame), typeof(CornerRadius), typeof(RoundedFrame));

        public RoundedFrame()
        {
            // MK Clearing default values (e.g. on iOS it's 5)
            base.CornerRadius = 0;
        }

        public new CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
    }
}
