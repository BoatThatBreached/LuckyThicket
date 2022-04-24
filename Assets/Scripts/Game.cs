using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject tilePref;

    public Dictionary<Point, Tile> Board { get; set; }
    public OccupantDesigner designer;
    private int Size { get; set; }
    public Engine gameEngine;
    private Queue<Player> turnsQueue;
    public Player currentPlayer;
    public Card currentCard;
    public GameObject cardPref;

    public Player[] Players { get; set; }

    void Start()
    {
        InitPlayers();
        InitBoard();
        InitDecks();
        turnsQueue = new Queue<Player>(Players);
        StartTurn();
    }

    private void InitDecks()
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
        foreach (var p in Players)
        {
            for (var i = 0; i < 20; i++)
                p.Deck.Push(chars.GetRandom());
            for (var i = 0; i < 5; i++)
                p.DrawCard();
        }
    }

    private void StartTurn()
    {
        currentPlayer = turnsQueue.Dequeue();
        currentPlayer.gameObject.SetActive(true);
    }

    public void EndTurn()
    {
        currentPlayer.gameObject.SetActive(false);
        turnsQueue.Enqueue(currentPlayer);
        currentPlayer.Hand.Remove(currentCard);
        currentPlayer.Discard.Add(currentCard);
        Destroy(currentCard.gameObject);
        StartTurn();
    }

    private void InitBoard()
    {
        Size = 3;
        Board = new Dictionary<Point, Tile>();
        for (int i = -Size / 2; i < Size / 2 + 1; i++)
        for (int j = -Size / 2; j < Size / 2 + 1; j++)
            gameEngine.AddTile(new Point(i, j));
    }

    private void InitPlayers()
    {
        Players = FindObjectsOfType<Player>(true);

        Players[0].Name = "Maximus";
        Players[1].Name = "Michael";
        foreach (var p in Players)
        {
            p.Init();
            p.gameObject.SetActive(false);
            p.game = this;
        }


        Template bebrus = new Template(new[,] {{Tribes.Beaver, Tribes.Beaver, Tribes.Beaver}}, SchemaType.Big, false);
        Players[0].AddWinTemplate(bebrus);
        Template magpuk = new Template(new[,] {{Tribes.Magpie}, {Tribes.None}, {Tribes.Magpie}}, SchemaType.Big, false);
        Players[1].AddWinTemplate(magpuk);
    }
}