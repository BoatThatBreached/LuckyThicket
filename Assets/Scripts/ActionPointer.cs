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
    private void Start()
    {
        ratio = Camera.main.orthographicSize / Screen.height * 2;
        center = new Vector3(Screen.width, Screen.height) / 2;
    }

    void Update()
    {
        if (game.CurrentAction!=Basis.Select)
            return;
        if(Input.GetMouseButtonDown(0))
        {
            var input = (Input.mousePosition - center) * ratio;
            var p = new Point(Mathf.RoundToInt(input.x), Mathf.RoundToInt(input.y));
            if (game.SatisfiesCriterias(p))
                game.SelectPoint(p);
        }
        
        
    }
}
