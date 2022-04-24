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
        var char0 = new CardCharacter(
            "Бобрёнок",
            "",
            new Queue<Basis>(new[] {Basis.Free, Basis.Select, Basis.Beaver, Basis.Spawn}),
            Rarity.Common);
        var char1 = new CardCharacter(
            "Бобёр-учёный",
            "Создайте бобра на соседней клетке.",
            new Queue<Basis>(new[]
            {
                Basis.Free, Basis.Select, Basis.Beaver, Basis.Spawn, Basis.Adjacent, Basis.Free, Basis.Select,
                Basis.Beaver, Basis.Spawn
            }),
            Rarity.Rare
        );
        var char2 = new CardCharacter(
            "Сорока-ниндзя",
            "Уничтожьте бобра на соседней клетке.",
            new Queue<Basis>(new[]
            {
                Basis.Free, Basis.Select, Basis.Magpie, Basis.Spawn,
                Basis.Adjacent, Basis.Beaver, Basis.Occupied, Basis.Select, Basis.Kill
            }),
            Rarity.Epic);

        var char3 = new CardCharacter(
            "Сорока-гопница",
            "Создайте сороку на соседней клетке и уничтожьте бобра.",
            new Queue<Basis>(new[]
            {
                Basis.Free, Basis.Select, Basis.Magpie, Basis.Spawn,
                Basis.Adjacent, Basis.Free, Basis.Select, Basis.Magpie, Basis.Spawn,
                Basis.Beaver, Basis.Occupied, Basis.Select, Basis.Kill
            }),
            Rarity.Legendary);
        var char4 = new CardCharacter(
            "Бобёр-гений",
            "Возьмите карту.",
            new Queue<Basis>(new[]
            {
                Basis.Free, Basis.Select, Basis.Beaver, Basis.Spawn,
                Basis.Draw
            }),
            Rarity.Rare
        );
        var chars = new[] {char0, char1, char2, char3, char4};
        for (var i = 0; i < 20; i++)
            CardCharacters.Add(chars.GetRandom());
    }
    
    public void BackToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
