using UnityEngine;

public class Crystal : MonoBehaviour
{
    public Game game;

    public void DrawFully()
    {
        while(game.player.Draw(game.player.Character.DeckList))
            print($"{game.player.Character.Login} drawn a card!");
        game.player.Character.Push();
        Destroy(gameObject);
    }
}
