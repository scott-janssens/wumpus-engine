using Lea;
using WumpusEngine.Events;

namespace WumpusEngine;

public class Cavern
{
    private readonly IEventAggregator _eventAggregator;

    public bool[] Traversed { get; private set; } = new bool[4];
    public void ClearTraversed() => Traversed = new bool[4];

    public Cavern? North { get; set; }
    public Cavern? East { get; set; }
    public Cavern? South { get; set; }
    public Cavern? West { get; set; }

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

    public bool IsCave { get; set; } = true;
    public bool IsPit { get; set; } = false;
    public bool IsAdjacentPit { get; set; } = false;
    public bool HasBlood { get; set; } = false;
    public bool HasWumpus { get; set; } = false;

    private bool _hasBat = false;
    public bool HasBat
    {
        get => _hasBat;
        set
        {
            _hasBat = value;
            if (value)
            {
                _eventAggregator.Publish(new CavernUpdated(this));
            }
        }
    }

    public bool IsRevealed { get; private set; } = false;

    private Direction? _direction;
    /// <summary>
    /// If the player is in this cavern, the direction the player moved into this cavern, otherwise null.
    /// </summary>
    public Direction? PlayerDirection
    {
        get => _direction;
        set
        {
            if (_direction != value)
            {
                _direction = value;
                IsRevealed = true;
                _eventAggregator.Publish(new CavernUpdated(this));
            }
        }
    }

    public bool HasPlayer => _direction != null;

    public Location Location { get; }

    public Cavern(IEventAggregator eventAggregator, Location location)
    {
        _eventAggregator = eventAggregator;
        Location = location;
    }

    public void Reveal()
    {
        if (!IsRevealed)
        {
            IsRevealed = true;
            _eventAggregator.Publish(new CavernUpdated(this));
        }
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