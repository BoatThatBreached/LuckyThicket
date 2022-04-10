﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

public enum SchemaType
{
	Big,
	Small
}

public class Template
{
	public Dictionary<Point, Tribes> Points { get; }
    public SchemaType Type { get; }
	public bool IsInvert { get; }
	public Template(Tribes [,] schema, SchemaType type, bool isInvert)
	{
		for (int i = 0; i < schema.Length; i++)
			for (int j = 0; j < schema.Length; i++)
				if (schema[i, j] != Tribes.None)
					Points[new Point(i, j)] = schema[i, j];

		IsInvert = isInvert;
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
			var point = StartingPoint + ((Size)i.Key);
			if (!board.ContainsKey(point) || board[point].occupantTribe != i.Value)
				return false;
		}
		return true;
	}
}

public class Player
{
	public string Name { get; }
	public List <Template> Templates { get; set; }
	public List<Template> CompletedTemplates { get; }
	public Player(string name)
	{
		Name = name;
	}

	public void AddWinTemplate(Template template) => Templates.Add(template);

	// важно чтобы сюда передавалась ссылка на темплэйт, находящийся в Templates - сравнение по ссылкам
	public void CompleteTemplate(Template template)
    {
		Templates = Templates.Where(x => x != template).ToList();
		CompletedTemplates.Add(template);
    }
	public Dictionary<SchemaType, int> CompletedCount()
    {
		var dict = new Dictionary<SchemaType, int>() { { SchemaType.Big, 0 }, { SchemaType.Small, 0} };
		foreach (var template in CompletedTemplates)
			dict[template.Type] += 1;
		return dict;
    }

	public List<PositionedTemplate> GetTemplatesPlayerCanComplete(Dictionary<Point, Tile> board)
    {
		var result = new List<PositionedTemplate>();
		foreach (var i in Templates)
			foreach (var j in board)
			{
				var posTemp = new PositionedTemplate(j.Key, i);
				if (posTemp.CheckIfMatch(board))
					result.Add(posTemp);
			}

		return result;
    }
}
