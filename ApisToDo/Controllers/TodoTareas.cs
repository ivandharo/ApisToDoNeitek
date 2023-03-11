using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System;

namespace ApisToDo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoTareas : ControllerBase
    {
        string cnnString;
        public TodoTareas(IConfiguration configuration)
        {
            cnnString = configuration.GetConnectionString("ConnectionStringToDo");
        }

        [HttpGet("{idLista}")]
        public async Task<IActionResult> GetInfo(string idLista)
        {
            try
            {
                DataTable dtRespuestaApi = new DataTable();
                using (SqlConnection sqlConnection = new SqlConnection(cnnString))
                {
                    await sqlConnection.OpenAsync();
                    using (SqlCommand cmdSolicitud = new SqlCommand("SELECT * FROM todoTareas WHERE id_lista = @idLista", sqlConnection))
                    {
                        cmdSolicitud.Parameters.Add("@idLista", SqlDbType.VarChar).Value = idLista;
                        cmdSolicitud.CommandType = CommandType.Text;
                        cmdSolicitud.CommandTimeout = 0;
                        var reader = await cmdSolicitud.ExecuteReaderAsync();
                        dtRespuestaApi.Load(reader);
                    }
                    await sqlConnection.CloseAsync();
                }
                return Content(JsonConvert.SerializeObject(dtRespuestaApi), "application/json");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public class ModelPost
        {
            public string id_lista { get; set; }
            public string id_tarea { get; set; }
            public string descripcion_tarea { get; set; }
            public bool importante { get; set; }
            public bool estado { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> PostInfo([FromBody] ModelPost modelDatos)
        {
            try
            {

                using (SqlConnection sqlConnection = new SqlConnection(cnnString))
                {
                    await sqlConnection.OpenAsync();
                    using (SqlCommand cmdSolicitud = new SqlCommand($@"INSERT INTO todoTareas VALUES(@idLista,@descripcion,GETDATE(),0,0)", sqlConnection))
                    {
                        cmdSolicitud.Parameters.Add("@idLista", SqlDbType.VarChar).Value = modelDatos.id_lista;
                        cmdSolicitud.Parameters.Add("@descripcion", SqlDbType.VarChar).Value = modelDatos.descripcion_tarea;
                        cmdSolicitud.CommandType = CommandType.Text;
                        cmdSolicitud.CommandTimeout = 0;
                        var reader = await cmdSolicitud.ExecuteNonQueryAsync();
                    }
                    await sqlConnection.CloseAsync();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> PutInfo([FromBody] ModelPost modelDatos)
        {
            try
            {

                using (SqlConnection sqlConnection = new SqlConnection(cnnString))
                {
                    await sqlConnection.OpenAsync();
                    using (SqlCommand cmdSolicitud = new SqlCommand($@"UPDATE todoTareas SET descripcion_tarea = @descripcion,estado = @estado,importante = @impor WHERE id_tarea = @idTarea", sqlConnection))
                    {
                        cmdSolicitud.Parameters.Add("@descripcion", SqlDbType.VarChar).Value = modelDatos.descripcion_tarea;
                        cmdSolicitud.Parameters.Add("@idTarea", SqlDbType.VarChar).Value = modelDatos.id_tarea;
                        cmdSolicitud.Parameters.Add("@estado", SqlDbType.Bit).Value = modelDatos.estado;
                        cmdSolicitud.Parameters.Add("@impor", SqlDbType.Bit).Value = modelDatos.importante;
                        cmdSolicitud.CommandType = CommandType.Text;
                        cmdSolicitud.CommandTimeout = 0;
                        var reader = await cmdSolicitud.ExecuteNonQueryAsync();
                    }
                    await sqlConnection.CloseAsync();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{idTarea}")]
        public async Task<IActionResult> DeleteInfo(string idTarea)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(cnnString))
                {
                    await sqlConnection.OpenAsync();
                    using (SqlCommand cmdSolicitud = new SqlCommand("DELETE FROM todoTareas WHERE id_tarea = @idTarea", sqlConnection))
                    {
                        cmdSolicitud.Parameters.Add("@idTarea", SqlDbType.VarChar).Value = idTarea;
                        cmdSolicitud.CommandType = CommandType.Text;
                        cmdSolicitud.CommandTimeout = 0;
                        await cmdSolicitud.ExecuteNonQueryAsync();
                    }
                    await sqlConnection.CloseAsync();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
