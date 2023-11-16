namespace WumpusEngine;

public class Cavern
{
    public Cavern? North { get; internal set; }
    public Cavern? East { get; internal set; }
    public Cavern? South { get; internal set; }
    public Cavern? West { get; internal set; }

    public int NumExits
    {
        get
        {
            var result = 0;

            if (North != null) { result++; }
            if (East != null) { result++; }
            if (South != null) { result++; }
            if (West != null) { result++; }

            return result;
        }
    }

    public bool IsCave { get; internal set; } = true;
    public bool IsPit { get; internal set; } = false;
    public bool IsAdjacentPit { get; internal set; } = false;
    public bool HasBlood { get; internal set; } = false;
    public bool HasBat { get; internal set; } = false;
    public bool HasWumpus { get; internal set; } = false;

    public bool IsRevealed { get; private set; } = false;

    /// <summary>
    /// If the player is in this cavern, the direction the player moved into this cavern, otherwise null.
    /// </summary>
    public Direction? PlayerDirection { get; internal set; }

    public Location Location { get; }

    public Cavern(Location location)
    {
        Location = location;
    }

    public void Reveal()
    {
        IsRevealed = true;
    }

    public Cavern? this[Direction direction]
    {
        get
        {
            return direction switch
            {
                Direction.North => North,
                Direction.East => East,
                Direction.South => South,
                Direction.West => West,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), $"Invalid Direction value {direction}")
            };
        }
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            switch (direction)
            {
                case Direction.North:
                    North = value;
                    value.South = this;
                    break;
                case Direction.East:
                    East = value;
                    value.West = this;
                    break;
                case Direction.South:
                    South = value;
                    value.North = this;
                    break;
                case Direction.West:
                    West = value;
                    value.East = this;
                    break;
            }
        }
    }
}