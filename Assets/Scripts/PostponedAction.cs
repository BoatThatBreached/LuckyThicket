using Point = System.Drawing.Point;

public class PostponedAction
{
    public Basis Action;
    public int Counter;
    public Point Anchor;
    public Tribes AnchorTribe;

    public PostponedAction(Point anchor, Basis action, Tribes anchorTribe = Tribes.None,int counter = 2)
    {
        Action = action;
        Counter = counter;
        Anchor = anchor;
        AnchorTribe = anchorTribe;
    }

    public bool TryTick()
    {
        Counter--;
        return Counter == 0;
    }
}