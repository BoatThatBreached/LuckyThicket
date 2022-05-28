using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using System;
using Random = UnityEngine.Random;
// ReSharper disable InconsistentNaming

[Serializable]
public class CardCharacter
{
    private static int _count;

    public static void SetCount(int n) => _count = n;
    
    public CardCharacter(string name, string abilityMask, Queue<Basis> ability, Rarity rarity)
    {
        Id = _count++;
        Name = name;
        Rarity = rarity;
        AbilityMask = abilityMask;
        Ability = ability;
    }
    [SerializeField]public int Id; 
    [SerializeField]public string Name;
    [SerializeField]public Rarity Rarity;
    [SerializeField]public string AbilityMask; 
    public Queue<Basis> Ability;
    [SerializeField]public string AbilityString;
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
    public static T GetRandom<T>(this IEnumerable<T> source)
    {
        var arr = source.ToArray();
        return arr[Random.Range(0,arr.Length)];
    }

    public static List<T> Shuffled<T>(this IEnumerable<T> source)
    {
        var list = source.ToList();
        var res = new List<T>();
        while (list.Count > 0)
        {
            var card = list.GetRandom();
            res.Add(card);
            list.Remove(card);
        }
        return res;
    }
}