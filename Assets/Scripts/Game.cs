using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using Color = UnityEngine.Color;
using Random = System.Random;

public class Game : MonoBehaviour
{
    public GameObject tilePref;
    public GameObject occupantPref;

    private Queue<Basis> CurrentChain { get; set; }

    // будем заставлять контроллер по очереди выполнять всё, что написано на карте
    private Dictionary<Basis, Action> Actions { get; set; } // этой штукой переводим элементы енума в void-ы
    private Dictionary<Point, Tile> Board { get; set; }
    private Point Anchor { get; set; }
    private Tribes AnchorTribe { get; set; }
    public Basis CurrentAction { get; private set; }

    private List<Func<Point, bool>> Criterias { get; set; }
    private bool NotExistingNeeded { get; set; }
    public OccupantDesigner designer;
    private int Size { get; set; }
    private Random Rand { get; set; }
    private Player[] Players { get; set; }

    public bool CanBeReplaced { get; private set; }

    void Start()
    {
        Size = 3;
        CurrentChain = new Queue<Basis>();
        Board = new Dictionary<Point, Tile>();
        Anchor = Point.Empty;
        AnchorTribe = Tribes.None;
        CanBeReplaced = true;
        NotExistingNeeded = false;
        Criterias = new List<Func<Point, bool>>();

        InitPlayers();
        
        InitActions();
        Rand = new Random();

        for (int i = -Size / 2; i < Size / 2 + 1; i++)
        {
            for (int j = -Size / 2; j < Size / 2 + 1; j++)
            {
                AddTile(new Point(i, j));
            }
        }
    }

    private void InitPlayers()
    {
        Players = new Player[2];
        Players[0] = new Player("Maximus");
        Players[1] = new Player("Michael");
        
        Template bebrus = new Template(new [,]{{Tribes.Beaver, Tribes.Beaver, Tribes.Beaver}}, SchemaType.Big, false);
        Players[0].AddWinTemplate(bebrus);
        Template magpuk = new Template(new [,]{{Tribes.Magpie}, {Tribes.None}, {Tribes.Magpie}}, SchemaType.Big, false);
        Players[1].AddWinTemplate(magpuk);
    }

    private void InitActions()
    {
        Actions = new Dictionary<Basis, Action>();
        Actions[Basis.Build] = () => AddTile(Anchor);
        Actions[Basis.Destroy] = () => DestroyTile(Anchor);
        Actions[Basis.Kill] = () =>
        {
            DestroyUnit(Anchor, AnchorTribe);
            Flush();
        };
        Actions[Basis.Spawn] = () =>
        {
            SpawnUnit(Anchor, AnchorTribe);
            Flush();
        };
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
        };
        Actions[Basis.Also] = () => { };
        Actions[Basis.Random] = TrySelectRandomPoint;
    }

    private void TrySelectRandomPoint()
    {
        Point[] possible;
        if (NotExistingNeeded)
        {
            possible = Board
                .Keys
                .Where(IsEdge)
                .SelectMany(GetAdjacent)
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

    private void Flush()
    {
        AnchorTribe = Tribes.None;
    }

    public bool Exists(Point p) => Board.ContainsKey(p);

    private IEnumerable<Point> GetAdjacent(Point p)
    {
        return Enumerable.Range(-1, 3)
            .SelectMany(x => Enumerable.Range(-1, 3).Select(y => (x, y)))
            .Where(tuple => tuple.x == 0 ^ tuple.y == 0)
            .Select(tuple => new Point(p.X + tuple.x, p.Y + tuple.y));
    }

    private IEnumerable<Point> GetSurrounding(Point p)
    {
        return Enumerable.Range(-1, 3)
            .SelectMany(x => Enumerable.Range(-1, 3).Select(y => (x, y)))
            .Where(tuple => !(tuple.x == 0 && tuple.y == 0))
            .Select(tuple => new Point(p.X + tuple.x, p.Y + tuple.y));
    }

    public Tribes GetOccupantTribe(Point p) => Board[p].occupantTribe;

    public bool HasAdjacent(Point p) => GetAdjacent(p).Any(Exists);
    public bool HasSurrounding(Point p) => GetSurrounding(p).Any(Exists);

    #region Criterias

    public bool IsOccupied(Point p) => Exists(p) && Board[p].occupantTribe != Tribes.None;
    private bool IsFree(Point p) => Exists(p) && Board[p].occupantTribe == Tribes.None;
    public bool IsAdjacentToAnchor(Point p) => GetAdjacent(Anchor).Contains(p);
    public bool IsSurroundingToAnchor(Point p) => GetSurrounding(Anchor).Contains(p);
    public bool IsEdge(Point p) => GetAdjacent(p).Count(Exists) < 4;
    public bool IsOccupiedByAnchorTribe(Point p) => GetOccupantTribe(p) == AnchorTribe;

    #endregion

    #region Operations

    public void AddTile(Point p)
    {
        var tile = Instantiate(tilePref, transform);
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
        var occupant = Instantiate(occupantPref, Board[p].transform);
        occupant.GetComponent<SpriteRenderer>().color = designer.Colors[t];
        occupant.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = designer.Sprites[t];
    }

    private void DestroyUnit(Point p, Tribes t)
    {
        Board[p].occupantTribe = Tribes.None;
        Destroy(Board[p].transform.GetChild(0).gameObject);
    }

    #endregion


    public void TryLoadActions(Queue<Basis> chain)
    {
        Criterias.Clear();
        if (CanBeReplaced)
        {
            Flush();
            CurrentChain.Clear();
            foreach (var act in chain)
                CurrentChain.Enqueue(act);
            Step();
        }
    }

    public bool SatisfiesCriterias(Point p)
    {
        var pred = Criterias.All(crit => crit(p));
        //print($"{p} {pred}");
        return pred;
    }


    private void Step()
    {
        CurrentAction = CurrentChain.Count > 0 ? CurrentChain.Dequeue() : Basis.Idle;
        print(CurrentAction);
        if (CurrentAction == Basis.Idle)
        {
            Anchor = Point.Empty;
            AnchorTribe = Tribes.None;
            CanBeReplaced = true;
            CheckWin();
            return;
        }

        Actions[CurrentAction]();
        if(CurrentAction!=Basis.Select && CurrentAction!=Basis.Idle)
            Step();
    }

    private void CheckWin()
    {
        foreach (var player in Players)
        {
            var list = player.GetTemplatesPlayerCanComplete(Board);
            if (list.Count > 0)
            {
                print($"{player.Name} can complete smth and count is {list.Count}");
                // delete first completed
                var template = list[0];
                foreach (var p in template.Template.Points.Keys)
                {
                    var currp = new Point(p.X + template.StartingPoint.X, p.Y + template.StartingPoint.Y);
                    DestroyUnit(currp, template.Template.Points[p]);
                }
            }
        }
    }

    private void SkipToAlso()
    {
        //CurrentChain.Clear();
        //Anchor = Point.Empty;
        Criterias.Clear();
        //Also will never start with smth relative to anchor
        while (CurrentChain.Count > 0 && CurrentChain.Peek() != Basis.Also)
            CurrentChain.Dequeue();
        Step();
    }

    private bool ShowPossibleTiles()
    {
        if (NotExistingNeeded)
        {
            var pts = Board.Keys
                .Where(IsEdge)
                .SelectMany(GetAdjacent)
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

    public void SelectPoint(Point p)
    {
        CanBeReplaced = false;
        Anchor = p;
        foreach (var t in Board.Values)
            t.Color = Color.white;
        Criterias.Clear();
        NotExistingNeeded = false;
        Step();
    }
}