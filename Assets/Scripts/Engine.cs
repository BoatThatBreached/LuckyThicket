using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using Color = UnityEngine.Color;
using Random = System.Random;

public class Engine : MonoBehaviour
{
    private Queue<Basis> CurrentChain { get; set; }
    private Dictionary<Basis, Action> Actions { get; set; }
    private Point Anchor { get; set; }
    // private Point AnchorF { get; set; }
    // private Point AnchorS { get; set; }
    // private Point AnchorT { get; set; }
    private Tribes AnchorTribe { get; set; }
    // private Tribes AnchorTribeF { get; set; }
    // private Tribes AnchorTribeS { get; set; }
    // private Tribes AnchorTribeT { get; set; }
    public Basis CurrentAction { get; private set; }
    private List<Func<Point, bool>> Criterias { get; set; }
    private bool NotExistingNeeded { get; set; }
    private Random Rand { get; set; }
    public Game game;
    private Dictionary<Point, Tile> Board => game.Board;
    private Queue<Point> LoadedSelections { get; set; }
    private Queue<Point> SelfSelections { get; set; }

    void Start()
    {
        CurrentChain = new Queue<Basis>();
        LoadedSelections = new Queue<Point>();
        SelfSelections = new Queue<Point>();
        Anchor = Point.Empty;
        AnchorTribe = Tribes.None;
        NotExistingNeeded = false;
        Criterias = new List<Func<Point, bool>>();
        Rand = new Random();
        InitActions();
    }

    private void InitActions()
    {
        Actions = new Dictionary<Basis, Action>();

        Actions[Basis.Build] = () => AddTile(Anchor);
        Actions[Basis.Destroy] = () => DestroyTile(Anchor);
        Actions[Basis.Kill] = () => KillUnit(Anchor);

        Actions[Basis.Spawn] = () => SpawnUnit(Anchor, AnchorTribe);
        //Actions[Basis.PushUnit] = ()=>PushUnit(Anchor, AnchorTribe);
        //Actions[Basis.PullUnit] = ()=>PullUnit(Anchor, AnchorTribe);
        Actions[Basis.Beaver] = () => AnchorTribe = Tribes.Beaver;
        Actions[Basis.Magpie] = () => AnchorTribe = Tribes.Magpie;

        Actions[Basis.Free] = () => Criterias.Add(IsFree);
        Actions[Basis.Adjacent] = () => Criterias.Add(IsAdjacentToAnchor);
        Actions[Basis.Surrounding] = () => Criterias.Add(IsSurroundingToAnchor);
        Actions[Basis.Occupied] = () => Criterias.Add(IsOccupiedByAnchorTribe);
        Actions[Basis.Existing] = () => Criterias.Add(Exists);
        Actions[Basis.NExisting] = () =>
        {
            Criterias.Add(p => !Exists(p));
            NotExistingNeeded = true;
        };
        Actions[Basis.Edge] = () => Criterias.Add(IsEdge);
        Actions[Basis.Select] = () =>
        {
            if (!ShowPossibleTiles())
                SkipToAlso();
            if (LoadedSelections.Count > 0)
            {
                SelectPoint(LoadedSelections.Dequeue());
            }
        };
        Actions[Basis.Also] = () => { };
        Actions[Basis.Random] = () =>
        {
            if(LoadedSelections.Count>0)
                SelectPoint(LoadedSelections.Dequeue());
            else
                TrySelectRandomPoint();
        };

        Actions[Basis.Draw] = () => game.player.DrawCard();
    }

    public bool Exists(Point p) => Board.ContainsKey(p);
    public Tribes GetOccupantTribe(Point p) => Board[p].occupantTribe;
    public bool HasAdjacent(Point p) => p.GetAdjacent().Any(Exists);
    public bool HasSurrounding(Point p) => p.GetSurrounding().Any(Exists);

    #region Criterias

    public bool IsOccupied(Point p) => Exists(p) && Board[p].occupantTribe != Tribes.None;
    private bool IsFree(Point p) => Exists(p) && Board[p].occupantTribe == Tribes.None;
    private bool IsAdjacentToAnchor(Point p) => Anchor.GetAdjacent().Contains(p);
    private bool IsSurroundingToAnchor(Point p) => Anchor.GetSurrounding().Contains(p);
    private bool IsEdge(Point p) => p.GetAdjacent().Count(Exists) < 4;
    private bool IsOccupiedByAnchorTribe(Point p) => GetOccupantTribe(p) == AnchorTribe;

    #endregion

    #region Operations

    public void AddTile(Point p)
    {
        var tile = Instantiate(game.tilePref, transform);
        tile.transform.position = new Vector3(p.X, p.Y, 0);
        var t = tile.GetComponent<Tile>();
        Board[p] = t;
    }

    public void DestroyTile(Point p)
    {
        var tile = Board[p].gameObject;
        Destroy(tile);
        Board.Remove(p);
    }

    public void SpawnUnit(Point p, Tribes t)
    {
        Board[p].occupantTribe = t;
        var occupant = Instantiate(game.designer.occupantPref, Board[p].transform);
        occupant.GetComponent<SpriteRenderer>().color = game.designer.Colors[t];
        occupant.transform.localScale = new Vector3(3, 3, 1);
        occupant.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = game.designer.Sprites[t];
        FlushTribe();
    }

    public void KillUnit(Point p)
    {
        Board[p].occupantTribe = Tribes.None;
        Destroy(Board[p].transform.GetChild(0).gameObject);
        FlushTribe();
    }

    #endregion

