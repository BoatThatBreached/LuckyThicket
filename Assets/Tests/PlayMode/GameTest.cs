using System.Drawing;
using NUnit.Framework;
using UnityEngine;

public class GameTest
{
    private Game empty_game;
    private Game game3x3; // x: [0, 2], y: [0, 2]

    [SetUp]
    public void SetUp()
    {
        var tilePref = Resources.Load("Prefabs/TilePref") as GameObject;
        
        var gameObject = new GameObject("Game");
        empty_game = gameObject.AddComponent<Game>();
        empty_game.tilePref = tilePref;
        
        var gameObject2 = new GameObject("Game2");
        game3x3 = gameObject2.AddComponent<Game>();
        game3x3.tilePref = tilePref;
        
        for (int i = 0; i < 3; ++i)
        {
            for (int j = 0; j < 3; ++j)
            {
                game3x3.gameEngine.AddTile(new Point(i, j));
            }
        }
    }
    [Test]
    public void SimpleAddDestroyTest()
    {
        var p1 = new Point(1, 2);
        var p2 = new Point(2, 2);
        var p3 = new Point(3, 2);

        empty_game.gameEngine.AddTile(p1);
        empty_game.gameEngine.AddTile(p2);
        empty_game.gameEngine.AddTile(p3);
        empty_game.gameEngine.DestroyTile(p2);

        Assert.IsTrue(empty_game.gameEngine.Exists(p1));
        Assert.IsTrue(empty_game.gameEngine.Exists(p3));
        Assert.IsFalse(empty_game.gameEngine.Exists(p2));
    }

    [Test]
    public void SimpleSpawnTest()
    {
        var p0 = new Point(0, 0);
        game3x3.gameEngine.SpawnUnit(p0, Tribes.Beaver);
        Assert.IsTrue(game3x3.gameEngine.IsOccupied(p0));
        var actual = game3x3.gameEngine.GetOccupantTribe(p0);
        Assert.AreEqual(Tribes.Beaver, actual);
    }

    [Test]
    public void SpawnNoneTest()
    {
        // Если спавним Tribes.None, то клетка очищается
        var p0 = new Point(0, 0);
        game3x3.gameEngine.SpawnUnit(p0, Tribes.None);
        Assert.IsFalse(game3x3.gameEngine.IsOccupied(p0));
        var actual = game3x3.gameEngine.GetOccupantTribe(p0);
        Assert.AreEqual(Tribes.None, actual);
    }

    [Test]
    public void HasAdjacentTest()
    {
        var p0 = new Point(0, 0);
        Assert.IsTrue(game3x3.gameEngine.HasAdjacent(p0));

        var p1 = new Point(0, -1);
        Assert.IsTrue(game3x3.gameEngine.HasAdjacent(p1));

        var p2 = new Point(-1, -1);
        Assert.IsFalse(game3x3.gameEngine.HasAdjacent(p2));
    }

    [Test]
    public void HasSurroundingTest()
    {
        var p0 = new Point(0, 0);
        Assert.IsFalse(game3x3.gameEngine.HasSurrounding(p0));
        
        game3x3.gameEngine.SpawnUnit(p0, Tribes.Beaver);
        Assert.IsFalse(game3x3.gameEngine.HasSurrounding(p0));

        var p1 = new Point(1, 0);
        Assert.IsTrue(game3x3.gameEngine.HasSurrounding(p1));
    }
}