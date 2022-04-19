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
    public Point RightBottomPoint { get };
    public SchemaType Type { get; }
    public bool IsRotatable { get; }

    public Template(Tribes[,] schema, SchemaType type, bool isRotatable)
    {
        Points = new Dictionary<Point, Tribes>();

        for (int i = 0; i < schema.GetLength(0); i++)
        for (int j = 0; j < schema.GetLength(1); j++)
            if (schema[i, j] != Tribes.None)
                Points[new Point(i, j)] = schema[i, j];

        RightBottomPoint = new Point(, schema.GetLength(1));
        IsRotatable = isRotatable;
        Type = type;
    }

    public static OffsetToLeftTop()
    {

    }

    public List<Dictionary<Point, Tribes>> GetSchemas()
    {
        var schemas = new List<Dictionary<Point,Tribes>>();

        schemas.Add(Points);
        if (IsRotatable)
            return schemas;

        var invertPoints = Dictionary<Point, Tribes>();

        foreach(var i in Points)
            invertPoints[new Point(i.Key.Y, i.Key.X)] = i.Value;
        schemas.Add(invertSchema);

        schemas.Add(FlipTemplate(Points));
        schemas.Add(FlipTemplate(invertPoints));
    }

    public Dictionary<Point, Tribes> FlipTemplate(Dictionary<Point, Tribes>)
    {
        var newPoints = new Dictionary<Point, Tribes>();

        foreach(var i in Points)
            newPoints[new Point(RightBottomPoint.X - i.Key.X, RightBottomPoint.Y - i.Key.Y)] = i.Value;

        return newPoints;
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
        var schemas = Template.GetSchemas();
        var cnt = schemas.Count;

        foreach (var scheme in sсhemas)
            foreach (var i in schema) { 
                var point = new Point(StartingPoint.X + i.Key.X,
                    StartingPoint.Y + i.Key.Y);
                if (!board.ContainsKey(point) || board[point].occupantTribe != i.Value)
                    cnt--;   
            }
        
        return cnt > 0;
    }
}