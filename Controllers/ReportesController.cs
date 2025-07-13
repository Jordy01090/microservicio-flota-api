using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using BibliotecaProyecto;
using Gestion_Flota.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Gestion_Flota.Controllers
{
    [ApiController]
    [Route("api/reportes")]
    public class ReportesController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ReportesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET api/reportes
        [HttpGet]
        public async Task<IActionResult> GetReportes()
        {
            try
            {
                // Cadena de conexión desde appsettings.json
                var cadenaConexion = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build()
                    .GetSection("ConnectionStrings")["Conexion"];

                // Generar XML de parámetros vacíos
                XDocument xmlParam = DBXmlMethods.GetXml(new { });

                // Ejecutar el SP y obtener DataSet con los 5 result-sets
                DataSet ds = await DBXmlMethods.EjecutaBase(
                    nombreProcedimiento: "GetReportes",
                    cadenaConexion: cadenaConexion,
                    proceso: "CONSULTAR",
                    dataXML: xmlParam?.ToString() ?? string.Empty
                );

                if (ds == null || ds.Tables.Count < 5)
                    return NotFound("No se encontraron resultados.");

                // 1) Estado de Vehículos
                var VehiculosEstados = ds.Tables[0].AsEnumerable()
                    .Select(r => new {
                        EstadoVehiculo = r["EstadoVehiculo"]?.ToString(),
                        Cantidad = Convert.ToInt32(r["Cantidad"])
                    }).ToList();

                // 2) Tipo de Mantenimiento
                var TipoMantenimiento = ds.Tables[1].AsEnumerable()
                    .Select(r => new {
                        TipoMantenimiento = r["TipoMantenimiento"]?.ToString(),
                        Cantidad = Convert.ToInt32(r["Cantidad"])
                    }).ToList();

                // 3) Top 3 Conductores Mejor Evaluados
                var TopConductores = ds.Tables[2].AsEnumerable()
                    .Select(r => new {
                        ConductorId = Convert.ToInt32(r["ConductorId"]),
                        NombreConductor = r["NombreConductor"]?.ToString(),
                        Puntaje = Convert.ToInt32(r["Puntaje"])
                    }).ToList();

                // 4) Estado de Conductores
                var EstadoConductores = ds.Tables[3].AsEnumerable()
                    .Select(r => new {
                        EstadoConductor = r["EstadoConductor"]?.ToString(),
                        Cantidad = Convert.ToInt32(r["Cantidad"])
                    }).ToList();

                // 5) Resultado de la operación
                var resultado = new Resultado
                {
                    Respuesta = ds.Tables[4].Rows[0]["respuesta"]?.ToString(),
                    Leyenda = ds.Tables[4].Rows[0]["leyenda"]?.ToString()
                };

                // Empaquetar respuesta
                var response = new
                {
                    VehiculosEstados,
                    TipoMantenimiento,
                    TopConductores,
                    EstadoConductores,
                    resultado
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al procesar los datos: " + ex.Message);
            }
        }
    }
}
