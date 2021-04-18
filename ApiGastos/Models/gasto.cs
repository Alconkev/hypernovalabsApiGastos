using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGastos.Models
{
    public class gasto
    {
        [Key]
        public int id { get; set; }
        public string concepto { get; set; }
        public DateTime fechaRegistro { get; set; }

        public DateTime desde { get; set; }
        public DateTime hasta { get; set; }
        public string aprobador { get; set; }
        public DateTime fechaAprobacion { get; set; }

        [ForeignKey("empleado")]
        public int empleadoId { get; set; }

        [ForeignKey("supervisor")]
        public int supervisorId { get; set; }

        public double total { get; set; }

        public List<detalleGasto> detalleGasto { get; set; }

    }
}
