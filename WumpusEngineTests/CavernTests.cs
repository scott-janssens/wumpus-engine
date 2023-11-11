using System.Diagnostics.CodeAnalysis;
using WumpusEngine;

namespace WumpusEngineTests
{
    [ExcludeFromCodeCoverage]
    public class CavernTests
    {
        private const int Row = 2;
        private const int Column = 4;
        private static readonly Location Location = new(Row, Column);

        [Test]
        public void CavernCtor()
        {
            var actual = new Cavern(Location);

            Assert.Multiple(() =>
            {
                Assert.That(actual, Is.Not.Null);
                Assert.That(actual.Location, Is.EqualTo(Location));
                Assert.That(actual[Direction.North], Is.Null);
                Assert.That(actual[Direction.East], Is.Null);
                Assert.That(actual[Direction.South], Is.Null);
                Assert.That(actual[Direction.West], Is.Null);
                Assert.That(actual.NumExits, Is.EqualTo(0));
                Assert.That(actual.IsCave, Is.True);
                Assert.That(actual.IsPit, Is.False);
                Assert.That(actual.IsAdjacentPit, Is.False);
                Assert.That(actual.HasBlood, Is.False);
                Assert.That(actual.HasBat, Is.False);
                Assert.That(actual.HasWumpus, Is.False);
                Assert.That(actual.IsRevealed, Is.False);
            });
        }

        [Test]
        public void CavernSetProps()
        {
            var actual = new Cavern(Location)
            {
                IsCave = false,
                IsPit = true,
                IsAdjacentPit = true,
                HasBlood = true,
                HasBat = true,
                HasWumpus = true
            };

            Assert.Multiple(() => {
                Assert.That(actual.IsCave, Is.False);
                Assert.That(actual.IsPit, Is.True);
                Assert.That(actual.IsAdjacentPit, Is.True);
                Assert.That(actual.HasBlood, Is.True);
                Assert.That(actual.HasBlood, Is.True);
                Assert.That(actual.HasWumpus, Is.True);
            });
        }

        [Test]
        public void CavernReveal()
        {
            var actual = new Cavern(Location);
            actual.Reveal();

            Assert.That(actual.IsRevealed, Is.True);
        }

        [Test]
        public void CavernIndexer()
        {
            var adjacentNorth = new Cavern(Location);
            var adjacentEast = new Cavern(Location);
            var adjacentSouth = new Cavern(Location);
            var adjacentWest = new Cavern(Location);
            var actual = new Cavern(Location);

            actual[Direction.North] = adjacentNorth;
            Assert.Multiple(() =>
            {
                Assert.That(actual.North, Is.EqualTo(adjacentNorth));
                Assert.That(actual[Direction.North], Is.EqualTo(adjacentNorth));
                Assert.That(actual.NumExits, Is.EqualTo(1));
            });

            actual[Direction.East] = adjacentEast;
            Assert.Multiple(() =>
            {
                Assert.That(actual.East, Is.EqualTo(adjacentEast));
                Assert.That(actual[Direction.East], Is.EqualTo(adjacentEast));
                Assert.That(actual.NumExits, Is.EqualTo(2));
            });

            actual[Direction.South] = adjacentSouth;
            Assert.Multiple(() =>
            {
                Assert.That(actual.South, Is.EqualTo(adjacentSouth));
                Assert.That(actual[Direction.South], Is.EqualTo(adjacentSouth));
                Assert.That(actual.NumExits, Is.EqualTo(3));
            });

            actual[Direction.West] = adjacentWest;
            Assert.Multiple(() =>
            {
                Assert.That(actual.West, Is.EqualTo(adjacentWest));
                Assert.That(actual[Direction.West], Is.EqualTo(adjacentWest));
                Assert.That(actual.NumExits, Is.EqualTo(4));
            });
        }

        [Test]
        public void CavernIndexerSetNull()
        {
            var actual = new Cavern(Location);

            Assert.Throws<ArgumentNullException>(() => actual[Direction.North] = null);
        }

        [Test]
        public void CavernIndexerGetNull()
        {
            var actual = new Cavern(Location);

            Assert.Throws<ArgumentOutOfRangeException>(() => _ = actual[(Direction)42], "Invalid Direction value 42");
        }

        [SuppressMessage("Structure", "NUnit1028:The non-test method is public", Justification = "not public")]
        internal static Direction GetValidDirection(Cavern cavern)
        {
            return cavern.North != null ? Direction.North :
                cavern.East != null ? Direction.East :
                cavern.South != null ? Direction.South :
                Direction.West;
        }

        [SuppressMessage("Structure", "NUnit1028:The non-test method is public", Justification = "not public")]
        internal static Direction Opposite(Direction direction)
        {
            return direction switch
            {
                Direction.North => Direction.South,
                Direction.East => Direction.West,
                Direction.South => Direction.North,
                Direction.West => Direction.East,
                _ => throw new ArgumentOutOfRangeException(nameof(direction))
            };
        }
    }
}
