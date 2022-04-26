using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using System;
using Mono.Data.Sqlite;
using Random = UnityEngine.Random;

[Serializable]
public class CardCharacter : Model
{
    public CardCharacter(int id, string name, string abilityMask, Queue<Basis> ability, Rarity rarity)
    {
        Id = id;
        Name = name;
        Rarity = rarity;
        AbilityMask = abilityMask;
        Ability = ability;
    }
    [SerializeField]public int Id; 
    [SerializeField]public string Name;
    [SerializeField]public Rarity Rarity;
    [SerializeField]public string AbilityMask; 
    [SerializeField]public Queue<Basis> Ability;
    [SerializeField]public string AbilityString;

    public static void AddNewCard(string name, string abilityMask, Queue<Basis> ability, Rarity rarity)
    {
        Action<SqliteCommand> query = (command) =>
        {
            command.CommandText = @$"
INSERT INTO Cards (Id, Name, Rarity, AbilityMask, AbilityString)
VALUES ({name}, {rarity}, {abilityMask}, {ability})
RETURNING *
";
            command.ExecuteNonQuery();
        };
        _database.WithOpenClose(query);
    }
    
    public static List<CardCharacter> ListCards()
    {
        Func<SqliteCommand, List<CardCharacter>> query = (command) =>
        {
            command.CommandText = "SELECT * FROM Cards;";
            using var rdr = command.ExecuteReader();

            var res = new List<CardCharacter>();
            while (rdr.Read())
            {
                var cc = new CardCharacter(
                    id: rdr.GetInt32(0),
                    name: rdr.GetString(1),
                    abilityMask: rdr.GetString(3),
                    ability: Serializer.AbilityFromString(rdr.GetString(4)),
                    rarity: Serializer.RarityFromString(rdr.GetString(2))
                );
                res.Add(cc);
            }

            return res;
        };
        return _database.WithOpenClose(query);
    }
    
    public static void FillTestData()
    {
        if (ListCards().Count != 0)
            return;
        Action<SqliteCommand> query = (command) =>
        {
            Debug.Log("Add test data");
            command.CommandText = @"
INSERT INTO Cards (Id, Name, Rarity, AbilityMask, AbilityString)
VALUES
(0, 'Бобрёнок', '0', '', 'Free Select Beaver Spawn '),
(10, 'Сорочонок', '0', '', 'Free Select Magpie Spawn '),
(1, 'Бобёр-учитель', '0', 'Возьмите карту.','Free Select Beaver Spawn Draw '),
(2, 'Бобёр-шахтёр', '0', 'Постройте клетку и поставьте на неё бобра-шахтёра.','NExisting Select Build Beaver Spawn '),
(3, 'Бобёр-строитель', '0', 'Постройте клетку.','Free Select Beaver Spawn NExisting Select Build '),
(4, 'Бобёр-воин', '1', 'Уничтожьте сороку на соседней клетке.','Free Select Beaver Spawn Adjacent Magpie Occupied Select Kill '),
(5, 'Бобёр-волшебник', '1', 'Создайте бобра на случайной клетке.','Free Select Beaver Spawn Free Random Beaver Spawn '),
(6, 'Бобёр-добряк', '1', 'Создайте бобра на соседней клетке.','Free Select Beaver Spawn Free Adjacent Select Beaver Spawn '),
(7, 'Бобёр-камикадзе', '2', 'Уничтожьте случайного бобра и двух сорок.','Free Select Beaver Spawn Beaver Occupied Random Kill Magpie Occupied Select Kill Magpie Occupied Select Kill '),
(8, 'Бобёр-бродяга', '2', 'Создайте бобра на краю поля и уничтожьте сороку на краю поля.','Free Select Beaver Spawn Free Edge Select Beaver Spawn Edge Magpie Occupied Select Kill '),
(9, 'Бобёр-король', '3', 'Создайте три клетки и поставьте на них бобров.','Free Select Beaver Spawn NExisting Select Build Beaver Spawn NExisting Select Build Beaver Spawn NExisting Select Build Beaver Spawn '),
(11, 'Сорока-диверсант', '0', 'Уничтожьте свободную клетку.','Free Select Magpie Spawn Free Select Destroy '),
(12, 'Сорока-садовник', '0', 'Постройте две клетки.','Free Select Magpie Spawn NExisting Select Build NExisting Select Build '),
(13, 'Сорока-художница', '0', 'Возьмите карту.','Free Select Magpie Spawn Draw '),
(14, 'Сорока-ниндзя', '2', 'Уничтожьте двух случайных бобров','Free Select Magpie Spawn Beaver Occupied Random Kill Beaver Occupied Random Kill '),
(15, 'Сорока-вестница', '2', 'Возьмите четыре карты.','Free Select Magpie Spawn Draw Draw Draw Draw '),
(16, 'Сорока-иллюзионист', '1', 'Создайте сороку на случайной клетке вокруг.','Free Select Magpie Spawn Surrounding Free Random Magpie Spawn '),
(17, 'Сорока-мастерица', '1', 'Постройте клетку в случайном месте и уничтожьте бобра по соседству.','Free Select Magpie Spawn NExisting Random Build Adjacent Beaver Occupied Select Kill '),
(18, 'Сорока-белобока', '1', 'Возьмите карту и уничтожьте соседнюю свободную клетку.','Free Select Magpie Spawn Draw Free Adjacent Select Destroy '),
(19, 'Сорока-мать', '3', 'Создайте трёх сорок на краю поля.','Free Select Magpie Spawn Free Edge Select Magpie Spawn Free Edge Select Magpie Spawn Free Edge Select Magpie Spawn ');
";
            command.ExecuteNonQuery();
        };
        _database.WithOpenClose(query);
    }

}

public enum Rarity
{
    Common, 
    Rare,
    Epic,
    Legendary
}

public static class CardExtensions
{
    public static CardCharacter GetRandom(this IEnumerable<CardCharacter> source)
    {
        var arr = source.ToArray();
        return arr[Random.Range(0,arr.Length)];
    }
}