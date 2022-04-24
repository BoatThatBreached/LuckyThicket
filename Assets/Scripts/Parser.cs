using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Parser
{
    private Encoding Encode = Encoding.UTF8;

    public void ResetEncode(Encoding WSet)
    {
        Encode = WSet;
    }

    public async void ConvertToFile(List<CardCharacter> data)
    {
        foreach (var it in data)
        {
            var fStream = new FileStream("Assets\\Resources\\Prefabs\\Cards\\" + it.Id + ".json", FileMode.OpenOrCreate);
            var sb = new StringBuilder();
            foreach (var it1 in it.Ability)
                sb.Append(it1 + " ");
            it.AbilityString = sb.ToString();
            var buffer = Encode.GetBytes(JsonUtility.ToJson(it));
            await fStream.WriteAsync(buffer, 0, buffer.Length);
            fStream.Close();
        }
    }

    public async Task<List<CardCharacter>> ConvertFromFile(List<String> filename)
    {
        var ans = new List<CardCharacter>();
        foreach (var fl in filename)
        {
            FileStream fStream = new FileStream("Assets\\Resources\\Prefabs\\Cards\\" +  fl + ".json", FileMode.Open);
            byte[] buffer = new byte[fStream.Length];
            await fStream.ReadAsync(buffer, 0, buffer.Length);
            string jsonString = Encode.GetString(buffer);
            var cardСharacter = JsonUtility.FromJson<CardCharacter>(jsonString);
            var abilityString = cardСharacter.AbilityString;
            Queue<Basis> q = new Queue<Basis>();
            foreach (var it in abilityString.Split(' ').ToArray())
            {
                if (it == "")
                    continue;
                q.Enqueue((Basis)Enum.Parse(typeof(Basis), it));
            }
            cardСharacter.Ability = q;
            ans.Add(cardСharacter);
            fStream.Close();
        }
        return ans;
    }
}