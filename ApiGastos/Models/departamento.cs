using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGastos.Models
{
    public class departamento
    {
        [Key]
        public int id { get; set; }
        public string nombre { get; set; }
    }
}
