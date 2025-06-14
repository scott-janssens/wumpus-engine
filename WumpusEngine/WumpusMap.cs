using Lea;

namespace WumpusEngine;

public class WumpusMap
{
    private readonly IEventAggregator _eventAggregator;
    private IRandom _random;

    public const uint MapWidth = 8;
    public const uint MapHeight = 6;

    public Cavern[,] Caverns { get; private set; } = new Cavern[0, 0];

    public DifficultyOptions DifficultyOptions { get; }

    public WumpusMap(IEventAggregator eventAggregator, DifficultyOptions difficultyOptions, IRandom? random = null)
    {
        _eventAggregator = eventAggregator;
        DifficultyOptions = difficultyOptions;
        _random = random ?? new RandomHelper();

        while (!CreateMap()) ;
    }

    public Cavern this[Location location]
    {
        get => Caverns[location.Row, location.Column];
    }

    public Cavern SetRandomBatLocation()
    {
        Cavern? cavern = null;

        while (cavern == null)
        {
            var r = _random.Next(MapHeight);
            var c = _random.Next(MapWidth);

            if (!Caverns[r, c].HasBat && Caverns[r, c].IsCave)
            {
                cavern = Caverns[r, c];
                cavern.HasBat = true;
            }
        }

        return cavern!;
    }

    private bool CreateMap()
    {
        // initialize new map
        Caverns = new Cavern[MapHeight, MapWidth];

        for (uint r = 0; r < MapHeight; r++)
        {
            for (uint c = 0; c < MapWidth; c++)
            {
                Caverns[r, c] = new Cavern(_eventAggregator, new Location(r, c));
            }
        }

        // connect caverns
        for (uint r = 0; r < MapHeight; r++)
        {
            for (uint c = 0; c < MapWidth; c++)
            {
                var maxExits = 2;
                var cavern = Caverns[r, c];

                if (GetAdjacentCell(cavern, Direction.North).South != null) maxExits++;
                if (GetAdjacentCell(cavern, Direction.West).East != null) maxExits++;

                var exits = _random.Next(maxExits) + 1;

                while (cavern.NumExits < exits)
                {
                    var exit = (Direction)_random.Next(4);

                    while (cavern[exit] != null && DifficultyOptions.Difficulty < GameDifficulty.Hard)
                    {
                        exit++;

                        if (exit > Direction.West)
                            exit = Direction.North;
                    }

                    cavern[exit] = GetAdjacentCell(cavern, exit);
                }
            }
        }

        // Set Tunnels
        var possibleTunnels = new List<Location>();

        for (uint r = 0; r < MapHeight; r++)
        {
            for (uint c = 0; c < MapWidth; c++)
            {
                var exits = Caverns[r, c].NumExits;

                if (exits == 2 || exits == 4)
                {
                    possibleTunnels.Add(Caverns[r, c].Location);
                }
            }
        }

        for (var tunnelMax = DifficultyOptions.MaxTunnels; tunnelMax > 0 && possibleTunnels.Count > 0; tunnelMax--)
        {
            var newTunnelLoc = possibleTunnels[_random.Next(possibleTunnels.Count)];
            possibleTunnels.Remove(newTunnelLoc);

            this[newTunnelLoc].IsCave = false;
        }

        // Set Pits
        var pits = new List<Cavern>();

        for (var i = 0; i < DifficultyOptions.NumPits;)
        {
            var r = _random.Next(MapHeight);
            var c = _random.Next(MapWidth);

            var cavern = Caverns[r, c];

            if (!cavern.IsPit && cavern.IsCave)
            {
                cavern.IsPit = true;
                pits.Add(cavern);
                i++;
            }
        }

        // Set Pit Warnings
        foreach (var cavern in pits)
        {
            foreach (var direction in (Direction[])Enum.GetValues(typeof(Direction)))
            {
                var adjacent = GetAdjacentCavern(cavern, direction);

                if (adjacent != null && !adjacent.IsPit)
                {
                    adjacent.IsAdjacentPit = true;
                }
            }
        }

        if (!EnsureMapIntegrity())
        {
            return false;
        }

        // Set bats
        for (var i = 0; i < DifficultyOptions.BatCount; i++)
        {
            SetRandomBatLocation();
        }

        // Set Wumpus
        Cavern? wumpusCavern = null;

        while (wumpusCavern == null)
        {
            var r = _random.Next(MapHeight);
            var c = _random.Next(MapWidth);

            if (Caverns[r, c].IsCave)
            {
                wumpusCavern = Caverns[r, c];
                wumpusCavern.HasWumpus = true;
            }
        }

        SetBloodCaverns(wumpusCavern, 2);

        wumpusCavern.HasBlood = false;

        return true;
    }

