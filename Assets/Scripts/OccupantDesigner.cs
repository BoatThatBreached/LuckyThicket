using System.Collections.Generic;
using UnityEngine;

public class OccupantDesigner: MonoBehaviour
{
    public Dictionary<Tribes, Sprite> Sprites;
    public Dictionary<Tribes, Color> Colors;

    public Sprite[] spriteArray;

    void Start()
    {
        Sprites = new Dictionary<Tribes, Sprite>();
        Colors = new Dictionary<Tribes, Color>();
        Add(Tribes.Beaver, spriteArray[0], Color.red);
        Add(Tribes.Magpie, spriteArray[1], Color.blue);
    }

    private void Add(Tribes t, Sprite sp, Color color)
    {
        Sprites[t] = sp;
        Colors[t] = color;
    }

}