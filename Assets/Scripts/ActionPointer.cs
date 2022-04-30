using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ActionPointer : MonoBehaviour
{
    public Engine engine;
    private Vector3 offset;
    private float ratio;
    private Vector3 center;
    public SpriteRenderer rend;
    public Point Position { get; set; }

    private void Start()
    {
        ratio = Camera.main.orthographicSize / Screen.height * 2;
        center = new Vector3(Screen.width, Screen.height) / 2;
    }

    void Update()
    {
        rend.enabled = engine.CurrentAction == Basis.Select;
        if (engine.CurrentAction != Basis.Select)
            return;

        var input = (Input.mousePosition - center) * ratio;
        var p = new Point(Mathf.RoundToInt(input.x), Mathf.RoundToInt(input.y));
        Position = p;
        transform.position = new Vector3(p.X, p.Y, 0);
        if (Input.GetMouseButtonDown(0) && engine.SatisfiesCriterias(p))
            engine.SelectPoint(p);
    }
}