using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace csharpapi.Data.Entities
{
    public class Producto
    {
        public int Codigo { get; set; }
        public string Nombre { get; set; }
        public int Stock { get; set; }
    }
}