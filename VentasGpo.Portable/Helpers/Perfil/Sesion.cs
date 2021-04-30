using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

using AppNutOp.Models;

namespace AppNutOp.DataLayer
{
    public sealed class SesionUsr
    {
        private static SesionUsr _instance = null;

        SesionUsr()
        {
        }

        public static SesionUsr Instance()
        {
            if (_instance == null)
            {
                _instance = new SesionUsr();
            }

            return _instance;
        }

        #region Properties

        public async Task LoadDataUser(Usuario user)
        {
            Application.Current.Properties["id"] = user.Id;
            Application.Current.Properties["name"] = user.Nombre;
            Application.Current.Properties["sexo"] = user.Sexo;
            Application.Current.Properties["user"] = user.Correo;

            Application.Current.Properties["depa"] = user.Departamento.TrimEnd();
            Application.Current.Properties["tienda"] = user.Tienda;
            Application.Current.Properties["perfil"] = user.Perfil;
            Application.Current.Properties["nivel"] = user.Nivel;
            Application.Current.Properties["puesto"] = user.Puesto;

            Application.Current.Properties["marcas"] = user.JsonMarcas;
            Application.Current.Properties["modulos"] = user.JsonModulos;
            Application.Current.Properties["zonas"] = user.JsonDepartamento;

            Application.Current.Properties["isLoggedIn"] = true;

            await Application.Current.SavePropertiesAsync();
        }

        #endregion

    }
}
