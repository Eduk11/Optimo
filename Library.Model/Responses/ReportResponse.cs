﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Model.Responses
{
    public class ListVenta
    {
        public List<Venta>? ListReport { get; set; }
    }

    public class Prueba()
    {
        public int numero { get; set; }
    }

    public class Venta
    {
        [JsonProperty("IdVenta")]
        public string? IdVenta { get; set; }

        [JsonProperty("nombre")]
        public string? nombre { get; set; }

        [JsonProperty("stock")]
        public int? stock { get; set; }
        
        [JsonProperty("precio")]
        public decimal? precio { get; set; }

    }
}
