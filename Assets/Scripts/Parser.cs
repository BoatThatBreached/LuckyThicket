using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Parser
{
    private Encoding _encode = Encoding.UTF8;
    public static readonly string Path = "Assets\\Resources\\Prefabs\\Cards\\";

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
            var fStream = new FileStream(Path +  fl + ".json", FileMode.Open);
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
                q.Enqueue((Basis)Enum.Parse(typeof(Basis), it));
            }
            cardCharacter.Ability = q;
            ans.Add(cardCharacter);
            fStream.Close();
        }
        return ans;
    }
    
    private List<CardCharacter> ConvertFromFile_(List<String> filename)
    {
        var ans = new List<CardCharacter>();
        foreach (var fl in filename)
        {
            var fStream = new FileStream(Path +  fl + ".json", FileMode.Open);
            var buffer = new byte[fStream.Length];
            fStream.ReadAsync(buffer, 0, buffer.Length);
            var jsonString = _encode.GetString(buffer);
            var cardCharacter = JsonUtility.FromJson<CardCharacter>(jsonString);
            var abilityString = cardCharacter.AbilityString;
            var q = new Queue<Basis>();
            foreach (var it in abilityString.Split(' ').ToArray())
            {
                if (it == "")
                    continue;
                q.Enqueue((Basis)Enum.Parse(typeof(Basis), it));
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
}