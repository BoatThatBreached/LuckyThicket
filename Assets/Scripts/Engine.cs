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

    private Point AnchorZ { get; set; }
    private Point AnchorF { get; set; }

    // private Point AnchorS { get; set; }
    // private Point AnchorT { get; set; }
    private Tribes AnchorTribeZ { get; set; }
    private Tribes AnchorTribeF { get; set; }

    // private Tribes AnchorTribeS { get; set; }
    // private Tribes AnchorTribeT { get; set; }
    public Basis CurrentAction { get; private set; }
    private List<Func<Point, bool>> Criterias { get; set; }
    private bool NotExistingNeeded { get; set; }
    private Random random;
    public Game game;
    private Dictionary<Point, Tile> Board => game.Board;
    private Queue<Point> LoadedSelections { get; set; }
    private Queue<Point> SelfSelections { get; set; }

    private void Start()
    {
        CurrentChain = new Queue<Basis>();
        LoadedSelections = new Queue<Point>();
        SelfSelections = new Queue<Point>();
        FlushAnchor();
        FlushTribe();
        NotExistingNeeded = false;
        Criterias = new List<Func<Point, bool>>();
        random = new Random();
        InitActions();
    }

    private void InitActions()
    {
        Actions = new Dictionary<Basis, Action>
        {
            [Basis.Build] = () => AddTile(AnchorZ),
            [Basis.Destroy] = () => DestroyTile(AnchorZ),
            [Basis.Kill] = () => Kill(AnchorZ),
            [Basis.Spawn] = () => Spawn(AnchorZ, AnchorTribeZ),
            [Basis.Push] = () => Push(AnchorF, AnchorZ),
            [Basis.Pull] = () => Pull(AnchorF, AnchorZ),
            [Basis.Convert] = () => Convert(AnchorZ, AnchorTribeZ),
            [Basis.Drag] = () => Drag(AnchorF, AnchorZ),
            [Basis.Beaver] = () => AnchorTribeZ = Tribes.Beaver,
            [Basis.Magpie] = () => AnchorTribeZ = Tribes.Magpie,
            [Basis.ShiftAnchor] = () => { AnchorF = AnchorZ; },
            [Basis.ShiftTribe] = () => { AnchorTribeF = AnchorTribeZ; },
            [Basis.Free] = () => Criterias.Add(IsFree),
            [Basis.Adjacent] = () => Criterias.Add(IsAdjacentToAnchor),
            [Basis.Surrounding] = () => Criterias.Add(IsSurroundingToAnchor),
            [Basis.Occupied] = () => Criterias.Add(IsOccupiedByAnchorTribe),
            [Basis.Existing] = () => Criterias.Add(Exists),
            [Basis.NExisting] = () =>
            {
                Criterias.Add(p => !Exists(p));
                NotExistingNeeded = true;
            },
            [Basis.Edge] = () => Criterias.Add(IsEdge),
            [Basis.Column] = () => Criterias.Add(IsOnAnchorColumn),
            [Basis.Row] = () => Criterias.Add(IsOnAnchorRow),
            [Basis.CrossPlus] = () => Criterias.Add(IsOnAnchorCrossPlus),
            [Basis.CrossX] = () => Criterias.Add(IsOnAnchorCrossX),
            [Basis.Select] = () =>
            {
                if (!ShowPossibleTiles())
                    SkipToAlso();
                if (LoadedSelections.Count > 0)
                {
                    SelectPoint(LoadedSelections.Dequeue());
                }
            },
            [Basis.Also] = () => { },
            [Basis.Random] = () =>
            {
                if (LoadedSelections.Count > 0)
                    SelectPoint(LoadedSelections.Dequeue());
                else
                    TrySelectRandomPoint();
            },
            [Basis.Draw] = () => game.player.DrawCard()
        };
    }

    public bool Exists(Point p) => Board.ContainsKey(p);
    public Tribes GetOccupantTribe(Point p) => Board[p].occupantTribe;
    public bool HasAdjacent(Point p) => p.GetAdjacent().Any(Exists);
    public bool HasSurrounding(Point p) => p.GetSurrounding().Any(Exists);

    #region Criterias

    public bool IsOccupied(Point p) => Exists(p) && Board[p].occupantTribe != Tribes.None;
    private bool IsFree(Point p) => Exists(p) && Board[p].occupantTribe == Tribes.None;
    private bool IsAdjacentToAnchor(Point p) => AnchorZ.GetAdjacent().Contains(p);
    private bool IsSurroundingToAnchor(Point p) => AnchorZ.GetSurrounding().Contains(p);
    private bool IsEdge(Point p) => p.GetAdjacent().Count(Exists) < 4;
    private bool IsOccupiedByAnchorTribe(Point p) => GetOccupantTribe(p) == AnchorTribeZ;
    private bool IsOnAnchorRow(Point p) => AnchorZ.GetRow(Board.Keys).Contains(p);
    private bool IsOnAnchorColumn(Point p) => AnchorZ.GetColumn(Board.Keys).Contains(p);
    private bool IsOnAnchorCrossPlus(Point p) => IsOnAnchorColumn(p) || IsOnAnchorRow(p);
    private bool IsOnAnchorCrossX(Point p) => AnchorZ.GetCross(Board.Keys).Contains(p);

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

    public void Spawn(Point p, Tribes t)
    {
        Board[p].occupantTribe = t;
        var occupant = Instantiate(game.designer.occupantPref, Board[p].transform);
        occupant.GetComponent<SpriteRenderer>().color = game.designer.Colors[t];
        occupant.transform.localScale = new Vector3(3, 3, 1);
        occupant.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = game.designer.Sprites[t];
        FlushTribe();
    }

    private void Kill(Point p)
    {
        Board[p].occupantTribe = Tribes.None;
        Destroy(Board[p].transform.GetChild(0).gameObject);
        FlushTribe();
    }

    private void Push(Point pusher, Point pushed)
    {
        var delta = pushed.Sub(pusher);
        var curr = pushed;
        while (IsFree(curr.Add(delta)))
            curr = curr.Add(delta);
        if (curr == pushed)
            return;
        Drag(pushed, curr);
    }

    private void Pull(Point puller, Point pulled)
    {
        var delta = puller.Sub(pulled).Uni();
        var curr = pulled;
        while (IsFree(curr.Add(delta)))
            curr = curr.Add(delta);
        if (curr == pulled)
            return;
        Drag(pulled, curr);
    }

    private void Convert(Point p, Tribes tribe)
    {
        Kill(p);
        Spawn(p, tribe);
    }

    private void Drag(Point from, Point to)
    {
        Spawn(to, GetOccupantTribe(from));
        Kill(from);
        //TODO: Replace with smooth movement
    }

    #endregion

    private void FlushTribe()
    {
        AnchorTribeZ = Tribes.None;
        AnchorTribeF = Tribes.None;
    }

    private void FlushAnchor()
    {
        AnchorZ = Point.Empty;
        AnchorF = Point.Empty;
    }

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
            FlushAnchor();
            FlushTribe();
            CheckWin();
            game.EndTurn();
            return;
        }

        AudioStatic.PlaySound(CurrentAction, AnchorTribeZ);

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
                Kill(currp);
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
                Kill(currp);
            }

            game.opponent.CompleteTemplate(template.Template);
        }

        if (game.opponent.CompletedCount()[SchemaType.Big] == 1 ||
            game.opponent.CompletedCount()[SchemaType.Small] == 2)
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
        AnchorZ = p;
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

        SelectPoint(possible[random.Next(possible.Length)]);
    }

    private Point CornerLu =>
        Board.Keys.First(p => p.X == Board.Keys.Min(px => px.X) && p.Y == Board.Keys.Max(py => py.Y));

    private Point CornerRd =>
        Board.Keys.First(p => p.X == Board.Keys.Max(px => px.X) && p.Y == Board.Keys.Min(py => py.Y));
}

internal static class EngineExtensions
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

    public static IEnumerable<Point> GetColumn(this Point p, IEnumerable<Point> board)
    {
        return board.Where(point => point != p && point.X == p.X);
    }

    public static IEnumerable<Point> GetRow(this Point p, IEnumerable<Point> board)
    {
        return board.Where(point => point != p && point.Y == p.Y);
    }

    public static IEnumerable<Point> GetCross(this Point p, IEnumerable<Point> board)
    {
        return board.Where(point => point != p && point.Sub(p).Uni().X * point.Sub(p).Uni().Y != 0);
    }

    public static Point Add(this Point source, Point other) => new Point(source.X + other.X, source.Y + other.Y);
    private static Point Neg(this Point source) => new Point(-source.X, -source.Y);
    public static Point Sub(this Point source, Point other) => source.Add(other.Neg());

    public static Point Uni(this Point source) =>
        new Point(source.X == 0 ? 0 : Math.Sign(source.X), source.Y == 0 ? 0 : Math.Sign(source.Y));
}