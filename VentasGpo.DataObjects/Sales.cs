using System;
using System.Collections.Generic;
using System.Text;

namespace AppNutOp.Models
{
    #region SalesProduccion

    public class Sales
    {
        public List<SalesByPerfil> ListaNewData { get; set; }
        public SalesResultTotal Resultado { get; set; }
    }

    public class NewSalesTemp
    {
        public List<Dictionary<string, object>> ListaNewData { get; set; }
        public Dictionary<string, object> Resultado { get; set; }
    }

    public class SalesByPerfil
    {
        public int Hora { get; set; }
        public double VentaNeta { get; set; }
        public double Visitas { get; set; }
        public double TicketPromedio { get; set; }
        public string Bandera { get; set; }
    }

    public class SalesResultTotal
    {
        public string Region { get; set; }
        public string Distrito { get; set; }
        public string Sucursal { get; set; }

        //***  Ventas ***
        public string VentaActual { get; set; }
        public string PorcentageVenta { get; set; }

        //***  Visitas ***
        public string VisitasTotales { get; set; }
        public string PorcentageVisitas { get; set; }

        //***  T_promedio ***
        public string TicketPromedio { get; set; }
        public string PorcentageTicket { get; set; }

        //***  FCST ***
        public string VentaFC { get; set; }
        public string TotalFC { get; set; }
        public string PorcentageFC { get; set; }
    }


    #endregion
}

