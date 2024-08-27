using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using TodoApp.Models;

namespace TodoApp.Controllers;

[ApiController]
[Route("/todo")]
public class ArticlesController(ILoggerFactory logger) : Controller
{
    private readonly ILogger<ArticlesController> _logger =
        logger.CreateLogger<ArticlesController>();

    [Route("/status")]
    [HttpGet]
    public IActionResult Status()
    {
        _logger.LogInformation("everything is good");
        return Ok();
    }

    [Route("/todo")]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        using NpgsqlConnection connection = GetConnection();

        using var command = new NpgsqlCommand(
            "SELECT * FROM todoItems ORDER BY created ASC;",
            connection
        );

        NpgsqlDataReader reader = await command.ExecuteReaderAsync();

        var itemList = new List<TodoItem>();
        while (reader.Read())
        {
            itemList.Add(new TodoItem { id = reader.GetGuid(0), text = reader.GetString(1) });
        }

        connection.Close();

        return Json(itemList);
    }

    [Route("/todo")]
    [HttpPost]
    public IActionResult Post(TodoItem todoItem)
    {
        using NpgsqlConnection connection = GetConnection();

        using var command = new NpgsqlCommand(
            "INSERT INTO todoItems (id, text, created) VALUES (@id, @text, @created);",
            connection
        );

        todoItem.id = Guid.NewGuid();
        command.Parameters.AddWithValue("id", todoItem.id);
        command.Parameters.AddWithValue("text", todoItem.text);
        command.Parameters.AddWithValue("created", DateTime.UtcNow);

        command.ExecuteNonQuery();
        connection.Close();

        return CreatedAtAction(nameof(Get), new { id = todoItem.id }, todoItem);
    }

    [Route("/todo/{id}")]
    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id)
    {
        using NpgsqlConnection connection = GetConnection();

        using var command = new NpgsqlCommand(
            $"DELETE FROM todoItems WHERE id='{id}';",
            connection
        );

        await command.ExecuteNonQueryAsync();

        connection.Close();

        return Ok();
    }

    [Route("/todo/{id}")]
    [HttpPatch]
    public IActionResult Patch(Guid id, string text)
    {
        using NpgsqlConnection connection = GetConnection();

        using var command = new NpgsqlCommand(
            "UPDATE todoItems SET text=@text WHERE id=@id;",
            connection
        );

        command.Parameters.AddWithValue("id", id);
        command.Parameters.AddWithValue("text", text);

        command.ExecuteNonQuery();
        connection.Close();

        return Json("");
    }

    private static NpgsqlConnection GetConnection()
    {
        // Pull connection string
        string connectionString =
            Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
            ?? throw new Exception("DB_CONNECTION_STRING not set");

        // Create a new NpgsqlConnection object
        NpgsqlConnection connection = new(connectionString);

        // Open the connection
        connection.Open();
        return connection;
    }
}
