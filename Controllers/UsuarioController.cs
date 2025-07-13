using BFlota;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Xml.Linq;
using Newtonsoft.Json;



namespace ApiFlota.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : Controller
    {
        private readonly IConfiguration _config;

        public UsuarioController(IConfiguration config)
        {
            _config = config;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult> GetUsuario([FromBody] Usuario usuario)
        {

            var cadenaConexion = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                .Build()
                .GetSection("ConnectionStrings")["Conexion"];

            XDocument xmlParam = ApiFlota.Shared.DBXmlMethods.GetXml(usuario);
            DataSet dsResultado = await ApiFlota.Shared.DBXmlMethods.EjecutaBase(ApiFlota.Shared.NameStoreProcedure.SP_GetNameUsuario, cadenaConexion, usuario.Transaccion, xmlParam.ToString());

            if (dsResultado.Tables.Count > 0)
            {
                try
                {
                    if (dsResultado.Tables[0].Rows.Count > 0)
                    {
                        Usuario usuario1 = new Usuario();
                        usuario1.Id = Convert.ToInt32(dsResultado.Tables[0].Rows[0]["id"]);
                        usuario1.Cedula = dsResultado.Tables[0].Rows[0]["cedula"].ToString();

                        return Ok(JsonConvert.SerializeObject(CrearToken(usuario1)));
                    }
                    else
                    {
                        /*Resultados objresponse = new Resultados();
                        objresponse.Leyenda = "Error en las credenciales de acceso";
                        objresponse.Respuesta = "Error";*/
                        return Ok(JsonConvert.SerializeObject("Error credenciales"));
                    }


                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                    return BadRequest(ex.ToString());

                }
            }

            return Ok(dsResultado);

        }

        private string CrearToken(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Cedula)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = creds,
                Expires = DateTime.Now.AddDays(1),
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }
    }
}
