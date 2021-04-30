using System;
using Newtonsoft.Json;

namespace AppNutOp.Models
{
    public class Usuario
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("NoPersonal")]
        public string NoPersonal { get; set; }
        [JsonProperty("puesto")]
        public string Puesto { get; set; }
        [JsonProperty("Nombre")]
        public string Nombre { get; set; }
        [JsonProperty("Contrasena")]
        public string Contrasena { get; set; }
        
        [JsonProperty("FechaIngreso")]
        public string FechaIngreso { get; set; }
        [JsonProperty("Perfil")]
        public string Perfil { get; set; }
        [JsonProperty("Nivel")]
        public string Nivel { get; set; }
        [JsonProperty("CorreoE")]
        public string Correo { get; set; }
        [JsonProperty("Sexo")]
        public string Sexo { get; set; }
        [JsonProperty("Status")]
        public string Status { get; set; }

        [JsonProperty("Departamento")]
        public string Departamento { get; set; }
        [JsonProperty("Tienda")]
        public string Tienda { get; set; }

        [JsonProperty("JsonMarcas")]
        public string JsonMarcas { get; set; }
        [JsonProperty("JsonModulos")]
        public string JsonModulos { get; set; }
        [JsonProperty("JsonDepartamento")]
        public string JsonDepartamento { get; set; }

    }
}
