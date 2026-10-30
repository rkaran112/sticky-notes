using System.IO;
using Microsoft.Data.Sqlite;

namespace StickyNotes.Data;

public static class Database
{
    public static string DbPath { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "StickyNotesApp",
        "notes.db");

    public static string ConnectionString => $"Data Source={DbPath}";

    public static void Initialize()
    {
        var dir = Path.GetDirectoryName(DbPath)!;
        Directory.CreateDirectory(dir);

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS Notes (
                Id TEXT PRIMARY KEY,
                Content TEXT NOT NULL DEFAULT '',
                Color TEXT NOT NULL DEFAULT 'Yellow',
                Left REAL NOT NULL DEFAULT 100,
                Top REAL NOT NULL DEFAULT 100,
                Width REAL NOT NULL DEFAULT 260,
                Height REAL NOT NULL DEFAULT 260,
                CreatedAt TEXT NOT NULL
            );
            """;
        command.ExecuteNonQuery();
    }
}
