using System;
using Xamarin.Forms;

namespace VentasGpo.Helpers
{
    public class Circle : BoxView
    {
        public Circle()
        {
            HorizontalOptions = LayoutOptions.Center;
            VerticalOptions = LayoutOptions.Center;
        }

        private double _size;

        public double Size
        {
            get => _size;
            set
            {
                _size = value;
                OnPropertyChanged();
                Ajustar();
            }
        }

        private void Ajustar()
        {
            HeightRequest = Size;
            WidthRequest = Size;
            CornerRadius = Size / 2;
        }
    }
}
