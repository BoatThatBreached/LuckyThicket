using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;
using System.Linq;

public class Opponent : MonoBehaviour
{
    public TMP_Text nameField;
    private PlayerCharacter Character;
    public RectTransform bigTemplateSlot;
    public RectTransform[] smallTemplatesSlots;
    public GameObject templateTilePref;
    public Image avatar;
    //TODO: delete this class and replace it by Player instance.
    public string Name
    {
        
        get => nameField.text;
        set => nameField.text = value;
    }
    public bool HasWon => Character.TemplatesList.Count(t=>t.Type==SchemaType.Big)==0 
                          || Character.TemplatesList.Count(t=>t.Type==SchemaType.Small)==0;
    
    public void Init()
    {
        Character = Account.Room.Other;
        avatar.color = GetSecretColor();
        RefreshTemplates();
    }

    private Color GetSecretColor()
    {
        var hash = (float)name.Select(ch=>(int)ch).Sum();
        var max = (float) name.Select(ch => (int) ch).Max() * Name.Length;
        return new Color(hash/max, hash/max*hash/max, hash/max/2);
    }

    public void CompleteTemplate(Template template)
    {
        Character.TemplatesList.Remove(template);
    }

    public List<PositionedTemplate> GetTemplatesPlayerCanComplete(Dictionary<Point, Tile> board)
    {
        var result = new List<PositionedTemplate>();
        foreach (var i in Character.TemplatesList)
        foreach (var j in board)
        {
            var posTemp = new PositionedTemplate(j.Key, i);
            if (posTemp.CheckIfMatch(board))
                result.Add(posTemp);
        }

        return result;
    }
    
    private void RefreshTemplates()
    {
        foreach (Transform child in bigTemplateSlot)
            Destroy(child.gameObject);
        foreach (var slot in smallTemplatesSlots)
        foreach (Transform child in slot)
            Destroy(child.gameObject);

        var smallTemplates = Character.TemplatesList.Where(t => t.Type == SchemaType.Small);
        var index = 0;
        foreach (var t in smallTemplates)
        {
            var width = t.Points.Keys.Select(p => p.X).Max() + 1;
            var height = t.Points.Keys.Select(p => p.Y).Max() + 1;
            for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
            {
                var templateTile = Instantiate(templateTilePref, smallTemplatesSlots[index]);
                templateTile.transform.position = smallTemplatesSlots[index].position + new Vector3(i+ (3 - width)/2f, j+ (3 - height)/2f, 0) * 0.45f*0.8f;
                var point = new Point(i, j);
                if (!t.Points.ContainsKey(point))
                    continue;
                if (t.Points[point] == Tribes.Beaver)
                    templateTile.GetComponent<Image>().color = (Color.red * 2 + Color.green) / 3;
                if (t.Points[point] == Tribes.Magpie)
                    templateTile.GetComponent<Image>().color = (Color.blue * 2 + Color.white) / 3;
            }

            index++;
        }

        var bigTemplates = Character.TemplatesList.Where(t => t.Type == SchemaType.Big);
        foreach (var t in bigTemplates)
        {
            var width = t.Points.Keys.Select(p => p.X).Max() + 1;
            var height = t.Points.Keys.Select(p => p.Y).Max() + 1;
            for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
            {
                var templateTile = Instantiate(templateTilePref, bigTemplateSlot);
                templateTile.transform.position = bigTemplateSlot.position + new Vector3(i+ (3 - width)/2f, j+ (3 - height)/2f, 0) * 0.45f * 0.8f;
                var point = new Point(i, j);
                if (!t.Points.ContainsKey(point))
                    continue;
                if (t.Points[point] == Tribes.Beaver)
                    templateTile.GetComponent<Image>().color = (Color.red * 2 + Color.green) / 3;
                if (t.Points[point] == Tribes.Magpie)
                    templateTile.GetComponent<Image>().color = (Color.blue * 2 + Color.white) / 3;
            }
        }
    }
}