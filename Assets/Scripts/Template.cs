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
    public bool IsRotatable { get; }

    public Template(Tribes[,] schema, SchemaType type, bool isRotatable)
    {
        Points = new Dictionary<Point, Tribes>();

        for (int i = 0; i < schema.GetLength(0); i++)
        for (int j = 0; j < schema.GetLength(1); j++)
            if (schema[i, j] != Tribes.None)
                Points[new Point(i, j)] = schema[i, j];

        IsRotatable = isRotatable;
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
            var point = new Point(StartingPoint.X + i.Key.X,
                StartingPoint.Y + i.Key.Y);
            if (!board.ContainsKey(point) || board[point].occupantTribe != i.Value)
                return false;
        }

        return true;
    }
}