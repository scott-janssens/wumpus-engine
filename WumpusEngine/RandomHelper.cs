namespace WumpusEngine;

public interface IRandom
{
    int Next(int maxValue);
    int Next(uint maxValue);
}

public class RandomHelper : IRandom
{
    private Random? _random;

    private int _seed;
    public int Seed
    {
        get => _seed;
        set
        {
#if DEBUG
            Console.WriteLine($"RandomHelper Seed set to {value}");
#endif
            _seed = value;
            _random = new Random(_seed);
        }
    }

    public RandomHelper()
        : this((int)DateTime.Now.Ticks)
    {
    }

    public RandomHelper(int seed)
    {
        Seed = seed;
    }

    public int Next(int maxValue)
    {
        return _random!.Next(maxValue);
    }

    public int Next(uint maxValue)
    {
        return _random!.Next((int)maxValue);
    }
}
