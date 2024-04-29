using LibraryExample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Data.Common;
namespace LibraryExample.Controllers
{
    public class AutorKnjigeController(IConfiguration configuration) : Controller
    {
        public string? connectionString = configuration.GetConnectionString("DefaultConnection");

        [HttpGet]
        [Route("api/Autorknjige")]
        public async Task<IActionResult> GetBookAuthors()
        {
            var SviAutori = new List<AutorKnjige>();
            try
            {
                using (DbConnection connection = SqlClientFactory.Instance.CreateConnection())
                {
                    connection.ConnectionString = connectionString;
                    await connection.OpenAsync();
                    DbCommand command = connection.CreateCommand();
                    command.CommandText = "SELECT * FROM AutorKnjige";

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            SviAutori.Add(new AutorKnjige((int)reader["id"], (string)reader["ime_autora"], (int)reader["godina_rodjenja"]));
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
        [Route("api/Autorknjige/{id}")]
        public async Task<IActionResult> GetBookAuthor(int id)
        {
            try
            {
                using DbConnection connection = SqlClientFactory.Instance.CreateConnection();
                connection.ConnectionString = connectionString;
                await connection.OpenAsync();
                DbCommand command = connection.CreateCommand();
                command.CommandText = String.Format(@"
                        SELECT ime_autora,godina_rodjenja
                        FROM AutorKnjige 
                        WHERE id= {0}", id);

                using DbDataReader reader = await command.ExecuteReaderAsync();
                await reader.ReadAsync();
                if (reader.HasRows)
                {
                    var autorKnjige = new AutorKnjigeView((string)reader["ime_autora"], (int)reader["godina_rodjenja"]);
                    connection.Close();
                    return Ok(Json(autorKnjige));
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
        [Route("api/Autorknjige/{authorName}/{yearOfBirth}")]
        public async Task<IActionResult> CreateBookAuthor(string authorName, int yearOfBirth)
        {
            try
            {
                using DbConnection connection = SqlClientFactory.Instance.CreateConnection();
                connection.ConnectionString = connectionString;
                await connection.OpenAsync();
                DbCommand command = connection.CreateCommand();
                command.CommandText = String.Format(@"
                    INSERT INTO AutorKnjige VALUES ('{0}', {1});", authorName, yearOfBirth);

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
        [Route("api/Autorknjige/{id}/{authorName}/{yearOfBirth}")]
        public async Task<IActionResult> UpdateBookAuthor(int id, string authorName, int yearOfBirth)
        {
            try
            {
                using DbConnection connection = SqlClientFactory.Instance.CreateConnection();
                connection.ConnectionString = connectionString;
                await connection.OpenAsync();
                DbCommand command = connection.CreateCommand();
                command.CommandText = String.Format(@"
                        UPDATE AutorKnjige
                        SET ime_autora = '{0}', godina_rodjenja = {1}
                        OUTPUT INSERTED.ime_autora
                        WHERE ID = {2};", authorName, yearOfBirth, id);

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
        [Route("api/Autorknjige/{id}")]
        public async Task<IActionResult> DeleteBookAuthor(int id)
        {
            try
            {
                using DbConnection connection = SqlClientFactory.Instance.CreateConnection();
                connection.ConnectionString = connectionString;
                await connection.OpenAsync();
                DbCommand command = connection.CreateCommand();
                command.CommandText = String.Format(@"
                       DELETE FROM AutorKnjige
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