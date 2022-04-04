using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Tribes tribe;
    public Queue<Basis> chain;
    private Vector3 offset;
    private float ratio;
    private Vector3 center;
    private Game game;

    private void Start()
    {
        //start mocking
        chain = new Queue<Basis>();
        chain.Enqueue(Basis.SelectFree);
        chain.Enqueue(Basis.ChooseBeaver);
        chain.Enqueue(Basis.SpawnUnit);
        chain.Enqueue(Basis.SelectAdjacentNE);
        chain.Enqueue(Basis.AddTile);
        chain.Enqueue(Basis.SelectEdgeNO);
        chain.Enqueue(Basis.ChooseBeaver); // redundant but is here for clarity;
        chain.Enqueue(Basis.SpawnUnit);
        
        //end mocking
        game = (Game) FindObjectOfType(typeof(Game));
        ratio = Camera.main.orthographicSize / Screen.height * 2;
        center = new Vector3(Screen.width, Screen.height) / 2;
    }

    public void OnMouseDown()
    {
        game.LoadActions(chain);
        //var input = (Input.mousePosition - center) * ratio;
        //offset = transform.position - input;
    }

    public void OnMouseDrag()
    {
        //var input = (Input.mousePosition - center) * ratio;
        //transform.position = input + offset;
    }

    private void OnMouseUp()
    {
        //offset = new Vector3();
        //var p = new Point(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        //if (game.Exists(p) && !game.IsOccupied(p)) //если можно поставить карту на клетку, применяем способности
        //    game.LoadActions(p, chain); //заполняем очередь действий способностями, применяем их относительно точки p.
    }
}