using System;
namespace AppNutOp.Models
{
    public class Tienda
    {
        public int IdMarca { get; set; }
        public string Region { get; set; }
        public string Distrito { get; set; }
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class Distrito
    {
        public int IdMarca { get; set; }
        public string Region { get; set; }
        public string Nombre { get; set; }
    }

    public class Regione
    {
        public string Nombre { get; set; }
        public int IdMarca { get; set; }
    }
}
