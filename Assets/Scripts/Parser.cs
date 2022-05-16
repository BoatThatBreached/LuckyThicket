using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Unity.Plastic.Newtonsoft.Json;

public class Parser
{
    private Encoding _encode = Encoding.UTF8;
    private const string Path = "Assets\\Resources\\Prefabs\\Cards\\";
    private const string PathToDecks = "Assets\\Resources\\Prefabs\\decks.json";

    public void ResetEncode(Encoding wSet)
    {
        _encode = wSet;
    }

    public async void ConvertToFile(List<CardCharacter> data)
    {
        foreach (var it in data)
        {
            var fStream = new FileStream(Path + it.Id + ".json", FileMode.OpenOrCreate);
            var sb = new StringBuilder();
            foreach (var it1 in it.Ability)
                sb.Append(it1 + " ");
            it.AbilityString = sb.ToString();
            var buffer = _encode.GetBytes(JsonUtility.ToJson(it));
            await fStream.WriteAsync(buffer, 0, buffer.Length);
            fStream.Close();
        }
    }

    private async Task<List<CardCharacter>> ConvertFromFile(List<String> filename)
    {
        var ans = new List<CardCharacter>();
        foreach (var fl in filename)
        {
            var fStream = new FileStream(Path + fl + ".json", FileMode.Open);
            var buffer = new byte[fStream.Length];
            await fStream.ReadAsync(buffer, 0, buffer.Length);
            var jsonString = _encode.GetString(buffer);
            var cardCharacter = JsonUtility.FromJson<CardCharacter>(jsonString);
            var abilityString = cardCharacter.AbilityString;
            var q = new Queue<Basis>();
            foreach (var it in abilityString.Split(' ').ToArray())
            {
                if (it == "")
                    continue;
                q.Enqueue((Basis) Enum.Parse(typeof(Basis), it));
            }

            cardCharacter.Ability = q;
            ans.Add(cardCharacter);
            fStream.Close();
        }

        return ans;
    }

    public Dictionary<string, List<int>> ListDecksFromFile_()
    {
        var ans = new Dictionary<string, List<int>>();
        var fStream = new FileStream(PathToDecks, FileMode.OpenOrCreate);
        var buffer = new byte[fStream.Length];
        fStream.Read(buffer, 0, buffer.Length);
        var jsonString = _encode.GetString(buffer);
        var decks = JsonUtility.FromJson<Dictionary<string, List<int>>>(jsonString);
        //var decks = JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(jsonString);
        fStream.Close();
        return decks ?? new Dictionary<string, List<int>>();
    }
    
    public void SaveDecksToFile_(Dictionary<string, List<int>> decks)
    {
        var json = JsonUtility.ToJson(decks);
        //var jsonString = JsonConvert.SerializeObject(decks);
        File.WriteAllText(PathToDecks, json);
    }


    private List<CardCharacter> ConvertFromFile_(List<string> filename)
    {
        var ans = new List<CardCharacter>();
        foreach (var fl in filename)
        {
            var fStream = new FileStream(Path + fl + ".json", FileMode.Open);
            var buffer = new byte[fStream.Length];
            fStream.Read(buffer, 0, buffer.Length);
            var jsonString = _encode.GetString(buffer);
            var cardCharacter = JsonUtility.FromJson<CardCharacter>(jsonString);
            var abilityString = cardCharacter.AbilityString;
            var q = new Queue<Basis>();
            foreach (var it in abilityString.Split(' ').ToArray())
            {
                if (it == "")
                    continue;
                q.Enqueue((Basis) Enum.Parse(typeof(Basis), it));
            }

            cardCharacter.Ability = q;
            ans.Add(cardCharacter);
            fStream.Close();
        }

        return ans;
    }

    public static async Task<List<CardCharacter>> GetCardsFromFile(IEnumerable<string> filenames)
    {
        var parser = new Parser();
        var cards = await parser.ConvertFromFile(filenames.ToList());
        return cards;
    }

    public static List<CardCharacter> GetCardsFromFile_(IEnumerable<string> filenames)
    {
        var parser = new Parser();
        var cards = parser.ConvertFromFile_(filenames.ToList());
        return cards;
    }

