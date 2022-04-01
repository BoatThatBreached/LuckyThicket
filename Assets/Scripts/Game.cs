using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject tilePref;
    private Dictionary<Point, Tile> board;
    void Start()
    {
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
}
