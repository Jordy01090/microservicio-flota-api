using BFlota;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Xml.Linq;
using Newtonsoft.Json;
using Microsoft.Extensions.Hosting;
using System.Transactions;

namespace ApiFlota.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MantenimientosController : Controller
    {
        [Route("[action]")]
        [HttpPost]

        public async Task<ActionResult<Mantenimiento>> GetMantenimiento([FromBody] Mantenimiento mantenimiento)
        {

            var cadenaConexion = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                .Build()
                .GetSection("ConnectionStrings")["Conexion"];

            XDocument xmlParam = ApiFlota.Shared.DBXmlMethods.GetXml(mantenimiento);
            DataSet dsResultado = await ApiFlota.Shared.DBXmlMethods.EjecutaBase(ApiFlota.Shared.NameStoreProcedure.SP_GetName1, cadenaConexion, mantenimiento.Transaccion, xmlParam.ToString());


            List<Mantenimiento> listMantenimiento = new List<Mantenimiento>();

            if (dsResultado.Tables.Count > 0)
            {
                try
                {
                    foreach (DataRow row in dsResultado.Tables[0].Rows)
                    {
                        Mantenimiento objResponse = new Mantenimiento
                        {
                            Id = Convert.ToInt32(row["id"]),
                            Tipo = new TiposMantenimiento
                            {
                                Id = Convert.ToInt32(row["tipo_id"]),
                                Descripcion = row["tipo_descripcion"].ToString()
                            },
                            Descripcion = row["descripcion"].ToString(),
                            Fecha = Convert.ToDateTime(row["fecha"]),
                            Costo = Convert.ToDecimal(row["costo"]),
                            Kilometraje = Convert.ToInt32(row["kilometraje"]),
                            VehiculoId = Convert.ToInt32(row["vehiculo_id"]),
                           
                        
                        };
                        listMantenimiento.Add(objResponse);

                    }

                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());

                }

            }

            return Ok(listMantenimiento);

        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<Respuestas>> SetMantenimiento([FromBody] Mantenimiento mantenimiento)
        {
            var cadenaConexion = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                .Build()
                .GetSection("ConnectionStrings")["Conexion"];

            XDocument xmlParam = ApiFlota.Shared.DBXmlMethods.GetXml(mantenimiento);
            DataSet dsResultado = await ApiFlota.Shared.DBXmlMethods.EjecutaBase(ApiFlota.Shared.NameStoreProcedure.SP_SetName1, cadenaConexion, mantenimiento.Transaccion, xmlParam.ToString());

            List<Respuestas> listResultados = new List<Respuestas>();
            if (dsResultado.Tables.Count > 0)
            {
                try
                {
                    foreach (DataRow row in dsResultado.Tables[0].Rows)
                    {
                        Respuestas objResponse = new Respuestas
                        {
                            Respuesta = row["respuesta"].ToString(),
                            Leyenda = row["leyenda"].ToString(),
                        };
                        listResultados.Add(objResponse);
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                }
            }



            return Ok(listResultados);
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<Respuestas>> SetMantenimiento_y_Consulta([FromBody] Mantenimiento mantenimiento)
        {
            var cadenaConexion = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                .Build()
                .GetSection("ConnectionStrings")["Conexion"];

            XDocument xmlParam = ApiFlota.Shared.DBXmlMethods.GetXml(mantenimiento);
            DataSet dsResultado = await ApiFlota.Shared.DBXmlMethods.EjecutaBase(ApiFlota.Shared.NameStoreProcedure.SP_SetName1, cadenaConexion, mantenimiento.Transaccion, xmlParam.ToString());

            List<Respuestas> listResultados = new List<Respuestas>();
            List<Mantenimiento> listMantenimientos = new List<Mantenimiento>();

            if (dsResultado.Tables.Count > 0)
            {
                try
                {
                    foreach (DataRow row in dsResultado.Tables[0].Rows)
                    {
                        if (row["respuesta"].ToString() == "Ok")
                        {
                            mantenimiento.Transaccion = "CONSULTAR_DESCRIPCION";
                            DataSet dsResultado1 = await ApiFlota.Shared.DBXmlMethods.EjecutaBase(ApiFlota.Shared.NameStoreProcedure.SP_GetName1, cadenaConexion, mantenimiento.Transaccion, xmlParam.ToString());
                            try
                            {
                                foreach (DataRow row1 in dsResultado1.Tables[0].Rows)
                                {
                                    Mantenimiento objResponse = new Mantenimiento
                                    {
                                        Id = Convert.ToInt32(row["id"]),
                                        Tipo = new TiposMantenimiento
                                        {
                                            Id = Convert.ToInt32(row["tipo_id"]),
                                            Descripcion = row["tipo_descripcion"].ToString()
                                        },
                                        Descripcion = row["descripcion"].ToString(),
                                        Fecha = Convert.ToDateTime(row["fecha"]),
                                        Costo = Convert.ToDecimal(row["costo"]),
                                        Kilometraje = Convert.ToInt32(row["kilometraje"]),
                                        VehiculoId = Convert.ToInt32(row["vehiculo_id"]),


                                    };
                                    listMantenimientos.Add(objResponse);

                                }

                            }
                            catch (Exception ex)
                            {
                                Console.Write(ex.ToString());

                            }



                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                }
            }



            return Ok(listMantenimientos);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<Mantenimiento>> GetMantenimientoDescripcion(String Descripcion, String Transaccion)
        {
            Mantenimiento mantenimiento = new Mantenimiento();
            mantenimiento.Descripcion = Descripcion;
            mantenimiento.Transaccion = Transaccion;

            var cadenaConexion = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                .Build().GetSection("ConnectionStrings")["Conexion"];

            XDocument xmlParam = ApiFlota.Shared.DBXmlMethods.GetXml(mantenimiento);
            DataSet dsResultado = await ApiFlota.Shared.DBXmlMethods.EjecutaBase(ApiFlota.Shared.NameStoreProcedure.SP_GetName1, cadenaConexion, mantenimiento.Transaccion, xmlParam.ToString());

            List<Mantenimiento> listMantenimientos = new List<Mantenimiento>();

            if (dsResultado.Tables.Count > 0)
            {
                try
                {
                    foreach (DataRow row in dsResultado.Tables[0].Rows)
                    {
                        Mantenimiento objResponse = new Mantenimiento
                        {
                            Id = Convert.ToInt32(row["id"]),
                            Tipo = new TiposMantenimiento
                            {
                                Id = Convert.ToInt32(row["tipo_id"]),
                                Descripcion = row["tipo_descripcion"].ToString()
                            },
                            Descripcion = row["descripcion"].ToString(),
                            Fecha = Convert.ToDateTime(row["fecha"]),
                            Costo = Convert.ToDecimal(row["costo"]),
                            Kilometraje = Convert.ToInt32(row["kilometraje"]),
                            VehiculoId = Convert.ToInt32(row["vehiculo_id"]),


                        };
                        listMantenimientos.Add(objResponse);
                    }

                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());

                }

            }

            return Ok(listMantenimientos);

        }
    }
}
