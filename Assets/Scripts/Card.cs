using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Card: MonoBehaviour
{
    public Tribes tribe;
    public Queue<Actions> chain;
    private Vector3 offset;
    private float ratio;
    private Vector3 center;
    private Game game;

    private void Start()
    {
        //start mocking
        chain = new Queue<Actions>();
        chain.Enqueue(Actions.SpawnBeaver);
        chain.Enqueue(Actions.AddTile);
        //end mocking
        game = (Game)FindObjectOfType(typeof(Game));
        ratio = Camera.main.orthographicSize / Screen.height * 2;
        center =  new Vector3(Screen.width, Screen.height) / 2;
    }

    public void OnMouseDown()
    {
        var input = (Input.mousePosition - center) * ratio;
        offset = transform.position - input;
    }

    public void OnMouseDrag()
    {
        var input = (Input.mousePosition - center) * ratio;
        transform.position = input + offset;
    }

    private void OnMouseUp()
    {
        offset = new Vector3();
        var p = new Point(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        if (!game.IsOccupied(p)) //если можно поставить карту на клетку, применяем способности
            game.LoadActions(p, chain); //заполняем очередь действий способностями, применяем их относительно точки p.
    }
}