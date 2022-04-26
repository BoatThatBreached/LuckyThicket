using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mono.Data.Sqlite;
using UnityEngine;

public class DB
{
    private string DBName { get; set; }
    private SqliteConnection Connection { get; }

    public DB(string dbName)
    {
        this.DBName = dbName;
        Connection = new SqliteConnection(dbName);
        CreateDB();
    }


    public T WithOpenClose<T>(Func<SqliteCommand, T> query)
    {
        T res;
        Connection.Open();
        using (var command = Connection.CreateCommand())
        {
            res = query(command);
        }

        Connection.Close();
        return res;
    }

    public void WithOpenClose(Action<SqliteCommand> query)
    {
        Connection.Open();
        using (var command = Connection.CreateCommand())
        {
            query(command);
        }

        Connection.Close();
    }

    #region Init DB

    private void CreateDB()
    {
        Action<SqliteCommand> query = (command) =>
        {
            command.CommandText = @"
CREATE TABLE IF NOT EXISTS Cards (
Id INTEGER PRIMARY KEY AUTOINCREMENT,
Name VARCHAR(80),
Rarity VARCHAR(10),
AbilityMask VARCHAR(500),
AbilityString VARCHAR(500)
);

CREATE TABLE IF NOT EXISTS Decks (
Id INTEGER PRIMARY KEY AUTOINCREMENT,
Name VARCHAR(80)
);

CREATE TABLE IF NOT EXISTS Content (
ContentId INTEGER PRIMARY KEY AUTOINCREMENT,
CardId INTEGER,
DeckId INTEGER 
);
";
            command.ExecuteNonQuery();
        };
        WithOpenClose(query);
    }
    #endregion
}

public class Model
{
    protected static DB _database = new DB(@$"URI=file:{Application.dataPath}/main.db");

    public Model()
    {
    }
}