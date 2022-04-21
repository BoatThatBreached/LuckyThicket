using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Collection : MonoBehaviour
{
    public GameObject cardInCollectionPref;
    public List<CardCharacter> CardCharacters { get; set; }
    public Transform collectionPanel;

    public void Init()
    {
        CardCharacters = new List<CardCharacter>() { };
        FillCollection();
    }
    
    void Start()
    {
        Init();
        DrawCollection();
    }

    public void DrawCollection()
    {
        foreach (var cardCharacter in CardCharacters)
        {
            var card = Instantiate(cardInCollectionPref, collectionPanel).GetComponent<CardInCollection>();
            card.Chain = cardCharacter.Ability;
            card.Name = cardCharacter.Name;
            card.AbilityMask = cardCharacter.AbilityMask;
            switch (cardCharacter.Rarity)
            {
                case Rarity.Common:
                    card.Color = Color.gray;
                    break;
                case Rarity.Rare:
                    card.Color = Color.blue;
                    break;
                case Rarity.Epic:
                    card.Color = Color.magenta;
                    break;
                case Rarity.Legendary:
                    card.Color = (Color.red + Color.yellow) / 2;
                    break;
            }
        }
    }
    
    public void FillCollection()
    {
        var char0 = new CardCharacter()
        {
            Id = 0,
            Ability = new Queue<Basis>(new[]{Basis.Free, Basis.Select, Basis.Beaver, Basis.Spawn}),
            AbilityMask = "",
            AbilityString = "",
            Name = "Бобрёнок",
            Rarity = Rarity.Common
        };
        var char1 = new CardCharacter()
        {
            Id = 1,
            Ability = new Queue<Basis>(new[]{Basis.Free, Basis.Select, Basis.Beaver, Basis.Spawn,Basis.Adjacent, Basis.Free, Basis.Select, Basis.Beaver, Basis.Spawn}),
            AbilityMask = "Создайте бобра на соседней клетке.",
            AbilityString = "",
            Name = "Бобёр-учёный",
            Rarity = Rarity.Rare
        };
        var char2 = new CardCharacter()
        {
            Id = 2,
            Ability = new Queue<Basis>(new[]{Basis.Free, Basis.Select, Basis.Magpie, Basis.Spawn,
                Basis.Adjacent, Basis.Beaver, Basis.Occupied, Basis.Select, Basis.Kill}),
            AbilityMask = "Уничтожьте бобра на соседней клетке.",
            AbilityString = "",
            Name = "Сорока-ниндзя",
            Rarity = Rarity.Epic
        };
        var char3 = new CardCharacter()
        {
            Id = 3,
            Ability = new Queue<Basis>(new[]{Basis.Free, Basis.Select, Basis.Magpie, Basis.Spawn,
                Basis.Adjacent, Basis.Free, Basis.Select, Basis.Magpie, Basis.Spawn, 
                Basis.Beaver, Basis.Occupied, Basis.Select, Basis.Kill}),
            AbilityMask = "Создайте сороку на соседней клетке и уничтожьте бобра.",
            AbilityString = "",
            Name = "Сорока-гопница",
            Rarity = Rarity.Legendary
        };
        var char4 = new CardCharacter()
        {
            Id = 4,
            Ability = new Queue<Basis>(new[]{Basis.Free, Basis.Select, Basis.Beaver, Basis.Spawn,
                Basis.Draw}),
            AbilityMask = "Возьмите карту.",
            AbilityString = "",
            Name = "Бобёр-гений",
            Rarity = Rarity.Rare
        };
        var chars = new[] {char0, char1, char2, char3, char4};
        for (var i = 0; i < 20; i++)
            CardCharacters.Add(chars.GetRandom());
    }
    
    public void BackToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
