using System;
using Xamarin.Forms;
using System.Collections.Generic;

namespace AppNutOp.Models
{
    public class Activities
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string FechaRegistro { get; set; }
        public string FechaInicio { get; set; }
        public string FechaTermino { get; set; }
        public int Estatus { get; set; }

        public string ModoTiendas { get; set; }
        public string ModoZona { get; set; }

        public int NumTiendasTarget { get; set; }
        public int NumTiendasDone { get; set; }

        public List<Tienda> Tiendas { get; set; }
        public List<UserActividad> Usuarios { get; set; }
    }

    public class UserActividad
    {
        public int IdActividad { get; set; }
        public int IdTienda { get; set; }
        public string Tienda { get; set; }
        public string NoEmpleado { get; set; }
        public string Nombre { get; set; }
        public string Reporte { get; set; }
        public string FechaEntrega { get; set; }
        public int Estatus { get; set; }
    }

    public class AuxLugar
    {
        public string Nombre { get; set; }
        public int NumTiendasTarget { get; set; }
        public int NumTiendasDone { get; set; }
    }

    public class AuxDetalles
    {
        public int IdActividad { get; set; }
        public string Descripcion { get; set; }
        public double Porcentaje { get; set; }
        public string PorcentajeTxt { get; set; }
        public Color PorcentajeColor { get; set; }
    }
}