    private void FlushTribe() => AnchorTribe = Tribes.None;

    public void TryLoadActions(Queue<Basis> chain, CardCharacter card, GameObject go)
    {
        if (game.currentCardCharacter != null)
            return;
        game.currentCardCharacter = card;
        game.currentCard = go;
        Criterias.Clear();
        FlushTribe();
        CurrentChain.Clear();
        LoadedSelections.Clear();
        SelfSelections.Clear();
        foreach (var act in chain)
            CurrentChain.Enqueue(act);
        Step();
    }

    public void LoadActionsWithSelections_Unsafe(Queue<Basis> chain, Queue<Point> selections)
    {
        Criterias.Clear();
        FlushTribe();
        CurrentChain.Clear();
        foreach (var act in chain)
            CurrentChain.Enqueue(act);
        LoadedSelections = selections;
        Step();
    }

    public bool SatisfiesCriterias(Point p)
    {
        var pred = Criterias.All(crit => crit(p));
        if (NotExistingNeeded)
            pred = pred && Board.Keys
                .Where(IsEdge)
                .SelectMany(point => point.GetAdjacent())
                .Contains(p);
        return pred;
    }

    private void Step()
    {
        CurrentAction = CurrentChain.Count > 0 ? CurrentChain.Dequeue() : Basis.Idle;
        //print(CurrentAction);
        if (CurrentAction == Basis.Idle)
        {
            Anchor = Point.Empty;
            AnchorTribe = Tribes.None;
            CheckWin();
            game.EndTurn();
            return;
        }

        Actions[CurrentAction]();
        if (CurrentAction != Basis.Select && CurrentAction != Basis.Idle)
            Step();
    }

    private void CheckWin()
    {
        var completedPlayer = game.player.GetTemplatesPlayerCanComplete(Board);
        if (completedPlayer.Count > 0)
        {
            print($"{game.player.Name} can complete smth and count is {completedPlayer.Count}");
            // delete first completed
            var template = completedPlayer[0];
            foreach (var p in template.Template.Points.Keys)
            {
                var currp = new Point(p.X + template.StartingPoint.X, p.Y + template.StartingPoint.Y);
                KillUnit(currp);
            }
            game.player.CompleteTemplate(template.Template);
        }

        if (game.player.CompletedCount()[SchemaType.Big] == 1 || game.player.CompletedCount()[SchemaType.Small] == 2)
            game.Win(true);
        
        var completedOpponent = game.opponent.GetTemplatesPlayerCanComplete(Board);
        if (completedOpponent.Count > 0)
        {
            print($"{game.opponent.Name} can complete smth and count is {completedOpponent.Count}");
            // delete first completed
            var template = completedOpponent[0];
            foreach (var p in template.Template.Points.Keys)
            {
                var currp = new Point(p.X + template.StartingPoint.X, p.Y + template.StartingPoint.Y);
                KillUnit(currp);
            }
            game.opponent.CompleteTemplate(template.Template);
        }

        if (game.opponent.CompletedCount()[SchemaType.Big] == 1 || game.opponent.CompletedCount()[SchemaType.Small] == 2)
            game.Lose(true);
        
    }

    private void SkipToAlso()
    {
        Criterias.Clear();
        while (CurrentChain.Count > 0 && CurrentChain.Peek() != Basis.Also)
            CurrentChain.Dequeue();
        Step();
    }

    public void SelectPoint(Point p)
    {
        Anchor = p;
        foreach (var t in Board.Values)
            t.Color = Color.white;
        SelfSelections.Enqueue(p);
        Criterias.Clear();
        NotExistingNeeded = false;
        Step();
    }

    private bool ShowPossibleTiles()
    {
        if (NotExistingNeeded)
        {
            var pts = Board.Keys
                .Where(IsEdge)
                .SelectMany(p => p.GetAdjacent())
                .Where(p => !Exists(p));
            var neres = false;
            foreach (var p in pts)
                if (SatisfiesCriterias(p))
                    neres = true;
            return neres;
        }

        var res = false;
        foreach (var p in Board.Keys)
            if (SatisfiesCriterias(p))
            {
                Board[p].Color = Color.yellow;
                res = true;
            }

        return res;
    }

    private void TrySelectRandomPoint()
    {
        Point[] possible;
        if (NotExistingNeeded)
        {
            possible = Board
                .Keys
                .Where(IsEdge)
                .SelectMany(p => p.GetAdjacent())
                .Where(SatisfiesCriterias)
                .ToArray();
        }
        else
        {
            possible = Board
                .Keys
                .Where(SatisfiesCriterias)
                .ToArray();
        }

        if (!possible.Any())
        {
            SkipToAlso();
            return;
        }

        SelectPoint(possible[Rand.Next(possible.Length)]);
    }
}

static class EngineExtensions
{
    public static IEnumerable<Point> GetAdjacent(this Point p)
    {
        return Enumerable.Range(-1, 3)
            .SelectMany(x => Enumerable.Range(-1, 3).Select(y => (x, y)))
            .Where(tuple => tuple.x == 0 ^ tuple.y == 0)
            .Select(tuple => new Point(p.X + tuple.x, p.Y + tuple.y));
    }

    public static IEnumerable<Point> GetSurrounding(this Point p)
    {
        return Enumerable.Range(-1, 3)
            .SelectMany(x => Enumerable.Range(-1, 3).Select(y => (x, y)))
            .Where(tuple => !(tuple.x == 0 && tuple.y == 0))
            .Select(tuple => new Point(p.X + tuple.x, p.Y + tuple.y));
    }
}