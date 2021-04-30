using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace VentasGpo.Views.Menu.Ventas
{
    public partial class CartDetails : ContentView
    {
        public delegate void TapDelegate();

        public CartDetails()
        {
            InitializeComponent();
            GoToState("Collapsed");
        }

        public int HeaderHeight { get; private set; } = 60;

        public TapDelegate OnExpandTapped { get; set; }
        public TapDelegate OnCollapseTapped { get; set; }

        private void ExpandTapped(object sender, EventArgs e)
        {
            GoToState("Expanded");
            OnExpandTapped?.Invoke();
        }

        private void CollapseTapped(object sender, EventArgs e)
        {
            GoToState("Collapsed");
            OnCollapseTapped?.Invoke();
        }

        private void GoToState(string visualState)
        {
            VisualStateManager.GoToState(CollapseButton, visualState);
            VisualStateManager.GoToState(ExpandButton, visualState);
        }

        void ExpandPanUpdated(System.Object sender, Xamarin.Forms.PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    break;
                case GestureStatus.Running:
                    break;
                case GestureStatus.Completed:
                    GoToState("Expanded");
                    OnExpandTapped?.Invoke();
                    break;
                case GestureStatus.Canceled:
                    break;
            }
        }

        void CollapsePanUpdated(System.Object sender, Xamarin.Forms.PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    break;
                case GestureStatus.Running:
                    break;
                case GestureStatus.Completed:
                    GoToState("Collapsed");
                    OnCollapseTapped?.Invoke();
                    break;
                case GestureStatus.Canceled:
                    break;
            }
        }
    }
}
