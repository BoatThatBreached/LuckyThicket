using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

public class Tile : MonoBehaviour
{
    public Point position;
    public Tribes occupantTribe;
    public GameObject occupant;
    public Color Color { get=>GetComponent<SpriteRenderer>().color; set=>GetComponent<SpriteRenderer>().color = value; }
}
