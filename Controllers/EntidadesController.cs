using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text.Json;
using csharpapi.Services;
using BCrypt.Net;

namespace api.Controllers
{
    [Route("api/{nombreProyecto}/{nombreTabla}")]
    [ApiController]
    [Authorize]
    public class EntidadesController : ControllerBase
    {
        private readonly ControlConexion controlConexion;
        private readonly IConfiguration _configuration;

        public EntidadesController(ControlConexion controlConexion, IConfiguration configuration)
        {
            this.controlConexion = controlConexion ?? throw new ArgumentNullException(nameof(controlConexion));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary> 
        /// Endpoint de la raíz de la API. 
        /// Muestra un mensaje de bienvenida con información básica sobre la API. 
        /// </summary> 
        /// <returns>Un mensaje JSON con información de la API.</returns> 
        [AllowAnonymous] 
        [HttpGet("/")] 
        public IActionResult Inicio() 
        { 
            var mensaje = new 
            { 
                Mensaje = "Bienvenido a la API Genérica en C#!", 
                Documentacion = "Para más detalles, visita /swagger", 
                FechaServidor = DateTime.UtcNow 
            }; 
            return Ok(mensaje); 
        } 

        /// <summary>
        /// Obtiene todos los registros de una tabla específica en la base de datos.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Listar(string nombreProyecto, string nombreTabla)
        {
            if (string.IsNullOrWhiteSpace(nombreTabla))
                return BadRequest("El nombre de la tabla no puede estar vacío.");

            try
            {
                var listaFilas = new List<Dictionary<string, object?>>();
                string comandoSQL = $"SELECT * FROM {nombreTabla}";
                controlConexion.AbrirBd();
                var tablaResultados = controlConexion.EjecutarConsultaSql(comandoSQL, null);
                controlConexion.CerrarBd();

                foreach (DataRow fila in tablaResultados.Rows)
                {
                    var propiedadesFila = fila.Table.Columns.Cast<DataColumn>()
                        .ToDictionary(columna => columna.ColumnName,
                                    columna => fila[columna] == DBNull.Value ? null : fila[columna]);
                    listaFilas.Add(propiedadesFila);
                }
                return Ok(listaFilas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifica si la contraseña proporcionada coincide con la contraseña almacenada en la base de datos.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("verificar-contrasena")]
        public IActionResult VerificarContrasena(string nombreProyecto, string nombreTabla, [FromBody] Dictionary<string, string> datos)
        {
            if (string.IsNullOrWhiteSpace(nombreTabla) || datos == null ||
                !datos.ContainsKey("campoUsuario") || !datos.ContainsKey("campoContrasena") ||
                !datos.ContainsKey("valorUsuario") || !datos.ContainsKey("valorContrasena"))
            {
                return BadRequest("Faltan parámetros requeridos.");
            }

            try
            {
                string campoUsuario = datos["campoUsuario"];
                string campoContrasena = datos["campoContrasena"];
                string valorUsuario = datos["valorUsuario"];
                string valorContrasena = datos["valorContrasena"];

                string consultaSQL = $"SELECT {campoContrasena} FROM {nombreTabla} WHERE {campoUsuario} = @ValorUsuario";
                var parametro = CrearParametro("@ValorUsuario", valorUsuario);

                controlConexion.AbrirBd();
                var resultado = controlConexion.EjecutarConsultaSql(consultaSQL, new DbParameter[] { parametro });
                controlConexion.CerrarBd();

                if (resultado.Rows.Count == 0)
                    return NotFound("Usuario no encontrado.");

                string contrasenaHasheada = resultado.Rows[0][campoContrasena]?.ToString() ?? string.Empty;

                if (!contrasenaHasheada.StartsWith("$2"))
                    throw new InvalidOperationException("El hash de la contraseña almacenada no es válido.");

                bool esContrasenaValida = BCrypt.Net.BCrypt.Verify(valorContrasena, contrasenaHasheada);

                return esContrasenaValida ? Ok("Contraseña verificada exitosamente.") : Unauthorized("Contraseña incorrecta.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        private DbParameter CrearParametro(string nombre, object? valor)
        {
            return new SqlParameter(nombre, valor ?? DBNull.Value);
        }
    }
}