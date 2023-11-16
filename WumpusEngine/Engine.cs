namespace WumpusEngine;

public class Engine
{
    private Direction _lastDirection = Direction.West;
    private IRandom _random = null!;

    public Map Map { get; private set; } = null!;
    public Location PlayerLocation { get; private set; } = null!;
    public GameState GameState { get; private set; }

    public Engine(DifficultyOptions difficultyOptions, IRandom random)
    {
        StartNewGame(difficultyOptions, random);
    }

    public void StartNewGame(DifficultyOptions difficultyOptions, IRandom random)
    {
        _random = random;
        Map = new Map(difficultyOptions, random);

        while (PlayerLocation == null)
        {
            var loc = GetRandomLocation();
            var cavern = Map[loc];

            if (cavern.IsCave && !cavern.IsPit && !cavern.HasWumpus)
            {
                SetPlayerLocation(loc, Direction.West);
                cavern.Reveal();
            }
        }

        GameState = GameState.Running;
    }

    public void FireArrow(Direction direction)
    {
        EndGame(Map[PlayerLocation][direction]?.HasWumpus ?? false ? GameState.Won : GameState.Missed);
    }

    public void MovePlayer(Direction direction)
    {
        if (direction > Direction.West)
        {
            throw new ArgumentOutOfRangeException(nameof(direction));
        }

        if (GameState != GameState.Running)
        {
            return;
        }

        var cavern = Map[PlayerLocation];
        Location? newLocation = null;

        if (!cavern.IsCave && cavern.NumExits == 4)
        {
            // tunnel cells with 2 tunnels require special handling
#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.
            newLocation = direction switch
            {
                Direction.North => (_lastDirection != direction && _lastDirection != Direction.West) ? PlayerLocation.ToDirection(direction) : null,
                Direction.East => (_lastDirection != direction && _lastDirection != Direction.South) ? PlayerLocation.ToDirection(direction) : null,
                Direction.South => (_lastDirection != direction && _lastDirection != Direction.East) ? PlayerLocation.ToDirection(direction) : null,
                Direction.West => (_lastDirection != direction && _lastDirection != Direction.North) ? PlayerLocation.ToDirection(direction) : null
            };
#pragma warning restore CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.
        }
        else
        {
            if (cavern[direction] != null)
            {
                newLocation = PlayerLocation.ToDirection(direction);
            }
        }

        _lastDirection = direction;

        if (newLocation != null)
        {
            SetPlayerLocation(newLocation, direction);
        }
        else
        {
            Map[PlayerLocation].PlayerDirection = direction;
        }
    }

    internal void SetPlayerLocation(Location location, Direction direction)
    {
        if (PlayerLocation != null)
        {
            Map[PlayerLocation].PlayerDirection = null;
        }

        PlayerLocation = location;
        var newCavern = Map[PlayerLocation];
        newCavern.PlayerDirection = direction;
        newCavern.Reveal();

        if (newCavern.HasBat && (newCavern.HasWumpus || newCavern.IsPit || Map.DifficultyOptions.BatCarryPct > _random.Next(100)))
        {
            Location newPlayerLocation = PlayerLocation;

            while (newPlayerLocation.Equals(PlayerLocation))
            {
                newPlayerLocation = GetRandomLocation();
            }

            //var startLocation = newCavern.Location;
            //var batLocation = GetRandomLocation();

            newCavern.HasBat = false;

            //var droppedCavern = Map[newPlayerLocation];

            Map.SetRandomBatLocation();

            // TODO: Add some sort of notify here

            SetPlayerLocation(newPlayerLocation, direction);
        }
        else if (newCavern.HasWumpus)
        {
            EndGame(GameState.Eaten);
        }
        else if (newCavern.IsPit)
        {
            EndGame(GameState.Pit);
        }
    }

    private Location GetRandomLocation()
    {
        return new Location(_random.Next(Map.MapHeight), _random.Next(Map.MapWidth));
    }

    public void EndGame(GameState result)
    {
        GameState = result;
        RevealAll();
    }

    private void RevealAll()
    {
        foreach (var cavern in Map.Caverns)
        {
            cavern.Reveal();
        }
    }

#if DEBUG
    internal IRandom Random
    {
        set => _random = value;
    }

    internal void SetLastDirection(Direction direction) => _lastDirection = direction;
#endif
}
