using System.Diagnostics.CodeAnalysis;
using WumpusEngine;

namespace WumpusEngineTests
{
    [ExcludeFromCodeCoverage]
    public class LocationTests
    {
        private const int Row = 2;
        private const int Column = 4;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void LocationCtor()
        {
            var actual = new Location(Row, Column);

            Assert.Multiple(() =>
            {
                Assert.That(actual.Row, Is.EqualTo(Row));
                Assert.That(actual.Column, Is.EqualTo(Column));
            });
        }

        [Test]
        public void LocationCtorBadRow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Location(42, Column), "row is out of range (max of 6)");
        }

        [Test]
        public void LocationCtorBadColumn()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Location(Row, 42), "column is out of range (max of 8)");
        }

        [Test]
        public void LocationDirections()
        {
            var north = new Location(Row, Column).ToDirection(Direction.North);
            var northWrap = new Location(0, Column).ToDirection(Direction.North);
            var east = new Location(Row, Column).ToDirection(Direction.East);
            var eastWrap = new Location(Row, Map.MapWidth - 1).ToDirection(Direction.East);
            var south = new Location(Row, Column).ToDirection(Direction.South);
            var southWrap = new Location(Map.MapHeight - 1, Column).ToDirection(Direction.South);
            var west = new Location(Row, Column).ToDirection(Direction.West);
            var westWrap = new Location(Row, 0).ToDirection(Direction.West);

            Assert.Multiple(() =>
            {
                Assert.That(north, Is.EqualTo(new Location(Row - 1, Column)));
                Assert.That(northWrap, Is.EqualTo(new Location(Map.MapHeight - 1, Column)));
                Assert.That(east, Is.EqualTo(new Location(Row, Column + 1)));
                Assert.That(eastWrap, Is.EqualTo(new Location(Row, 0)));
                Assert.That(south, Is.EqualTo(new Location(Row + 1, Column)));
                Assert.That(southWrap, Is.EqualTo(new Location(0, Column)));
                Assert.That(west, Is.EqualTo(new Location(Row, Column - 1)));
                Assert.That(westWrap, Is.EqualTo(new Location(Row, Map.MapWidth - 1)));
                Assert.Throws<ArgumentOutOfRangeException>(() => _ = new Location(Row, Column).ToDirection((Direction)42));
            });
        }

        [Test]
        public void LocationEquals()
        {
            var loc1 = new Location(Row, Column);
            var loc2 = new Location(Row, Column);
            var actual = loc1.Equals(loc2);

            Assert.That(actual, Is.True);
        }

        [Test]
        public void LocationNotEquals()
        {
            var loc1 = new Location(Row, Column);
            var loc2 = new Location(Row, Column + 1);
            var actual = loc1.Equals(loc2);
            Assert.That(actual, Is.False);

            loc2 = new Location(Row + 1, Column);
            actual = loc1.Equals(loc2);
            Assert.That(actual, Is.False);

            loc2 = new Location(Row + 1, Column + 1);
            actual = loc1.Equals(loc2);
            Assert.That(actual, Is.False);
        }

        [Test]
        public void LocationNotEqualsNull()
        {
            var loc = new Location(Row, Column);
            var actual = loc.Equals(null);

            Assert.That(actual, Is.False);
        }

        [Test]
        public void LocationNotEqualsNotLocation()
        {
            var loc = new Location(Row, Column);
            var actual = loc.Equals("Nope");

            Assert.That(actual, Is.False);
        }

        [Test]
        public void LocationGetHashCode()
        {
            var actual = new Location(Row, Column).GetHashCode();
            var expected = 20;

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void LocationToString()
        {
            var actual = new Location(Row, Column).ToString();
            var expected = "(2, 4)";

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}