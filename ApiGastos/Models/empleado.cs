﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGastos.Models
{
    public class empleado
    {
        [Key]
        public int id { get; set; }
        public string nombre { get; set; }
        public string posicion { get; set; }
         public int supervidorId { get; set; }

        public int departamentoId { get; set; }

    }
}
