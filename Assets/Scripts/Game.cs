using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject tilePref;
    private Queue<Action> currentChain; // будем заставлять контроллер по очереди выполнять всё, что написано на карте
    private Dictionary<Actions, Action<Point>> actions; // этой штукой переводим элементы енума в void-ы
    private Dictionary<Point, Tile> board = new Dictionary<Point, Tile>();

    void Start()
    {
        currentChain = new Queue<Action>();
        actions = new Dictionary<Actions, Action<Point>>();
        for (int i = -2; i < 3; i++)
        {
            for (int j = -2; j < 3; j++)
            {
                AddTile(new Point(i, j));
            }
        }
    }

    public bool Exists(Point p) => board.ContainsKey(p);

    public bool IsOccupied(Point p) => Exists(p) && board[p].occupantTribe != Tribes.None;

    private IEnumerable<Point> GetAdjacent(Point p)
    {
        return Enumerable.Range(-1, 3)
            .SelectMany(x => Enumerable.Range(-1, 3).Select(y => (x, y)))
            .Where(tuple => tuple.x == 0 ^ tuple.y == 0)
            .Select(tuple => new Point(p.X + tuple.x, p.Y + tuple.y));
    }

    public bool HasAdjacent(Point p) => GetAdjacent(p).Any(Exists);


    public bool HasSurrounding(Point p) => GetAdjacent(p).Any(IsOccupied);


    public Tribes GetOccupantTribe(Point p) => board[p].occupantTribe;

    public void AddTile(Point p)
    {
        var tile = Instantiate(tilePref, transform);
        tile.transform.position = new Vector3(p.X, p.Y, 0);
        var t = tile.GetComponent<Tile>();
        board[p] = t;
    }

    public void DestroyTile(Point p)
    {
        var tile = board[p];
        Destroy(tile);
        board.Remove(p);
    }

    public void SpawnUnit(Point p, Tribes t) => board[p].occupantTribe = t;

    public void LoadActions(Point p, Queue<Actions> chain)
    {
        foreach (var act in chain)
            currentChain.Enqueue(() => actions[act](p));
    }
}