using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        return new Template(array, array.Count > 3 ? SchemaType.Big : SchemaType.Small);
    }

    public static Queue<Point> ParseSelections(string selections)
    {
        var queue = new Queue<Point>();
        foreach (var s in selections.Split(','))
            queue.Enqueue(s.ToPoint());
        return queue;
    }

    public static string ConvertSelections(Queue<Point> selections)
        => string.Join(",", selections.Select(p => p.ToCompactString()));

    public static Dictionary<Point, Tribes> EmptyBoard(int size, Point center, bool holes = false)
    {
        var board = new Dictionary<Point, Tribes>();
        for (var i = -size / 2; i <= size / 2; i++)
        for (var j = -size / 2; j <= size / 2; j++)
            board[center.Add(new Point(i, j))] = Tribes.None;
        board.Remove(center.Add(new Point(0, -size / 2)));
        board.Remove(center.Add(new Point(0, size / 2)));
        board.Remove(center.Add(new Point(size / 2 - 1, 1 - size / 2)));
        board.Remove(center.Add(new Point(1 - size / 2, size / 2 - 1)));
        return board;
    }
}

public static class StringExtensions
{
    public static string ToSystemRoom(this string source) => $"S1Y2S3T4E5M{source}R6O7O8M";
    public static string FromSystemRoom(this string source) => source.Substring(11, source.Length - 18);

    public static string ToJsonList<T>(this IEnumerable<T> source) =>
        $"[{string.Join(",", source.Select(t => t.ToString()))}]";

    public static List<string> FromJsonList(this string source)
    {
        var trim = source.Replace("[", "").Replace("]", "");
        return trim == "" ? new List<string>() : trim.Split(',').ToList();
    }

    public static List<T> JsonsFromJsonList<T>(this string source)
    {
        return source.GetJsons().Select(JsonUtility.FromJson<T>).ToList();
    }

    public static List<string> GetJsons(this string source)
    {
        var trim = source.Substring(1).Substring(0, source.Length - 2);
        var currJson = new StringBuilder();
        var res = new List<string>();
        var figBracketsCount = 0;
        foreach (var ch in trim.Where(ch => figBracketsCount != 0 || "{}".Contains(ch)))
        {
            currJson.Append(ch);
            switch (ch)
            {
                case '{':
                    figBracketsCount++;
                    break;
                case '}':
                    figBracketsCount--;
                    break;
            }

            if (figBracketsCount != 0) continue;
            res.Add(currJson.ToString());
            currJson.Clear();
        }

        return res;
    }

    public static Point ToPoint(this string source) =>
        new Point(int.Parse(source.Split('_')[0]), int.Parse(source.Split('_')[1]));

    public static string ToCompactString(this Point source) => $"{source.X}_{source.Y}";
}