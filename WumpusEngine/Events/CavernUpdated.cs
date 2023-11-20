using Lea;

namespace WumpusEngine.Events;

public class CavernUpdated : IEvent
{
    public Cavern Cavern { get; }

    public CavernUpdated(Cavern cavern)
    {
        Cavern = cavern;
    }
}
