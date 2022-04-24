using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using System;
using Random = UnityEngine.Random;

[Serializable]
public class CardCharacter
{
    private static int _count;

    public static void Flush() => _count = 0;
    
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
    [SerializeField]public Queue<Basis> Ability;
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
    public static CardCharacter GetRandom(this IEnumerable<CardCharacter> source)
    {
        var arr = source.ToArray();
        return arr[Random.Range(0,arr.Length)];
    }
}