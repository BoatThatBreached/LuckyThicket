using UnityEngine;

public class Crystal : MonoBehaviour
{
    public Game game;
    public Tutorial tutorial;
    public void DrawFully()
    {
        if(game!=null)
        {
            if (!game.isMyTurn)
                return;

            while (game.player.Draw(game.player.Character.DeckList))
                print($"{game.player.Character.Login} drawn a card!");
            game.player.Character.Push();
            Destroy(gameObject);
        }
        else
        {
            if(!tutorial.canDraw)
                return;
            tutorial.player.DrawCard(18,tutorial);
            Destroy(gameObject);
        }
    }
}
