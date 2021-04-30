using System;
using System.ComponentModel;

namespace VentasGpo.Helpers
{
    public class MenuMaster : INotifyPropertyChanged
    {
        private string titulo = "";
        private int numPag = 0;
        private Type type;

        public Type Tipo
        {
            get { return type; }
            set
            {
                type = value;
                OnPropertyChanged();
            }
        }

        public int Pagina
        {
            get { return numPag; }
            set
            {
                numPag = value;
                OnPropertyChanged();
            }
        }

        public string Titulo
        {
            get { return titulo; }
            set
            {
                titulo = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
