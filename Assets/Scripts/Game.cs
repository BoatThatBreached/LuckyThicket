using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

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
    [SerializeField] public OccupantDesigner designer;

    void Start()
    {
        CurrentChain = new Queue<Basis>();
        Board = new Dictionary<Point, Tile>();
        AnchorTribe = Tribes.None;
        InitActions();

        for (int i = -2; i < 3; i++)
        {
            for (int j = -2; j < 3; j++)
            {
                AddTile(new Point(i, j));
            }
        }
    }

    private void InitActions()
    {
        Actions = new Dictionary<Basis, Action>();
        Actions[Basis.AddTile] = ()=>AddTile(Anchor);
        Actions[Basis.DestroyTile] = ()=>DestroyTile(Anchor);
        Actions[Basis.DestroyUnit] = () => DestroyUnit(Anchor, AnchorTribe);
        Actions[Basis.SpawnUnit] = () => SpawnUnit(Anchor, AnchorTribe);
        
        Actions[Basis.ChooseBeaver] = () => AnchorTribe = Tribes.Beaver;
        Actions[Basis.ChooseMagpie] = () => AnchorTribe = Tribes.Magpie;
    }

    private void DestroyUnit(Point p, Tribes t)
    {
        Board[p].occupantTribe = Tribes.None;
        Destroy(Board[p].transform.GetChild(0).gameObject);
    }

    public bool Exists(Point p) => Board.ContainsKey(p);

    public bool IsOccupied(Point p) => Exists(p) && Board[p].occupantTribe != Tribes.None;
    public bool IsOccupiedByAnchorTribe(Point p) => Exists(p) && Board[p].occupantTribe == AnchorTribe;
    public bool IsFree(Point p) => Exists(p) && Board[p].occupantTribe == Tribes.None;

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

    public bool HasAdjacent(Point p) => GetAdjacent(p).Any(Exists);


    //public bool HasSurrounding(Point p) => GetAdjacent(p).Any(IsOccupied);
    public bool HasSurrounding(Point p) => GetSurrounding(p).Any(Exists);

    public Tribes GetOccupantTribe(Point p) => Board[p].occupantTribe;

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

    public void LoadActions(Queue<Basis> chain)
    {
        foreach (var act in chain)
        {
            CurrentChain.Enqueue(act);
        }
        Step();
    }

    public void ApplySelection(Point p)
    {
        Anchor = p;
        Step();
    }

    private void Step()
    {
        CurrentAction = CurrentChain.Count > 0 ? CurrentChain.Dequeue() : Basis.Idle;
        print(CurrentAction);
        if (!Actions.ContainsKey(CurrentAction)) return;
        Actions[CurrentAction]();
        Step();
    }

    public bool IsAdjacentToAnchor(Point p, bool exists = false) => GetAdjacent(p).Contains(Anchor)&&Exists(p)==exists;
    public bool IsSurroundingToAnchor(Point p,bool exists = false) => GetSurrounding(p).Contains(Anchor)&&Exists(p)==exists;
    public bool IsEdge(Point p, bool occ = false) => GetAdjacent(p).Count(Exists) < 4 && occ?IsOccupied(p):IsFree(p);
}