    public static int GetCardsCount()
    {
        var info = new DirectoryInfo(Parser.Path);
        var fileInfos = info.GetFiles();
        var jsons = fileInfos
            .Where(f => f.Name.EndsWith(".json"));
        return jsons.Count();
    }

    public static CardCharacter GetCardFromJson(string json)
    {
        var cardCharacter = JsonUtility.FromJson<CardCharacter>(json);
        var abilityString = cardCharacter.AbilityString;
        var q = new Queue<Basis>();
        //Debug.Log(json);
        
        foreach (var it in abilityString.Split(' ').ToArray())
        {
            if (it == "")
                continue;
            q.Enqueue((Basis) Enum.Parse(typeof(Basis), it));
        }
        cardCharacter.Ability = q;
        return cardCharacter;
    }

    public static string ConvertBoardToJson(Dictionary<Point, Tile> board)
    {
        var lines = (from point in board.Keys 
            let key = $"({point.X}_{point.Y})" 
            let val = board[point].occupantTribe.ToString() 
            select $"\"{key}\":\"{val}\"").ToList();
        return "{"+string.Join(",", lines)+"}";
    }
    public static string ConvertBoardToJson(Dictionary<Point, Tribes> board)
    {
        var lines = (from point in board.Keys 
            let key = $"({point.X}_{point.Y})" 
            let val = board[point].ToString() 
            select $"\"{key}\":\"{val}\"").ToList();
        return "{"+string.Join(",", lines)+"}";
    }

    public static Dictionary<Point, Tribes> EmptyField(int size)
    {
        var points = Enumerable.Range(0, size * size)
            .Select(i => new Point(i / size, i % size));
        var result = new Dictionary<Point, Tribes>();
        foreach (var p in points)
            result[p] = Tribes.None;
        return result;
    }

    public static Dictionary<Point, Tribes> ConvertJsonToBoard(string json)
    {
        try
        {
            var result = new Dictionary<Point, Tribes>();
            var sp = json.Replace("{", "").Replace("}", "").Split(',');
            foreach (var line in sp)
            {
                var unquoted = line.Replace("\"", "").Split(':');
                var coords = unquoted[0].Replace("(", "").Replace(")", "").Split('_');
                var p = new Point(int.Parse(coords[0]), int.Parse(coords[1]));
                var tribe = (Tribes) Enum.Parse(typeof(Tribes), unquoted[1]);
                result[p] = tribe;
            }

            return result;
        }
        catch (Exception e)
        {
            Debug.Log($"An error occured: {e.Message}. Sending empty board!");
            return EmptyField(5);
        }
    }
    
    public static Template GetTemplateFromString(string s)
    {
        // columns from down to top.
        // Beaver Beaver None|None None None|None None Beaver converts to 
        // **B
        // B**
        // B**
        var lines = s.Split('|');
        var array = new List<List<Tribes>>();

        foreach (var line in lines)
        {
            var tribes = line.Split(' ');
            array.Add(new List<Tribes>());
            foreach (var tribe in tribes)
                array.Last().Add((Tribes) Enum.Parse(typeof(Tribes), tribe));
        }

        return new Template(array, array.Count > 3 ? SchemaType.Big : SchemaType.Small, false);
    }
}

public static class StringExtensions
{
    public static bool EndsWith(this string source, string suffix)
    {
        var delta = source.Length - suffix.Length;
        if (delta < 0)
            return false;
        return !suffix
            .Where((t, i) => source[delta + i] != t)
            .Any();
    }

    public static string ToSystemRoom(this string source) => $"S1Y2S3T4E5M{source}R6O7O8M";
    public static string FromSystemRoom(this string source) => source.Substring(11, source.Length - 18);
    public static string ToSystemDeck(this string source) => $"S1Y2S3T4E5M{source}D6E7C8K";
    public static string FromSystemDeck(this string source) => source.Substring(11, source.Length - 18);
    public static string ToJsonList(this IEnumerable<string> source) => $"[{string.Join(",", source)}]";

    public static List<string> FromJsonList(this string source)
    {
        var trim = source.Replace("[", "").Replace("]", "");
        return trim == "" ? new List<string>() : trim.Split(',').ToList();
    }
}