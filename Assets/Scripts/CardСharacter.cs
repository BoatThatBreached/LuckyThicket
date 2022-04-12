using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class CardСharacter
{
    public int Id { get; set; }
    public String Name { get; set; }
    public Rarity Rarity { get; set; }
    public string AbilityMask { get; set; }
    public Queue<Basis> Ability { get; set; }
    public String AbilityString { get; set; }
}