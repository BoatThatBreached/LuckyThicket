using System.Collections.Generic;
using System.Drawing;

public enum SchemaType
{
    Big,
    Small
}

//TODO: Rotate

public class Template
{
    public Dictionary<Point, Tribes> Points { get; }
    public SchemaType Type { get; }

    public Template(List<List<Tribes>> schema, SchemaType type)
    {
        Points = new Dictionary<Point, Tribes>();

        for (int i = 0; i < schema.Count; i++)
        for (int j = 0; j < schema[i].Count; j++)
            if (schema[i][j] != Tribes.None)
                Points[new Point(i, j)] = schema[i][j];
        Type = type;
    }
}

public class PositionedTemplate
{
    public Point StartingPoint { get; }

    public Template Template { get; }

    public PositionedTemplate(Point startingPoint, Template template)
    {
        StartingPoint = startingPoint;
        Template = template;
    }

    public bool CheckIfMatch(Dictionary<Point, Tile> board)
    {
        foreach (var i in Template.Points)
        {
            var point = i.Key.Add(StartingPoint);
            if (!board.ContainsKey(point) || board[point].occupantTribe != i.Value)
                return false;
        }

        return true;
    }
}