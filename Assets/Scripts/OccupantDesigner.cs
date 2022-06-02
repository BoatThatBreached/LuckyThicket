using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccupantDesigner: MonoBehaviour
{
    public Dictionary<Tribes, Sprite> Sprites;
    public Dictionary<Tribes, Color> Colors;
    //public Dictionary<Tribes, Dictionary<Basis, Color>> ProperColors; 
    public Sprite[] spriteArray;
    public GameObject occupantPref;
    
    public void Init()
    {
        Sprites = new Dictionary<Tribes, Sprite>();
        Colors = new Dictionary<Tribes, Color>();
        Add(Tribes.Beaver, spriteArray[0], new Color(155f/255, 103f/255, 60f/255));
        Add(Tribes.Magpie, spriteArray[1], new Color(225f/255, 246f/255, 255f/255));
        Add(Tribes.Obstacle, spriteArray[2], new Color(3f/255, 75f/255, 3f/255));
    //     ProperColors = new Dictionary<Tribes, Dictionary<Basis, Color>>();
    //     ProperColors[Tribes.Beaver] = new Dictionary<Basis, Color>();
    //     ProperColors[Tribes.Magpie] = new Dictionary<Basis, Color>();
    //     ProperColors[Tribes.Obstacle] = new Dictionary<Basis, Color>();
    //     ProperColors[Tribes.Beaver][Basis.Await] = new Color(155f / 255, 103f / 255, 60f / 255, 128f / 255);
    //     ProperColors[Tribes.Beaver][Basis.Idle] = new Color(155f / 255, 103f / 255, 60f / 255);
    //     ProperColors[Tribes.Obstacle][Basis.Idle] = new Color(3f/255, 75f/255, 3f/255, 128f/255);
    //     ProperColors[Tribes.Obstacle][Basis.Idle] = new Color(3f/255, 75f/255, 3f/255);
    //     ProperColors[Tribes.Magpie][Basis.Await] = new Color(225f/255, 246f/255, 255f/255, 128f / 255);
    //     ProperColors[Tribes.Magpie][Basis.Idle] = new Color(225f/255, 246f/255, 255f/255);
    //
    }

    private void Add(Tribes t, Sprite sp, Color color)
    {
        Sprites[t] = sp;
        Colors[t] = color;
    }

    public static IEnumerator Move(Transform who, Transform to)
    {
        var startPos = who.position;
        var delta = to.position-who.position;
        var passed = 0f;
        const float maxPassed = 0.35f;
        who.SetParent(to);
        while (passed < maxPassed)
        {
            var dt = Time.deltaTime;
            passed += dt;
            who.position = startPos + passed / maxPassed * delta;
            yield return new WaitForSeconds(dt);
        }
        who.position = to.position;
    }

}