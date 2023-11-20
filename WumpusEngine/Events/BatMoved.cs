using Lea;

namespace WumpusEngine.Events;

public class BatMoved : IEvent
{
    public Location StartLocation { get; }

    public Location PlayerLocation { get; }

    public Location BatLocation { get; }

    public bool GameStateChanged { get; }

    public BatMoved(Location start, Location player, Location end, bool gameStateChanged)
    {
        StartLocation = start;
        PlayerLocation = player;
        BatLocation = end;
        GameStateChanged = gameStateChanged;
    }
}
