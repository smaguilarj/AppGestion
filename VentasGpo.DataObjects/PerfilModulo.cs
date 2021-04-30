using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AppNutOp.Models
{
    public class PerfilModulo
    {
        [JsonProperty("id_modulo")]
        public string IdModulo { get; set; }
        [JsonProperty("Descripcion")]
        public string Descripcion { get; set; }
        [JsonProperty("Image")]
        public string Image { get; set; }
        [JsonProperty("id_modulo_padre")]
        public string IdModuloPadre { get; set; }

        public List<PerfilModulo> Perfiles { get; set; }
    }
}
