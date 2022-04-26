using System;
using System.Collections.Generic;
using System.Linq;

public class Serializer
{
    public static string AbilityToString(Queue<Basis> ability)
    {
        return string.Join(" ", ability);
    }

    public static Queue<Basis> AbilityFromString(string str)
    {
        var res = new Queue<Basis>();
        foreach (var it in str.Split(' ').ToArray())
        {
            if (it == "")
                continue;
            res.Enqueue((Basis) Enum.Parse(typeof(Basis), it));
        }

        return res;
    }

    public static string RarityToString(Rarity rarity)
    {
        return rarity.ToString();
    }

    public static Rarity RarityFromString(string str)
    {
        return (Rarity) Enum.Parse(typeof(Rarity), str);
    }
}