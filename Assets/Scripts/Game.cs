using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject tilePref;
    private Queue<Action> currentChain; // будем заставлять контроллер по очереди выполнять всё, что написано на карте
    private Dictionary<Actions, Action<Point>> actions; // этой штукой переводим элементы енума в void-ы
    private Dictionary<Point, Tile> board;
    
    void Start()
    {
        currentChain = new Queue<Action>();
        actions = new Dictionary<Actions, Action<Point>>();
        board = new Dictionary<Point, Tile>();
        for (int i = -2; i < 3; i++)
        {
            for (int j = -2; j < 3; j++)
            {
                var tile = Instantiate(tilePref, transform);
                tile.transform.position = new Vector3(i, j, 0);
                var p = new Point(i, j);
                var t = tile.GetComponent<Tile>();
                t.position = p;
                board[p] = t;
            }
        }
    }

    public bool Exists(Point p)
    {
        throw new NotImplementedException();
    }

    public bool IsOccupied(Point p)
    {
        // чтобы это проверять, клетка должна существовать
        throw new NotImplementedException();
       
    }
    
    public bool HasAdjacent(Point p)
    {
        throw new NotImplementedException();
    }
    
    public bool HasSurrounding(Point p)
    {
        throw new NotImplementedException();
    }
    

    public Tribes GetOccupantTribe(Point p)
    {
        throw new NotImplementedException();
    }

    private void AddTile(Point p)
    {
        throw new NotImplementedException();
    }

    private void DestroyTile(Point p)
    {
        throw new NotImplementedException();
    }

    private void SpawnUnit(Point p, Tribes t)
    {
        throw new NotImplementedException();
    }

    public void LoadActions(Point p, Queue<Actions> chain)
    {
        foreach(var act in chain)
            currentChain.Enqueue(()=>actions[act](p));
    }
}
