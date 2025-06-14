using CommunityToolkit.Diagnostics;
using Lea;
using WumpusEngine.Events;

namespace WumpusEngine;

public class Engine
{
    private readonly IEventAggregator _eventAggregator;
    private IRandom _random = null!;
    private Direction _lastDirection = Direction.West;
    private BatMoved? _lastBatMoved; // The BatMoved event if the bat carried the player this turn. Resets every turn.

    /// <summary>
    /// The Map object of the current game.
    /// </summary>
    public WumpusMap Map { get; private set; } = null!;

    /// <summary>
    /// The location of the player.
    /// </summary>
    public Location PlayerLocation { get; private set; } = null!;

    /// <summary>
    /// The Game Over message when the game reaches and end state.
    /// </summary>
    public string EndGameMessage { get; private set; } = string.Empty;

    private readonly Messages _messages;

    private GameState _gameState;

    /// <summary>
    /// The current state of the game.
    /// </summary>
    public GameState GameState
    {
        get => _gameState;
        private set
        {
            if (_gameState != value)
            {
                var oldGameState = _gameState;

                if (value == GameState.Running)
                {
                    EndGameMessage = string.Empty;
                }

                _gameState = value;
                _eventAggregator.Publish(new GameStateChanged(oldGameState, value));
            }
        }
    }

    public Engine(IEventAggregator eventAggregator, DifficultyOptions difficultyOptions, IRandom random)
    {
        _eventAggregator = eventAggregator;
        _messages = new Messages(random);
        StartNewGame(difficultyOptions, random);
    }

    public void StartNewGame()
    {
        StartNewGame(Map.DifficultyOptions, new RandomHelper());
    }

    public void StartNewGame(DifficultyOptions difficultyOptions, IRandom random)
    {
        Guard.IsNotNull(difficultyOptions);
        Guard.IsNotNull(random);
        difficultyOptions.Validate();

        _random = random;
        _lastBatMoved = null;
        Map = new WumpusMap(_eventAggregator, difficultyOptions, random);

        PlayerLocation = null!;
        while (PlayerLocation == null)
        {
            var loc = GetRandomLocation();
            var cavern = Map[loc];

            if (cavern.IsCave && !cavern.IsPit && !cavern.HasWumpus && !cavern.HasBat)
            {
                SetPlayerLocation(loc, Direction.West);
            }
        }

        GameState = GameState.Running;

        _eventAggregator.Publish(new NewGameStarted());
    }

    public void HandleKeyboardEvent(string key)
    {
        Direction direction;

        _lastBatMoved = null;

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
            case " ":
                TriggerFireMode();
                return;
            case "Escape":
                if (GameState == GameState.Firing)
                {
                    TriggerFireMode();
                }
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

    public void TriggerFireMode() =>
        GameState = GameState == GameState.Running
                    ? GameState.Firing
                    : GameState == GameState.Firing
                        ? GameState.Running
                        : GameState;

    public void FireArrow(Direction direction)
    {
        _lastDirection = direction;
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

            _lastBatMoved = new BatMoved(newCavern.Location, newPlayerLocation, newBatLocation, droppedCavern.HasWumpus || droppedCavern.IsPit);
            _eventAggregator.Publish(_lastBatMoved);

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
        return new Location(_random.Next(WumpusMap.MapHeight), _random.Next(WumpusMap.MapWidth));
    }

    public void EndGame(GameState result)
    {
        EndGameMessage = result switch
        {
            GameState.Missed => _messages.GetMissedDescription(Map[PlayerLocation], _lastDirection),
            GameState.Pit => _messages.GetPitDescription(_lastBatMoved != null),
            GameState.Eaten => _messages.GetEatenDescription(_lastBatMoved != null),
            GameState.Won => _messages.GetVictoryDescription(),
            _ => string.Empty
        };

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
        get => _random;
        set => _random = Map.Random = value;
    }

    internal void SetLastDirection(Direction direction) => _lastDirection = direction;

    internal void SetGameState(GameState state) => _gameState = state;
#endif
}
