using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ActionPointer : MonoBehaviour
{
    public Game game;
    private Vector3 offset;
    private float ratio;
    private Vector3 center;
    private Dictionary<Basis, Func<Point, bool>> selectors;
    private void Start()
    {
        ratio = Camera.main.orthographicSize / Screen.height * 2;
        center = new Vector3(Screen.width, Screen.height) / 2;
        selectors = new Dictionary<Basis, Func<Point, bool>>();
        selectors[Basis.SelectAdjacentNE] = p => game.IsAdjacentToAnchor(p);
        selectors[Basis.SelectAdjacentE] = p => game.IsAdjacentToAnchor(p, true);
        selectors[Basis.SelectSurroundingNE] = p => game.IsSurroundingToAnchor(p);
        selectors[Basis.SelectSurroundingE] = p => game.IsSurroundingToAnchor(p, true);
        selectors[Basis.SelectEdgeNO] = p => game.IsEdge(p);
        selectors[Basis.SelectFree] = p => game.IsFree(p);
        selectors[Basis.SelectOccupied] = p => game.IsOccupiedByAnchorTribe(p);
    }

    void Update()
    {
        if (!selectors.ContainsKey(game.CurrentAction))
            return;
        if(Input.GetMouseButtonDown(0))
        {
            var input = (Input.mousePosition - center) * ratio;
            //transform.position = input;
            var p = new Point(Mathf.RoundToInt(input.x), Mathf.RoundToInt(input.y));
            if (selectors[game.CurrentAction](p))
                game.ApplySelection(p);
        }
        
        
    }
}
