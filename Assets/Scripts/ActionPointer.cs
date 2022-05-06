using System.Drawing;
using UnityEngine;

public class ActionPointer : MonoBehaviour
{
    public Engine engine;
    private float _ratio;
    private Vector3 _center;
    public SpriteRenderer rend;

    private void Start()
    {
        _center = new Vector3(Screen.width, Screen.height) / 2;
    }

    void Update()
    {
        rend.enabled = engine.CurrentAction == Basis.Select;
        if (engine.CurrentAction != Basis.Select)
            return;

        if (Camera.main is null) return;
        _ratio = Camera.main.orthographicSize / Screen.height * 2;
        var input = (Input.mousePosition - _center) * _ratio + Camera.main.transform.position;
        var p = new Point(Mathf.RoundToInt(input.x), Mathf.RoundToInt(input.y));
        transform.position = new Vector3(p.X, p.Y, 0);
        if (Input.GetMouseButtonDown(0) && engine.SatisfiesCriterias(p))
            engine.SelectPoint(p);
    }
}