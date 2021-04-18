using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGastos.Models
{
    public class token
    {
        [Key]
        public int id { get; set; }
        public string aplicacion { get; set; }
        public string apikey { get; set; }
    }
}