    /// <summary>
    /// Returns the cavern object 1 cell in the specified direction 
    /// </summary>
    public Cavern GetAdjacentCell(Cavern cavern, Direction direction) => this[cavern.Location.ToDirection(direction)];

    /// <summary>
    /// Returns a list of Cavern objects that are a route from the starting cavern to the specified location.
    /// </summary>
    public IList<Cavern> TraverseToCell(Cavern start, Location end, bool revealed)
    {
        var route = new Stack<Cavern>();

        ClearTraverseTags();
        Traverse(start, end, null, route, revealed);

        return route.ToList();
    }

    private void ClearTraverseTags()
    {
        foreach (var cavern in Caverns)
        {
            cavern.TraverseTag = false;
            cavern.TunnelTag2 = false;
        }
    }

    private bool Traverse(Cavern cavern, Location end, Direction? entrance, Stack<Cavern> route, bool revealed)
    {
        Cavern? adjacent;

        if (cavern.Location.Equals(end))
        {
            return true;
        }

        if (cavern.TraverseTag || (revealed && !cavern.IsRevealed)) return false;

        if (cavern.IsCave)
        {
            foreach (var direction in Directions.All)
            {
                adjacent = cavern[direction];
                if (adjacent != null)
                {
                    cavern.TraverseTag = true;
                    if (Traverse(adjacent, end, Directions.Opposite(direction), route, revealed))
                    {
                        route.Push(adjacent);
                        return true;
                    }
                }
            }
        }
        else
        {
            if (cavern.NumExits == 4)
            {
#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.
                var direction = entrance switch
                {
                    Direction.North => Direction.West,
                    Direction.East => Direction.South,
                    Direction.South => Direction.East,
                    Direction.West => Direction.North,
                    null => Direction.North
                };
#pragma warning restore CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.

                if (direction == Direction.North || direction == Direction.West)
                {
                    if (cavern.TraverseTag)
                    {
                        return false;
                    }

                    cavern.TraverseTag = true;
                }
                else
                {
                    if (cavern.TunnelTag2)
                    {
                        return false;
                    }

                    cavern.TunnelTag2 = true;
                }

                adjacent = GetAdjacentCell(cavern, direction);

                if (Traverse(adjacent, end, Directions.Opposite(direction), route, revealed))
                {
                    route.Push(adjacent);
                    return true;
                }
            }
            else
            {
                cavern.TraverseTag = true;
                foreach (var direction in Directions.All)
                {
                    if (direction == entrance)
                    {
                        continue;
                    }

                    if ((adjacent = cavern[direction]) != null)
                    {
                        if (Traverse(adjacent, end, Directions.Opposite(direction), route, revealed))
                        {
                            route.Push(adjacent);
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    // Returns the first cavern object where IsCave is true in the specified direction from the specified cavern.
    // If no exit exists in that direction, null is returned.
    private Cavern? GetAdjacentCavern(Cavern cavern, Direction direction)
    {
        Cavern? result = null;
        var adjacent = cavern[direction];

        if (adjacent != null)
        {
            if (adjacent.IsCave)
            {
                result = adjacent;
            }
            else
            {
                var tunnel = adjacent;
                var entrance = (Direction)(((uint)direction + 2) % 4);

                if (tunnel.NumExits == 4)
                {
#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.
                    result = entrance switch
                    {
                        Direction.North => GetAdjacentCavern(tunnel, Direction.West),
                        Direction.East => GetAdjacentCavern(tunnel, Direction.South),
                        Direction.South => GetAdjacentCavern(tunnel, Direction.East),
                        Direction.West => GetAdjacentCavern(tunnel, Direction.North),
                    };
#pragma warning restore CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.

                }
                else
                {
                    for (var i = 0; i < 4; i++)
                    {
                        if (i == (uint)entrance)
                        {
                            continue;
                        }

                        if (tunnel[(Direction)i] != null)
                        {
                            result = GetAdjacentCavern(tunnel, (Direction)i);
                        }
                    }
                }
            }
        }

        return result;
    }

    private void SetBloodCaverns(Cavern cavern, uint length)
    {
        cavern.HasBlood = true;

        if (length > 0)
        {
            foreach (var direction in (Direction[])Enum.GetValues(typeof(Direction)))
            {
                var nextCavern = GetAdjacentCavern(cavern, direction);

                if (nextCavern != null)
                {
                    SetBloodCaverns(nextCavern, length - 1);
                }
            }
        }
    }

    private bool EnsureMapIntegrity()
    {
        // Use the bat property to check for dead zones
        var lastDirection = new Stack<Direction>();

        // make sure the starting cell isn't a tunnel with 4 exits which would require special code
        var startCavern = Caverns.Cast<Cavern>().First(x => x.IsCave);
        var cavernsTraversed = new HashSet<Cavern>();

        CaveTrace(startCavern, lastDirection, cavernsTraversed);

        return cavernsTraversed.Count == WumpusMap.MapWidth * WumpusMap.MapHeight;
    }

    private void CaveTrace(Cavern cavern, Stack<Direction> lastDirection, HashSet<Cavern> traversed)
    {
        if (traversed.Contains(cavern))
            return;

        traversed.Add(cavern);

        if (!cavern.IsCave && cavern.NumExits == 4)
        {
            // tunnel cells with 2 tunnels require special handling
            switch (lastDirection.Peek())
            {
                case Direction.North:
                    lastDirection.Push(Direction.East);
                    CaveTrace(cavern.West!, lastDirection, traversed);
                    break;
                case Direction.East:
                    lastDirection.Push(Direction.North);
                    CaveTrace(cavern.South!, lastDirection, traversed);
                    break;
                case Direction.South:
                    lastDirection.Push(Direction.West);
                    CaveTrace(cavern.East!, lastDirection, traversed);
                    break;
                case Direction.West:
                    lastDirection.Push(Direction.South);
                    CaveTrace(cavern.North!, lastDirection, traversed);
                    break;
            }
        }
        else
        {
            if (cavern.North != null)
            {
                lastDirection.Push(Direction.South);
                CaveTrace(cavern.North, lastDirection, traversed);
            }

            if (cavern.East != null)
            {
                lastDirection.Push(Direction.West);
                CaveTrace(cavern.East, lastDirection, traversed);
            }

            if (cavern.South != null)
            {
                lastDirection.Push(Direction.North);
                CaveTrace(cavern.South, lastDirection, traversed);
            }

            if (cavern.West != null)
            {
                lastDirection.Push(Direction.East);
                CaveTrace(cavern.West, lastDirection, traversed);
            }
        }

        lastDirection.Pop();
    }

    internal void ResetBats()
    {
        foreach (var cavern in Caverns)
        {
            cavern.HasBat = false;
        }
    }

    public static bool IsAdjacent(Location location1, Location location2)
    {
        var deltaRow = Math.Abs((int)location1.Row - location2.Row);
        var deltaColumn = Math.Abs((int)location1.Column - location2.Column);

        return deltaRow + deltaColumn == 1 || 
            (deltaRow + deltaColumn == 5 && deltaColumn == 0) ||
            (deltaRow + deltaColumn == 7 && deltaColumn == 7);
    }

    /// <summary>
    /// Returns Direction value to move from the start location to the end location. The locations must be adjacent and
    /// must be traversable.
    /// </summary>
    public Direction? GetDirection(Location start, Location end)
    {
        if (IsAdjacent(start, end))
        {
            if (start.Column != end.Column)
            {
                return (int)start.Column - end.Column == -1 || (start.Column == 7) 
                    ? this[start].East != null ? Direction.East : null
                    : this[start].West != null ? Direction.West : null;
            }

            return (int)start.Row - end.Row == -1 || (start.Row == 5)
                ? this[start].South != null ? Direction.South : null
                : this[start].North != null ? Direction.North : null;
        }

        return null;
    }

#if DEBUG

    internal IRandom Random
    {
        set => _random = value;
    }

    public Cavern GetWumpusCavern()
    {
        return Caverns.Cast<Cavern>().First(x => x.HasWumpus)!;
    }

    public IEnumerable<Cavern> GetPitCaverns()
    {
        return Caverns.Cast<Cavern>().Where(x => x.IsPit);
    }

#endif
}
