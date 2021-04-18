using ApiGastos.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGastos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GastosController : ControllerBase
    {
        private readonly AppDbContext DbContext;

        public GastosController(AppDbContext AppDbContext)
        {
            this.DbContext = AppDbContext;
        }

        // GET api/values
        [Route("getAll")]
        [HttpGet]
        public ActionResult getAll()
        {

            try
            {
                var listGastos = DbContext.Gastos.ToList();
                List<responseGasto> datos = new List<responseGasto>();
                
                if (listGastos.Count > 0) {

                    for (int i = 0; i < listGastos.Count; i++)
                    {
                        var empleado = DbContext.Empleado.Where(f => f.id == listGastos[i].empleadoId).FirstOrDefault();
                        var supervisor = DbContext.Supervisor.Where(f => f.id == empleado.supervidorId).FirstOrDefault();
                        responseGasto registro = new responseGasto { 
                            id = listGastos[i].id,
                            concepto = listGastos[i].concepto,
                            empleadoId = empleado.id,
                            nombre = empleado.nombre,
                            supervisorId= supervisor.id,
                            supervisor = supervisor.nombre,
                            aprobado = listGastos[i].aprobador,
                            fechadesde = listGastos[i].desde.ToString("yyyy-MM-dd"),
                            fechahasta = listGastos[i].hasta.ToString("yyyy-MM-dd"),
                            fecha = listGastos[i].fechaRegistro.ToString("yyyy-MM-dd"),
                            total = listGastos[i].total

                        };
                        datos.Add(registro);
                    }
                
                }

                return Ok(datos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }


        // GET api/values/5
        [Route("getGastoById")]
        [HttpGet("{id}")]
        public ActionResult getGastoById(int id)
        {
            try
            {
                var gasto = DbContext.Gastos.FirstOrDefault(f => f.id == id);

                if (gasto != null)
                {
                    var empleado = DbContext.Empleado.Where(f => f.id == gasto.empleadoId).FirstOrDefault();

                    if (empleado == null)
                    {
                        return BadRequest("error, el empleado no exite");
                    }

                    var supervisor = DbContext.Supervisor.Where(f => f.id == empleado.supervidorId).FirstOrDefault();

                    if (supervisor == null)
                    {
                        return BadRequest("error, el supervisor no exite");
                    }

                    var departamento = DbContext.Departamento.Where(f => f.id == empleado.departamentoId).FirstOrDefault();
                    if (departamento == null)
                    {
                        return BadRequest("error, el departamento no exite");
                    }

                    var detallesDelGasto = DbContext.DetalleGasto.Where(f => f.gastoId == id).ToList();

                    if (detallesDelGasto.Count == 0)
                    {
                        return BadRequest("error, no hay detalles del gasto");
                    }

                    List<detalles> losDetalles = new List<detalles>();

                    for (int i = 0; i < detallesDelGasto.Count; i++)
                    {
                        detalles detalleInformacion = new detalles
                        {
                            fecha = detallesDelGasto[i].fecha,
                            cuenta = detallesDelGasto[i].cuenta,
                            descripcion = detallesDelGasto[i].descripcion,
                            total = detallesDelGasto[i].monto

                        };
                        losDetalles.Add(detalleInformacion);
                    }

                    responseGasto datoGasto = new responseGasto
                    {

                        concepto = gasto.concepto,
                        fechadesde = gasto.desde.ToString(),
                        fechahasta = gasto.hasta.ToString(),
                        nombre = empleado.nombre,
                        empleadoId = empleado.id,
                        departamento = departamento.nombre,
                        departamentoId = departamento.id,
                        posicion = empleado.posicion,
                        supervisor = supervisor.nombre,
                        supervisorId = supervisor.id,
                        detallesgasto = losDetalles
                    };
                    return Ok(datoGasto);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        // POST api/values
        [Route("setGasto")]
        [HttpPost]
        public ActionResult setGasto([FromBody] requestgasto gasto)
        {
            try
            {
                var supervisorId = 0;
                var departamentoId = 0;
                var empleadoId = 0;

                var existeSupervisor = DbContext.Supervisor.Where(f => f.nombre.ToUpper() == gasto.supervisor.ToUpper()).FirstOrDefault();

                if (existeSupervisor == null)
                {
                    supervisor nuevoSupervisor = new supervisor
                    {
                        nombre = gasto.supervisor
                    };

                    DbContext.Supervisor.Add(nuevoSupervisor);
                    DbContext.SaveChanges();

                    supervisorId = nuevoSupervisor.id;
                }
                else
                {
                    supervisorId = existeSupervisor.id;
                }

                var existeDepartamento = DbContext.Departamento.Where(f => f.nombre.ToUpper() == gasto.departamento.ToUpper()).FirstOrDefault();

                if (existeDepartamento == null)
                {
                    departamento nuevoDepartamento = new departamento
                    {
                        nombre = gasto.departamento
                    };

                    DbContext.Departamento.Add(nuevoDepartamento);
                    DbContext.SaveChanges();

                    departamentoId = nuevoDepartamento.id;
                }
                else
                {
                    departamentoId = existeDepartamento.id;
                }

                var existeEmpelado = DbContext.Empleado.Where(f => f.nombre.ToUpper() == gasto.nombre.ToUpper() && f.posicion.ToUpper() == gasto.posicion.ToUpper() && f.supervidorId == supervisorId && f.departamentoId == departamentoId).FirstOrDefault();

                if (existeEmpelado == null)
                {
                    empleado nuevoempleado = new empleado
                    {
                        nombre = gasto.nombre,
                        posicion = gasto.posicion,
                        departamentoId = departamentoId,
                        supervidorId = supervisorId
                    };

                    DbContext.Empleado.Add(nuevoempleado);
                    DbContext.SaveChanges();

                    empleadoId = nuevoempleado.id;
                }
                else
                {
                    empleadoId = existeEmpelado.id;
                }

                var monto = 0.0;

                for (int i = 0; i < gasto.detallesgasto.Count(); i++)
                {
                    monto += gasto.detallesgasto[i].total;
                }

                gasto nuevoGasto = new gasto
                {
                    aprobador = gasto.aprobado,
                    concepto = gasto.concepto,
                    fechaRegistro = DateTime.Now,
                    desde = gasto.fechadesde,
                    hasta = gasto.fechahasta,
                    empleadoId = empleadoId,
                    total = monto
                };

                DbContext.Gastos.Add(nuevoGasto);
                DbContext.SaveChanges();


                for (int i = 0; i < gasto.detallesgasto.Count(); i++)
                {
                    detalleGasto nuevoDetalle = new detalleGasto
                    {
                        fecha = gasto.detallesgasto[i].fecha,
                        cuenta = gasto.detallesgasto[i].cuenta,
                        descripcion = gasto.detallesgasto[i].descripcion,
                        monto = gasto.detallesgasto[i].total,
                        gastoId = nuevoGasto.id
                    };
                    DbContext.DetalleGasto.Add(nuevoDetalle);
                    DbContext.SaveChanges();
                }

                return Ok(nuevoGasto);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

    }

    public class requestgasto
    {
        public string concepto { get; set; }
        public DateTime fechadesde { get; set; }

        public DateTime fechahasta { get; set; }

        public string nombre { get; set; }
        public string departamento { get; set; }
        public string posicion { get; set; }
        public string supervisor { get; set; }

        public List<detalles> detallesgasto { get; set; }

        public string aprobado { get; set; }
    }

    public class responseGasto
    {
        public int id { get; set; }
        public string concepto { get; set; }

        public string fecha { get; set; }
        public string fechadesde { get; set; }

        public string fechahasta { get; set; }
        public int empleadoId { get; set; }
        public string nombre { get; set; }
        public int departamentoId { get; set; }
        public string departamento { get; set; }
        public string posicion { get; set; }
        public int supervisorId { get; set; }
        public string supervisor { get; set; }

        public List<detalles> detallesgasto { get; set; }

        public string aprobado { get; set; }

        public double total { get; set; }

    }

    public class detalles
    {
        public DateTime fecha { get; set; }
        public string cuenta { get; set; }
        public string descripcion { get; set; }
        public double total { get; set; }
    }

}