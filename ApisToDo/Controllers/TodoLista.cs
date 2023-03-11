using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace ApisToDo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoLista : ControllerBase
    {
        string cnnString;
        public TodoLista(IConfiguration configuration)
        {
            cnnString = configuration.GetConnectionString("ConnectionStringToDo");
        }

        [HttpGet]
        public async Task<IActionResult> GetInfo()
        {
            try
            {

                DataTable dtRespuestaApi = new DataTable();
                using (SqlConnection sqlConnection = new SqlConnection(cnnString))
                {
                    await sqlConnection.OpenAsync();
                    using (SqlCommand cmdSolicitud = new SqlCommand($"SELECT id_lista,descripcion_lista,fecha_creada,(SELECT COUNT(*) FROM todoTareas WHERE id_lista = tl.id_lista) as tareas_total,(SELECT COUNT(*) FROM todoTareas WHERE id_lista = tl.id_lista AND estado = 1) as tareas_completadas,ISNULL((CAST((SELECT COUNT(*) FROM todoTareas WHERE id_lista = tl.id_lista AND estado = 1) AS FLOAT)/NULLIF(CAST((SELECT COUNT(*) FROM todoTareas WHERE id_lista = tl.id_lista) as FLOAT),0) * 100),0) as porcentaje FROM todoLista tl", sqlConnection))
                    {
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
            public string descripcion_lista { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> PostInfo([FromBody] ModelPost modelDatos)
        {
            try
            {

                using (SqlConnection sqlConnection = new SqlConnection(cnnString))
                {
                    await sqlConnection.OpenAsync();
                    using (SqlCommand cmdSolicitud = new SqlCommand($@"INSERT INTO todoLista VALUES(@descripcion,getdate())", sqlConnection))
                    {
                        cmdSolicitud.Parameters.Add("@descripcion", SqlDbType.VarChar).Value = modelDatos.descripcion_lista;
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
                    using (SqlCommand cmdSolicitud = new SqlCommand($@"UPDATE todoLista SET descripcion_lista = @descripcion WHERE id_lista = @idLista", sqlConnection))
                    {
                        cmdSolicitud.Parameters.Add("@descripcion", SqlDbType.VarChar).Value = modelDatos.descripcion_lista;
                        cmdSolicitud.Parameters.Add("@idLista", SqlDbType.VarChar).Value = modelDatos.id_lista;
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

        [HttpDelete("{idLista}")]
        public async Task<IActionResult> DeleteInfo(string idLista)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(cnnString))
                {
                    await sqlConnection.OpenAsync();
                    using (SqlCommand cmdSolicitud = new SqlCommand($@"DELETE FROM todoLista where id_lista = @idLista", sqlConnection))
                    {
                        cmdSolicitud.Parameters.Add("@idLista", SqlDbType.VarChar).Value = idLista;
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
    }
}
