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
            var actual = new Map(_eventAggregatorMock.Object, DifficultyOptions.Easy);

            Assert.That(actual.DifficultyOptions.Difficulty, Is.EqualTo(GameDifficulty.Easy));
        }

        [Test]
        public void MapCtorRandom()
        {
            var actual = new Map(_eventAggregatorMock.Object, DifficultyOptions.Hard, new RandomHelper());

            Assert.That(actual.DifficultyOptions.Difficulty, Is.EqualTo(GameDifficulty.Hard));
        }

        [Test]
        public void MapSetRandomBatLocation()
        {
            var map = new Map(_eventAggregatorMock.Object, new DifficultyOptions(GameDifficulty.Normal, 0, 0, 0, 0));
            map.SetRandomBatLocation();

            var actual = GetCavernWhere(map, c => c.HasBat).SingleOrDefault();

            Assert.That(actual, Is.Not.Null);
        }

        [Test]
        public void MapSetRandomBatLocationAvoidBatCollision()
        {
            var map = new Map(_eventAggregatorMock.Object, new DifficultyOptions(GameDifficulty.Normal, 0, 0, 0, 0));
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
            var map = new Map(_eventAggregatorMock.Object, new DifficultyOptions(GameDifficulty.Normal, 0, 0, 15, 0));
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
            var map = new Map(_eventAggregatorMock.Object, DifficultyOptions.Normal, new RandomHelper());
            var expected = map.Caverns.Cast<Cavern>().Single(x => x.HasWumpus);
            var actual = map.GetWumpusCavern();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void MapGetPitCaverns()
        {
            var map = new Map(_eventAggregatorMock.Object, DifficultyOptions.Normal, new RandomHelper());
            var expected = map.Caverns.Cast<Cavern>().Where(x => x.IsPit);
            var actual = map.GetPitCaverns();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [SuppressMessage("Structure", "NUnit1028:The non-test method is public", Justification = "not public")]
        internal static IEnumerable<Cavern> GetCavernWhere(Map map, Predicate<Cavern> condition)
        {
            return map.Caverns.Cast<Cavern>().Where(c => condition(c));
        }
    }
}
