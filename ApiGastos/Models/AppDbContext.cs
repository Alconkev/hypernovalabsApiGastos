using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGastos.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

            Database.EnsureCreated();
        }
        public DbSet<Models.gasto> Gastos { get; set; }
        public DbSet<Models.detalleGasto> DetalleGasto { get; set; }
        public DbSet<Models.empleado> Empleado { get; set; }
        public DbSet<Models.supervisor> Supervisor { get; set; }
        public DbSet<Models.token> Token { get; set; }

        public DbSet<Models.departamento> Departamento { get; set; }
    }
}
