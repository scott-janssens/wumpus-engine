namespace WumpusEngine;

public class Location
{
    public uint Row { get; }
    public uint Column { get; }

    public Location(int row, int column)
        :this((uint)row, (uint)column)
    { 
    }

    public Location(uint row, uint column)
    {
        if (row >= WumpusMap.MapHeight) throw new ArgumentOutOfRangeException(nameof(row), $"row is out of range (max of {WumpusMap.MapHeight})");
        if (column >= WumpusMap.MapWidth) throw new ArgumentOutOfRangeException(nameof(column), $"column is out of range (max of {WumpusMap.MapWidth})");

        Row = row;
        Column = column;
    }

    public Location ToDirection(Direction direction)
    {
        return direction switch
        {
            Direction.North => new Location((Row == 0) ? WumpusMap.MapHeight - 1 : Row - 1, Column),
            Direction.East => new Location(Row, (Column == WumpusMap.MapWidth - 1) ? 0 : Column + 1),
            Direction.South => new Location((Row == WumpusMap.MapHeight - 1) ? 0 : Row + 1, Column),
            Direction.West => new Location(Row, (Column == 0) ? WumpusMap.MapWidth - 1 : Column - 1),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), $"Invalid Direction value {direction}")
        };
    }

    public override string ToString() { return $"({Row}, {Column})"; }

    public override bool Equals(object? obj)
    {
        if (obj is not Location otherLocation)
        {
            return false;
        }

        return Row == otherLocation.Row && Column == otherLocation.Column;
    }

    public override int GetHashCode()
    {
        return (int)(Row * WumpusMap.MapWidth + Column); 
    }
}
