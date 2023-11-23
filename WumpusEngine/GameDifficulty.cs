using CommunityToolkit.Diagnostics;

namespace WumpusEngine;

public enum GameDifficulty : uint
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

    public void Validate()
    {
        Guard.IsInRange((uint)Difficulty, 0, (uint)GameDifficulty.Hard + 1);
        Guard.IsInRange(BatCount, 0, 3);
        Guard.IsInRange(BatCarryPct, 0, 101);
        Guard.IsInRange(MaxTunnels, 0, 25);
        Guard.IsInRange(NumPits, 0, 4);
    }

    public static DifficultyOptions FromGameDifficulty(GameDifficulty difficulty)
    {
        return difficulty switch
        {
            GameDifficulty.Easy => Easy,
            GameDifficulty.Normal => Normal,
            GameDifficulty.Hard => Hard,
            _ => throw new ArgumentOutOfRangeException(nameof(difficulty))
        };
    }
}
