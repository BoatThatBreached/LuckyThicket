
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardCharacter
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Rarity Rarity { get; set; }
    public string AbilityMask { get; set; }
    public Queue<Basis> Ability { get; set; }
    public string AbilityString { get; set; }
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