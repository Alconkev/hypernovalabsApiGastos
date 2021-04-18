using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGastos.Models
{
    public class detalleGasto
    {
        [Key]
        public int id { get; set; }
        public DateTime fecha { get; set; }
        public string cuenta { get; set; }
        public string descripcion { get; set; }

        public double monto { get; set; }

        [ForeignKey("gasto")]
        public int gastoId { get; set; }


    }
}
