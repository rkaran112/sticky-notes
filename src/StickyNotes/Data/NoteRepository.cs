using Microsoft.Data.Sqlite;

namespace StickyNotes.Data;

public class NoteRepository
{
    public List<Note> GetAll()
    {
        var notes = new List<Note>();

        using var connection = new SqliteConnection(Database.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Content, Color, Left, Top, Width, Height, CreatedAt FROM Notes ORDER BY CreatedAt ASC;";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            notes.Add(new Note
            {
                Id = reader.GetString(0),
                Content = reader.GetString(1),
                Color = reader.GetString(2),
                Left = reader.GetDouble(3),
                Top = reader.GetDouble(4),
                Width = reader.GetDouble(5),
                Height = reader.GetDouble(6),
                CreatedAt = DateTime.Parse(reader.GetString(7)),
            });
        }

        return notes;
    }

    public void Insert(Note note)
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Notes (Id, Content, Color, Left, Top, Width, Height, CreatedAt)
            VALUES ($id, $content, $color, $left, $top, $width, $height, $createdAt);
            """;
        command.Parameters.AddWithValue("$id", note.Id);
        command.Parameters.AddWithValue("$content", note.Content);
        command.Parameters.AddWithValue("$color", note.Color);
        command.Parameters.AddWithValue("$left", note.Left);
        command.Parameters.AddWithValue("$top", note.Top);
        command.Parameters.AddWithValue("$width", note.Width);
        command.Parameters.AddWithValue("$height", note.Height);
        command.Parameters.AddWithValue("$createdAt", note.CreatedAt.ToString("o"));
        command.ExecuteNonQuery();
    }

    public void UpdateContent(string id, string content)
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE Notes SET Content = $content WHERE Id = $id;";
        command.Parameters.AddWithValue("$content", content);
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }

    public void UpdateColor(string id, string color)
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE Notes SET Color = $color WHERE Id = $id;";
        command.Parameters.AddWithValue("$color", color);
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }

    public void UpdateBounds(string id, double left, double top, double width, double height)
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE Notes SET Left = $left, Top = $top, Width = $width, Height = $height WHERE Id = $id;
            """;
        command.Parameters.AddWithValue("$left", left);
        command.Parameters.AddWithValue("$top", top);
        command.Parameters.AddWithValue("$width", width);
        command.Parameters.AddWithValue("$height", height);
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }

    public void Delete(string id)
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Notes WHERE Id = $id;";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }
}
