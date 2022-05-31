using UnityEngine;
using Color = UnityEngine.Color;

public class Tile : MonoBehaviour
{
    public Tribes occupantTribe;
    public Color Color { get=>GetComponent<SpriteRenderer>().color; set=>GetComponent<SpriteRenderer>().color = value; }
}
