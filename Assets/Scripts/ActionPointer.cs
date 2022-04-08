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
    public SpriteRenderer rend;

    private void Start()
    {
        ratio = Camera.main.orthographicSize / Screen.height * 2;
        center = new Vector3(Screen.width, Screen.height) / 2;
    }

    void Update()
    {
        rend.enabled = game.CurrentAction == Basis.Select;
        if (game.CurrentAction != Basis.Select)
            return;

        var input = (Input.mousePosition - center) * ratio;
        var p = new Point(Mathf.RoundToInt(input.x), Mathf.RoundToInt(input.y));
        transform.position = new Vector3(p.X, p.Y, 0);
        if (Input.GetMouseButtonDown(0) && game.SatisfiesCriterias(p))
            game.SelectPoint(p);
    }
}