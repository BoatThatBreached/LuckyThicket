using System.Drawing;
using UnityEngine;

public class ActionPointer : MonoBehaviour
{
    //public Engine engine;
    public UpdatedEngine engine;
    private float _ratio;
    private Vector3 _center;
    public SpriteRenderer rend;

    private void Start()
    {
        rend.enabled = false;
        _center = new Vector3(Screen.width, Screen.height) / 2;
    }

    private void Update()
    {
        return;
        // rend.enabled = engine.CurrentAction == Basis.Select&&!engine.Loaded;
        // //rend.enabled = engine.CurrentAction == Basis.Select&&!engine.loaded;
        // if (engine.CurrentAction != Basis.Select || engine.Loaded)
        //     return;
        // var cam = Camera.main;
        // if (cam is null) 
        //     return;
        // _ratio = cam.orthographicSize / Screen.height * 2;
        // var input = (Input.mousePosition - _center) * _ratio + cam.transform.position;
        // var p = new Point(Mathf.RoundToInt(input.x), Mathf.RoundToInt(input.y));
        // transform.position = new Vector3(p.X, p.Y, 0);
        // // if (Input.GetMouseButtonDown(0) && engine.SatisfiesCriterias(p))
        // //     engine.SelectPoint(p);
        // if (Input.GetMouseButtonDown(0) && engine.IsGoodPoint(p))
        //     engine.SelectPoint(p);
    }
}