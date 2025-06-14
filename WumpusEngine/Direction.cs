namespace WumpusEngine;

public enum Direction : uint
{
    North = 0,
    East,
    South,
    West
}

public static class Directions
{
    public static readonly Direction[] All = new Direction[]
    {
        Direction.North,
        Direction.East,
        Direction.South,
        Direction.West
    };

    public static Direction Opposite(Direction direction) => (Direction)(((uint)direction + 2) % 4);
}
