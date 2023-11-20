using CommunityToolkit.Diagnostics;
using Lea;
using WumpusEngine.Events;

namespace WumpusEngine;

public class Engine
{
    private readonly IEventAggregator _eventAggregator;
    private IRandom _random = null!;
    private Direction _lastDirection = Direction.West;

    public Map Map { get; private set; } = null!;
    public Location PlayerLocation { get; private set; } = null!;

    private GameState _gameState;
    public GameState GameState 
    {
        get => _gameState;
        private set
        {
            if (_gameState != value)
            {
                _gameState = value;
                _eventAggregator.Publish(new GameStateChanged(value));
            }
        }
    }

    public Engine(IEventAggregator eventAggregator, DifficultyOptions difficultyOptions, IRandom random)
    {
        _eventAggregator = eventAggregator;
        StartNewGame(difficultyOptions, random);
    }

    public void StartNewGame(DifficultyOptions difficultyOptions, IRandom random)
    {
        Guard.IsNotNull(difficultyOptions);
        Guard.IsNotNull(random);
        difficultyOptions.Validate();

        _random = random;
        Map = new Map(_eventAggregator, difficultyOptions, random);

        while (PlayerLocation == null)
        {
            var loc = GetRandomLocation();
            var cavern = Map[loc];

            if (cavern.IsCave && !cavern.IsPit && !cavern.HasWumpus && !cavern.HasBat)
            {
                SetPlayerLocation(loc, Direction.West);
                cavern.Reveal();
            }
        }

        GameState = GameState.Running;
    }

    public void HandleKeyboardEvent(string key)
    {
        Direction direction;

        switch (key)
        {
            case "ArrowUp":
                direction = Direction.North;
                break;
            case "ArrowRight":
                direction = Direction.East;
                break;
            case "ArrowDown":
                direction = Direction.South;
                break;
            case "ArrowLeft":
                direction = Direction.West;
                break;
            case "Space":
                TriggerFireMode();
                return;
            default:
                return;
        }

        if (GameState == GameState.Running)
        {
            MovePlayer(direction);
        }
        else if (GameState == GameState.Firing)
        {
            FireArrow(direction);
        }
    }

    private void TriggerFireMode() =>
        GameState = GameState == GameState.Running
                    ? GameState.Firing
                    : GameState == GameState.Firing
                        ? GameState.Running
                        : GameState;

    public void FireArrow(Direction direction)
    {
        EndGame(Map[PlayerLocation][direction]?.HasWumpus ?? false ? GameState.Won : GameState.Missed);
    }

    public void MovePlayer(Direction direction)
    {
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

            newCavern.HasBat = false;

            var newBatLocation = Map.SetRandomBatLocation().Location;
            var droppedCavern = Map[newPlayerLocation];

            _eventAggregator.Publish(new BatMoved(newCavern.Location, newPlayerLocation, newBatLocation, droppedCavern.HasWumpus || droppedCavern.IsPit));

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
        set => _random = Map.Random = value;
    }

    internal void SetLastDirection(Direction direction) => _lastDirection = direction;

    internal void SetGameState(GameState state) => _gameState = state;
#endif
}
