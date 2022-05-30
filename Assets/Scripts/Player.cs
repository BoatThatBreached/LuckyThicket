using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class Player : MonoBehaviour
{
    public TMP_Text nameField;
    public RectTransform[] smallTemplatesSlots;
    public RectTransform bigTemplateSlot;
    public GameObject templateTilePref;
    public Image avatar;
    public string Name
    {
        get => nameField.text;
        set => nameField.text = value;
    }

    public bool HasWon => Character.TemplatesList.Count(t => t.Type == SchemaType.Big) == 0
                          || Character.TemplatesList.Count(t => t.Type == SchemaType.Small) == 0;

    public Game game;
    public GameObject cardPref;
    public PlayerCharacter Character;
    public Transform handPanel;
    
    private Color GetSecretColor()
    {
        var hash = (float)name.Select(ch=>(int)ch).Sum();
        var max = (float) name.Select(ch => (int) ch).Max() * Name.Length;
        return new Color(hash/max, hash/max*hash/max, hash/max/2);
    }

    public void DrawCard(int id)
    {
        var cardCharacter = Account.GetLocalCard(id);
        var card = Instantiate(cardPref, handPanel).GetComponent<Card>();
        card.LoadFrom(cardCharacter, this);
    }

    public bool Draw(List<int> source)
    {
        if (Character.HandList.Count >= 5)
            return false;
        var id = source[0];
        source.RemoveAt(0);
        Character.HandList.Add(id);
        DrawCard(id);
        return true;
    }

    public bool Discard(List<int> source)
    {
        if (source.Count == 0)
            return false;
        var id = source.GetRandom();
        source.Remove(id);
        return true;
    }
    public bool Give(List<int> source)
    {
        if (source.Count == 0)
            return false;
        var id = source.GetRandom();
        source.Remove(id);
        game.opponent.Character.HandList.Add(id);
        return true;
    }

    public void Init()
    {
        Character = Account.Room.Me;
        avatar.color = GetSecretColor();
        RefreshTemplates();
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
                templateTile.transform.position = smallTemplatesSlots[index].position + new Vector3(i+ (3 - width)/2f, j+ (3 - height)/2f, 0) * 0.45f;
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
                templateTile.transform.position = bigTemplateSlot.position + new Vector3(i+ (3 - width)/2f, j+ (3 - height)/2f, 0) * 0.45f;
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
}