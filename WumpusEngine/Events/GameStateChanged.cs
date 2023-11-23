using Lea;

namespace WumpusEngine.Events;

public class GameStateChanged : IEvent
{
    public GameState OldGameState { get; }

    public GameState NewGameState { get; }

    public GameStateChanged(GameState old, GameState @new)
    {
        OldGameState = old;
        NewGameState = @new;
    }
}