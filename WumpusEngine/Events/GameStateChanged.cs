using Lea;

namespace WumpusEngine.Events;

public class GameStateChanged : IEvent
{
    public GameState GameState { get; }

    public GameStateChanged(GameState gameState)
    {
        GameState = gameState;
    }
}