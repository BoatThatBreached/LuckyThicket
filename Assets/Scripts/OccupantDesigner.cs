using System.Collections.Generic;
using UnityEngine;

public class OccupantDesigner: MonoBehaviour
{
    public Dictionary<Tribes, Sprite> Sprites;
    public Dictionary<Tribes, Color> Colors;
    
    public Sprite[] spriteArray;
    public GameObject occupantPref;
    
    public void Init()
    {
        Sprites = new Dictionary<Tribes, Sprite>();
        Colors = new Dictionary<Tribes, Color>();
        Add(Tribes.Beaver, spriteArray[0], new Color(155f/255, 103f/255, 60f/255));
        Add(Tribes.Magpie, spriteArray[1], new Color(225f/255, 246f/255, 255f/255));
        Add(Tribes.Obstacle, spriteArray[2], new Color(3f/255, 75f/255, 3f/255));
    }

    private void Add(Tribes t, Sprite sp, Color color)
    {
        Sprites[t] = sp;
        Colors[t] = color;
    }

}