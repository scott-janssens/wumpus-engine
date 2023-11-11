namespace WumpusEngine;

public enum GameDifficulty
{
    Easy = 0,
    Normal,
    Hard
}

public class DifficultyOptions
{
    public GameDifficulty Difficulty { get; }
    public int BatCount { get; }
    public int BatCarryPct { get; }
    public int MaxTunnels { get; }
    public int NumPits { get; }

    public static DifficultyOptions Easy => new(GameDifficulty.Easy, 1, 50, 5, 1);
    public static DifficultyOptions Normal => new(GameDifficulty.Normal, 2, 66, 15, 2);
    public static DifficultyOptions Hard => new(GameDifficulty.Hard, 2, 75, 24, 3);

    public DifficultyOptions(GameDifficulty difficulty, int batCount, int batCarryPct, int maxTunnels, int numPits)
    {
        Difficulty = difficulty;
        BatCount = batCount;
        BatCarryPct = batCarryPct;
        MaxTunnels = maxTunnels;
        NumPits = numPits;
    }
}
