using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using csharpapi.Data.Entities;

namespace csharpapi.Data.Entities
{
    public class Usuario
    {
        public string Email { get; set; }
        public string Contrasena { get; set; }
    }
}