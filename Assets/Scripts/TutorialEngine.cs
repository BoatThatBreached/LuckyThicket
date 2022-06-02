using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using Color = UnityEngine.Color;

public class TutorialEngine:MonoBehaviour
{
    public Tutorial tutorial;
    public List<Func<Point, bool>> Criterias;
    private Dictionary<Point, Tile> Board => tutorial.Board;
    public Point selectedPoint;
    public void Awake()
    {
        Criterias = new List<Func<Point, bool>>();
    }

    public void Spawn(Tribes t)
    {
        Board[selectedPoint].occupantTribe = t;
        var occupant = Instantiate(tutorial.designer.occupantPref, Board[selectedPoint].transform);
        occupant.GetComponent<SpriteRenderer>().color = tutorial.designer.Colors[t];
        occupant.transform.localScale = new Vector3(3, 3, 1);
        occupant.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = tutorial.designer.Sprites[t];
        AudioStatic.sounds[Basis.Spawn][t]();
    }
    public void Kill()
    {
        Board[selectedPoint].occupantTribe = Tribes.None;
        foreach(Transform child in Board[selectedPoint].transform)
            Destroy(child.gameObject);
    }

    public void Build(Point p)
    {
        
        AudioStatic.sounds[Basis.Build][Tribes.Beaver]();
        var tile = Instantiate(tutorial.tilePref, transform);
        tile.transform.position = new Vector3(p.X, p.Y, 0);
        var t = tile.GetComponent<Tile>();
        Board[p] = t;
    }
    public void DestroyTile()
    {
        
        AudioStatic.sounds[Basis.Destroy][Tribes.Beaver]();
        Destroy(Board[selectedPoint].gameObject);
        Board.Remove(selectedPoint);
    }

    private bool IsGoodPoint(Point p) => Criterias.All(f => f(p));
    public void ShowPossibleTiles()
    {
        foreach (var p in Board.Keys.Where(IsGoodPoint))
            Board[p].GetComponent<SpriteRenderer>().color = Color.yellow;
    }
    public void HidePossibleTiles()
    {
        foreach (var p in Board.Keys)
            Board[p].GetComponent<SpriteRenderer>().color = Color.white;
    }

    public IEnumerator WaitForSelection()
    {
        var cam = Camera.main;
        if (cam is null)
            yield break;
        var ratio = cam.orthographicSize / Screen.height * 2;
        var center = new Vector3(Screen.width, Screen.height) / 2;
        while (true)
        {
            if (!Input.GetMouseButtonDown(0))
            {
                yield return new WaitForSeconds(Time.deltaTime);
                continue;
            }
            var input = (Input.mousePosition - center) * ratio + cam.transform.position;
            var p = new Point(Mathf.RoundToInt(input.x), Mathf.RoundToInt(input.y));
            print(p.ToCompactString());
            if (!IsGoodPoint(p)) 
            {
                yield return new WaitForSeconds(Time.deltaTime);
                continue;
            }
            selectedPoint = p;
            yield break;
        }
    }
}