using System;
using Xamarin.Forms;

using VentasGpo.Helpers;
using VentasGpo.Portable.Helpers;
using VentasGpo.Portable.Services;
using VentasGpo.Portable.Interfaces;

namespace VentasGpo.ViewModels
{
    public class VMBase : MyObservableObject
    {
        public VMBase(INavigation navigation = null, Page next = null)
        {
            Navigation = navigation;
            PageDetail = next;
        }

        public IDataConfig DataConfig => DependencyService.Get<IDataConfig>();
        public IPerfil MyPerfil => DependencyService.Get<IPerfil>();

        // -- Objetos helpers

        private static Message mj;
        public static Message Message
        {
            get { return mj = mj ?? new Message(); }
        }

        // -- Objetos services

        private static UserService us;
        public static UserService UserService
        {
            get { return us = us ?? new UserService(); }
        }

        private static SalesService ss;
        public static SalesService SalesService
        {
            get { return ss = ss ?? new SalesService(); }
        }

        private static ActivitiesService ase;
        public static ActivitiesService ActivitiesService
        {
            get { return ase = ase ?? new ActivitiesService(); }
        }

        // --

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        INavigation navigation;
        public INavigation Navigation
        {
            get { return navigation; }
            set
            {
                navigation = value;
                SetProperty(ref navigation, value);
            }
        }

        Page page;
        public Page PageDetail
        {
            get { return page; }
            set
            {
                page = value;
                SetProperty(ref page, value);
            }
        }
    }
}
