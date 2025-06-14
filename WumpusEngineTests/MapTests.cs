using Lea;
using Moq;
using System.Diagnostics.CodeAnalysis;
using WumpusEngine;

namespace WumpusEngineTests
{
    [ExcludeFromCodeCoverage]
    public class MapTests
    {
        private Mock<IEventAggregator> _eventAggregatorMock;

        [SetUp]
        public void Setup()
        {
            _eventAggregatorMock = new Mock<IEventAggregator>();
        }

        [Test]
        public void MapCtor()
        {
            var actual = new WumpusMap(_eventAggregatorMock.Object, DifficultyOptions.Easy);

            Assert.That(actual.DifficultyOptions.Difficulty, Is.EqualTo(GameDifficulty.Easy));
        }

        [Test]
        public void MapCtorRandom()
        {
            var actual = new WumpusMap(_eventAggregatorMock.Object, DifficultyOptions.Hard, new RandomHelper());

            Assert.That(actual.DifficultyOptions.Difficulty, Is.EqualTo(GameDifficulty.Hard));
        }

        [Test]
        public void MapSetRandomBatLocation()
        {
            var map = new WumpusMap(_eventAggregatorMock.Object, new DifficultyOptions(GameDifficulty.Normal, 0, 0, 0, 0));
            map.SetRandomBatLocation();

            var actual = GetCavernWhere(map, c => c.HasBat).SingleOrDefault();

            Assert.That(actual, Is.Not.Null);
        }

        [Test]
        public void MapSetRandomBatLocationAvoidBatCollision()
        {
            var map = new WumpusMap(_eventAggregatorMock.Object, new DifficultyOptions(GameDifficulty.Normal, 0, 0, 0, 0));
            var loc1 = new Location(2, 2);
            var loc2 = new Location(4, 4);
            var randomMock = new Mock<IRandom>();

            randomMock.SetupSequence(x => x.Next(It.IsAny<uint>()))
                .Returns(2).Returns(2)
                .Returns(4).Returns(4);

            map.Random = randomMock.Object;
            map[loc1].HasBat = true;
            map.SetRandomBatLocation();
           
            var actual1 = map[loc1].HasBat;
            var actual2 = map[loc2].HasBat;

            Assert.Multiple(() =>
            {
                Assert.That(actual1, Is.True);
                Assert.That(actual2, Is.True);
            });
        }

        [Test]
        public void MapSetRandomBatLocationAvoidTunnel()
        {
            var map = new WumpusMap(_eventAggregatorMock.Object, new DifficultyOptions(GameDifficulty.Normal, 0, 0, 15, 0));
            var loc1 = GetCavernWhere(map, c => !c.IsCave).First().Location;
            var loc2 = GetCavernWhere(map, c => c.IsCave).First().Location;
            var randomMock = new Mock<IRandom>();

            randomMock.SetupSequence(x => x.Next(It.IsAny<uint>()))
                .Returns((int)loc1.Row).Returns((int)loc1.Column)
                .Returns((int)loc2.Row).Returns((int)loc2.Column);

            map.Random = randomMock.Object;
            map.SetRandomBatLocation();

            var actual1 = map[loc1].HasBat;
            var actual2 = map[loc2].HasBat;

            Assert.Multiple(() =>
            {
                Assert.That(actual1, Is.False);
                Assert.That(actual2, Is.True);
            });
        }

        [Test]
        public void MapGetWumpusCavern()
        {
            var map = new WumpusMap(_eventAggregatorMock.Object, DifficultyOptions.Normal, new RandomHelper());
            var expected = map.Caverns.Cast<Cavern>().Single(x => x.HasWumpus);
            var actual = map.GetWumpusCavern();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void MapGetPitCaverns()
        {
            var map = new WumpusMap(_eventAggregatorMock.Object, DifficultyOptions.Normal, new RandomHelper());
            var expected = map.Caverns.Cast<Cavern>().Where(x => x.IsPit);
            var actual = map.GetPitCaverns();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void MapGetDirection()
        {
            var map = new WumpusMap(_eventAggregatorMock.Object, DifficultyOptions.Normal, new RandomHelper());
            ConnectAllCaverns(map);

            var start = new Location(1, 1);
            var end = new Location(1, 2);
            var actual = map.GetDirection(start, end);
            Assert.That(actual, Is.EqualTo(Direction.East));

            start = new Location(1, 7);
            end = new Location(1, 0);
            actual = map.GetDirection(start, end);
            Assert.That(actual, Is.EqualTo(Direction.East));

            start = new Location(1, 1);
            end = new Location(1, 3);
            actual = map.GetDirection(start, end);
            Assert.That(actual, Is.Null);

            start = new Location(1, 1);
            end = new Location(1, 0);
            actual = map.GetDirection(start, end);
            Assert.That(actual, Is.EqualTo(Direction.West));

            start = new Location(1, 0);
            end = new Location(1, 7);
            actual = map.GetDirection(start, end);
            Assert.That(actual, Is.EqualTo(Direction.West));

            start = new Location(1, 3);
            end = new Location(1, 1);
            actual = map.GetDirection(start, end);
            Assert.That(actual, Is.Null);

            start = new Location(1, 1);
            end = new Location(2, 1);
            actual = map.GetDirection(start, end);
            Assert.That(actual, Is.EqualTo(Direction.South));

            start = new Location(5, 7);
            end = new Location(0, 7);
            actual = map.GetDirection(start, end);
            Assert.That(actual, Is.EqualTo(Direction.South));

            start = new Location(1, 1);
            end = new Location(3, 1);
            actual = map.GetDirection(start, end);
            Assert.That(actual, Is.Null);

            start = new Location(2, 1);
            end = new Location(1, 1);
            actual = map.GetDirection(start, end);
            Assert.That(actual, Is.EqualTo(Direction.North));

            start = new Location(0, 7);
            end = new Location(5, 7);
            actual = map.GetDirection(start, end);
            Assert.That(actual, Is.EqualTo(Direction.North));

            start = new Location(3, 1);
            end = new Location(1, 1);
            actual = map.GetDirection(start, end);
            Assert.That(actual, Is.Null);
        }

        [Test]
        public void MapTraverseToCell()
        {
            var map = new WumpusMap(_eventAggregatorMock.Object, DifficultyOptions.Normal, new RandomHelper());
            ConnectAllCaverns(map);

            var actual = map.TraverseToCell(map[new Location(0, 0)], new Location(0, 2), false);

            Assert.That(actual, Has.Count.EqualTo(2));
        }

        private static void ConnectAllCaverns(WumpusMap map)
        {
            foreach (var cavern in map.Caverns)
            {
                cavern.North = cavern.North ?? map.GetAdjacentCell(cavern, Direction.North);
                cavern.East = cavern.East ?? map.GetAdjacentCell(cavern, Direction.East);
                cavern.South = cavern.South ?? map.GetAdjacentCell(cavern, Direction.South);
                cavern.West = cavern.West ?? map.GetAdjacentCell(cavern, Direction.West);
            }
        }

        [SuppressMessage("Structure", "NUnit1028:The non-test method is public", Justification = "not public")]
        internal static IEnumerable<Cavern> GetCavernWhere(WumpusMap map, Predicate<Cavern> condition)
        {
            return map.Caverns.Cast<Cavern>().Where(c => condition(c));
        }
    }
}
