using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Xamarin.Forms;

using AppNutOp.Models;

namespace VentasGpo.Portable.Helpers
{
    public class Background 
    {
        //public Command ReadRegionesCommand { get; set; }

        public Background()
        {
            //ReadRegionesCommand = new Command<object>(async (x) => await ReadRegiones(x));
        }

        //async Task ReadRegiones(object parameter)
        //{
        //    var values = (object[])parameter;
        //    var perfil = (int)values[0];
        //    var user = (string)values[1];

        //    await Task.Run(async () => {
        //        Debug.WriteLine("Soy la lectura de regiones");

        //        List<ModelSalesGetDataPerfil> cont = await SalesService.RecuperaDataByPerfil(perfil, user);
        //        if (cont != null)
        //        {
        //            var ss = JsonConvert.SerializeObject(cont);
        //            DataStore.Add(ss);
        //        }
        //    });
        //}
    }
}
