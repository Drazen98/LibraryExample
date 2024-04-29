using LibraryExample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data.Common;
using System.Diagnostics;

namespace LibraryExample.Controllers
{
    public class ZanrController(IConfiguration configuration) : Controller
    {
        public string? connectionString = configuration.GetConnectionString("DefaultConnection");

        [HttpGet]
        [Route("api/Zanr")]
        public async Task<IActionResult> GetGenres()
        {
            var SviAutori = new List<Zanr>();
            try
            {
                using (DbConnection connection = SqlClientFactory.Instance.CreateConnection())
                {
                    connection.ConnectionString = connectionString;
                    await connection.OpenAsync();
                    DbCommand command = connection.CreateCommand();
                    command.CommandText = "SELECT * FROM Zanrovi";

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            SviAutori.Add(new Zanr((int)reader["id"], (string)reader["ime_zanra"]));
                        }

                    }
                    connection.Close();
                }

                return Ok(Json(SviAutori));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return StatusCode(500);
            }
        }
        [HttpGet]
        [Route("api/Zanr/{id}")]
        public async Task<IActionResult> GetGenre(int id)
        {
            try
            {
                using DbConnection connection = SqlClientFactory.Instance.CreateConnection();
                connection.ConnectionString = connectionString;
                await connection.OpenAsync();
                DbCommand command = connection.CreateCommand();
                command.CommandText = String.Format(@"
                    SELECT ime_zanra
                    FROM Zanrovi 
                    WHERE id= {0}", id);

                using DbDataReader reader = await command.ExecuteReaderAsync();
                await reader.ReadAsync();
                if (reader.HasRows)
                {
                    var zanr = new ZanrView((string)reader["ime_zanra"]);
                    connection.Close();
                    return Ok(Json(zanr));
                }
                else
                {
                    connection.Close();
                    return StatusCode(404);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("api/Zanr/{zanrName}")]
        public async Task<IActionResult> CreateGenre(string zanrName)
        {
            try
            {
                using DbConnection connection = SqlClientFactory.Instance.CreateConnection();
                connection.ConnectionString = connectionString;
                await connection.OpenAsync();
                DbCommand command = connection.CreateCommand();
                command.CommandText = String.Format(@"
                INSERT INTO Zanrovi VALUES ('{0}');", zanrName);

                int numberOfAffactedRows = await command.ExecuteNonQueryAsync();
                connection.Close();
                if (numberOfAffactedRows > 0)
                    return Ok();
                else
                    return StatusCode(500);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return StatusCode(500);
            }
        }

        [HttpPut]
        [Route("api/Zanr/{id}/{zanrName}")]
        public async Task<IActionResult> UpdateGenre(int id, string zanrName)
        {
            try
            {
                using DbConnection connection = SqlClientFactory.Instance.CreateConnection();
                connection.ConnectionString = connectionString;
                await connection.OpenAsync();
                DbCommand command = connection.CreateCommand();
                command.CommandText = String.Format(@"
                    UPDATE Zanrovi
                    SET ime_zanra = '{0}'
                    OUTPUT INSERTED.ime_zanra
                    WHERE ID = {1};", zanrName, id);

                using DbDataReader reader = await command.ExecuteReaderAsync();
                await reader.ReadAsync();
                if (reader.HasRows)
                {
                    connection.Close();
                    return Ok();
                }
                else
                {
                    connection.Close();
                    return StatusCode(404);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return StatusCode(500);
            }
        }

        [HttpDelete]
        [Route("api/Zanr/{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            try
            {
                using DbConnection connection = SqlClientFactory.Instance.CreateConnection();
                connection.ConnectionString = connectionString;
                await connection.OpenAsync();
                DbCommand command = connection.CreateCommand();
                command.CommandText = String.Format(@"
                    DELETE FROM Zanrovi
                    WHERE ID = {0};", id);

                int numberOfAffactedRows = await command.ExecuteNonQueryAsync();
                connection.Close();
                if (numberOfAffactedRows > 0)
                    return Ok();
                else
                    return StatusCode(404);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return StatusCode(500);
            }
        }
    }
}
