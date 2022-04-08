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
        chain = new Queue<Basis>();
        
        Add(Basis.NExisting);
        Add(Basis.Random);
        Add(Basis.Build);
        
        Add(Basis.Free);
        Add(Basis.Select);
        Add(Basis.Beaver);
        Add(Basis.Spawn);
        
        Add(Basis.Surrounding);
        Add(Basis.Free);
        Add(Basis.Random);
        Add(Basis.Magpie);
        Add(Basis.Spawn);
        
        //end mocking
        game = (Game) FindObjectOfType(typeof(Game));
        ratio = Camera.main.orthographicSize / Screen.height * 2;
        center = new Vector3(Screen.width, Screen.height) / 2;
    }

    private void Add(Basis b) => chain.Enqueue(b);

    public void OnMouseDown()
    {
        game.TryLoadActions(chain);
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