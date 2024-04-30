using LibraryExample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Data.SqlClient;
using System.Data.Common;
using System.Diagnostics;

namespace LibraryExample.Controllers
{
    public class KnjigeController(IConfiguration configuration, ZanrController zanrController, AutorKnjigeController autorKnjigeController) : Controller
    {
        public string? connectionString = configuration.GetConnectionString("DefaultConnection");

        [HttpGet]
        [Route("api/Knjige")]
        public async Task<IActionResult> GetBooks()
        {
            var allBooks = new List<Knjige>();
            try
            {
                using (DbConnection connection = SqlClientFactory.Instance.CreateConnection())
                {
                    connection.ConnectionString = connectionString;
                    await connection.OpenAsync();
                    DbCommand command = connection.CreateCommand();
                    command.CommandText = @"
                    SELECT Knjige.ID, ime_knjige, autorKnjigeId, zanrId, datum_unosa, ime_autora, godina_rodjenja, ime_zanra from 
                    knjige LEFT join AutorKnjige ON Knjige.autorKnjigeId = AutorKnjige.ID 
                    left join Zanrovi ON Knjige.zanrId = Zanrovi.ID";

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var bookAuthor = new AutorKnjige((int)reader["autorKnjigeId"], (string)reader["ime_autora"], (int)reader["godina_rodjenja"]);
                            var bookGenre = new Zanr((int)reader["zanrId"], (string)reader["ime_zanra"]);
                            var bookToAdd = new Knjige((int)reader["ID"], (string)reader["ime_knjige"], bookAuthor, bookGenre, (DateTime) reader["datum_unosa"]);

                            allBooks.Add(bookToAdd);
                        }

                    }
                    connection.Close();
                }

                return Ok(Json(allBooks));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return StatusCode(500);
            }
        }
        
        [HttpGet]
        [Route("api/Knjige/{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            try
            {
                using DbConnection connection = SqlClientFactory.Instance.CreateConnection();
                connection.ConnectionString = connectionString;
                await connection.OpenAsync();
                DbCommand command = connection.CreateCommand();
                command.CommandText = String.Format(@"
                    SELECT ime_knjige, datum_unosa, ime_autora, godina_rodjenja, ime_zanra from 
                    knjige LEFT join AutorKnjige ON Knjige.autorKnjigeId = AutorKnjige.ID 
                    left join Zanrovi ON Knjige.zanrId = Zanrovi.ID
                    WHERE Knjige.ID = {0};", id);

                using DbDataReader reader = await command.ExecuteReaderAsync();
                await reader.ReadAsync();
                if (reader.HasRows)
                {
                    var bookAuthor = new AutorKnjigeView((string)reader["ime_autora"], (int)reader["godina_rodjenja"]);
                    var bookGenre = new ZanrView((string)reader["ime_zanra"]);
                    var bookToAdd = new KnjigeView((string)reader["ime_knjige"], bookAuthor, bookGenre, (DateTime)reader["datum_unosa"]);
                 
                    connection.Close();
                    return Ok(Json(bookToAdd));
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
        [Route("api/Knjige/{bookName}/{autorKnjigeID}/{zanrID}")]
        public async Task<IActionResult> CreateBook(string bookName, int autorKnjigeID, int zanrID)
        {
            try
            {
                var checkAutor = await autorKnjigeController.GetBookAuthor(autorKnjigeID);
                var checkZanr = await zanrController.GetGenre(zanrID);
                if(((IStatusCodeActionResult)checkAutor).StatusCode == 404 || ((IStatusCodeActionResult)checkZanr).StatusCode == 404)
                    return BadRequest();

                using DbConnection connection = SqlClientFactory.Instance.CreateConnection();
                connection.ConnectionString = connectionString;
                await connection.OpenAsync();

                DbCommand command = connection.CreateCommand();

                command.CommandText = String.Format(@"
                INSERT INTO Knjige(ime_knjige, autorKnjigeId,zanrId) VALUES ('{0}', {1}, {2});", bookName, autorKnjigeID, zanrID);

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
        [Route("api/Knjige/{idKnjige}/{bookName}/{autorKnjigeID}/{zanrID}")]
        public async Task<IActionResult> UpdateBook(int idKnjige, string bookName, int autorKnjigeID, int zanrID)
        {
            try
            {
                var checkAutor = await autorKnjigeController.GetBookAuthor(autorKnjigeID);
                var checkZanr = await zanrController.GetGenre(zanrID);
                if (((IStatusCodeActionResult)checkAutor).StatusCode == 404 || ((IStatusCodeActionResult)checkZanr).StatusCode == 404)
                    return BadRequest();
                using DbConnection connection = SqlClientFactory.Instance.CreateConnection();
                connection.ConnectionString = connectionString;
                await connection.OpenAsync();
                DbCommand command = connection.CreateCommand();
                command.CommandText = String.Format(@"
                    UPDATE Knjige
                    SET ime_knjige = '{0}', autorKnjigeId = {1}, zanrID = {2}
                    OUTPUT INSERTED.ime_knjige
                    WHERE ID = {3};", bookName, autorKnjigeID, zanrID, idKnjige);

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
        [Route("api/Knjige/{id}")]
        public async Task<IActionResult> DeleteKnjige(int id)
        {
            try
            {
                using DbConnection connection = SqlClientFactory.Instance.CreateConnection();
                connection.ConnectionString = connectionString;
                await connection.OpenAsync();
                DbCommand command = connection.CreateCommand();
                command.CommandText = String.Format(@"
                    DELETE FROM Knjige
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
