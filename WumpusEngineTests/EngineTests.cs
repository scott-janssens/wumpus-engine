using Moq;
using System.Diagnostics.CodeAnalysis;
using WumpusEngine;

namespace WumpusEngineTests
{
    [ExcludeFromCodeCoverage]
    public class EngineTests
    {
        private static readonly DifficultyOptions DifficultyOptionsBasic = new(GameDifficulty.Normal, 0, 0, 15, 0);
        private static readonly DifficultyOptions DifficultyOptionsMinimal = new(GameDifficulty.Normal, 0, 0, 0, 0);

        [Test]
        public void EngineCtor()
        {
            var actual = new Engine(DifficultyOptionsBasic, new RandomHelper());

            Assert.Multiple(() =>
            {
                Assert.That(actual.GameState, Is.EqualTo(GameState.Running));
                Assert.That(actual.Map.Caverns.Cast<Cavern>().Count(), Is.EqualTo(48));
            });
        }

        [Test]
        public void EngineMovePlayerNorth()
        {
            MovePlayerHelper(Direction.North, out var startCave, out var endCave);
            Assert.Multiple(() =>
            {
                Assert.That(endCave.Location, Is.EqualTo(startCave.Location.ToDirection(Direction.North)));
                Assert.That(startCave.PlayerDirection, Is.Null);
                Assert.That(endCave.PlayerDirection, Is.EqualTo(Direction.North));
            });
        }

        [Test]
        public void EngineMovePlayerEast()
        {
            MovePlayerHelper(Direction.East, out var startCave, out var endCave);
            Assert.Multiple(() =>
            {
                Assert.That(endCave.Location, Is.EqualTo(startCave.Location.ToDirection(Direction.East)));
                Assert.That(startCave.PlayerDirection, Is.Null);
                Assert.That(endCave.PlayerDirection, Is.EqualTo(Direction.East));
            });
        }

        [Test]
        public void EngineMovePlayerSouth()
        {
            MovePlayerHelper(Direction.South, out var startCave, out var endCave);
            Assert.Multiple(() =>
            {
                Assert.That(endCave.Location, Is.EqualTo(startCave.Location.ToDirection(Direction.South)));
                Assert.That(startCave.PlayerDirection, Is.Null);
                Assert.That(endCave.PlayerDirection, Is.EqualTo(Direction.South));
            });
        }

        [Test]
        public void EngineMovePlayerWest()
        {
            MovePlayerHelper(Direction.West, out var startCave, out var endCave);
            Assert.Multiple(() =>
            {
                Assert.That(endCave.Location, Is.EqualTo(startCave.Location.ToDirection(Direction.West)));
                Assert.That(startCave.PlayerDirection, Is.Null);
                Assert.That(endCave.PlayerDirection, Is.EqualTo(Direction.West));
            });
        }

        [Test]
        public void EngineMovePlayerLast4WayTunnelNorthLastNorth()
        {
            MovePlayer4WayTunnelHelper(Direction.North, Direction.North, out var startCave, out var endCave);
            Assert.Multiple(() =>
            {
                Assert.That(endCave.Location, Is.EqualTo(startCave.Location));
                Assert.That(endCave.PlayerDirection, Is.EqualTo(Direction.North));
            });
        }

        [Test]
        public void EngineMovePlayerLast4WayTunnelNorthLastEast()
        {
            MovePlayer4WayTunnelHelper(Direction.East, Direction.North, out var startCave, out var endCave);
            Assert.Multiple(() =>
            {
                Assert.That(endCave.Location, Is.EqualTo(startCave.Location.ToDirection(Direction.North)));
                Assert.That(startCave.PlayerDirection, Is.Null);
                Assert.That(endCave.PlayerDirection, Is.EqualTo(Direction.North));
            });
        }

        [Test]
        public void EngineMovePlayerLast4WayTunnelNorthLastSouth()
        {
            MovePlayer4WayTunnelHelper(Direction.South, Direction.North, out var startCave, out var endCave);
            Assert.Multiple(() =>
            {
                Assert.That(endCave.Location, Is.EqualTo(startCave.Location.ToDirection(Direction.North)));
                Assert.That(startCave.PlayerDirection, Is.Null);
                Assert.That(endCave.PlayerDirection, Is.EqualTo(Direction.North));
            });
        }

        [Test]
        public void EngineMovePlayerLast4WayTunnelNorthLastWest()
        {
            MovePlayer4WayTunnelHelper(Direction.West, Direction.North, out var startCave, out var endCave);
            Assert.Multiple(() =>
            {
                Assert.That(endCave.Location, Is.EqualTo(startCave.Location));
                Assert.That(endCave.PlayerDirection, Is.EqualTo(Direction.North));
            });
        }

        [Test]
        public void EngineMovePlayerLast4WayTunnelEastLastNorth()
        {
            MovePlayer4WayTunnelHelper(Direction.North, Direction.East, out var startCave, out var endCave);
            Assert.Multiple(() =>
            {
                Assert.That(endCave.Location, Is.EqualTo(startCave.Location.ToDirection(Direction.East)));
                Assert.That(startCave.PlayerDirection, Is.Null);
                Assert.That(endCave.PlayerDirection, Is.EqualTo(Direction.East));
            });
        }

        [Test]
        public void EngineMovePlayerLast4WayTunnelEastLastEast()
        {
            MovePlayer4WayTunnelHelper(Direction.East, Direction.East, out var startCave, out var endCave);
            Assert.Multiple(() =>
            {
                Assert.That(endCave.Location, Is.EqualTo(startCave.Location));
                Assert.That(endCave.PlayerDirection, Is.EqualTo(Direction.East));
            });
        }

        [Test]
        public void EngineMovePlayerLast4WayTunnelEastLastSouth()
        {
            MovePlayer4WayTunnelHelper(Direction.South, Direction.East, out var startCave, out var endCave);
            Assert.Multiple(() =>
            {
                Assert.That(endCave.Location, Is.EqualTo(startCave.Location));
                Assert.That(endCave.PlayerDirection, Is.EqualTo(Direction.East));
            });
        }

        [Test]
        public void EngineMovePlayerLast4WayTunnelEastLastWest()
        {
            MovePlayer4WayTunnelHelper(Direction.West, Direction.East, out var startCave, out var endCave);
            Assert.Multiple(() =>
            {
                Assert.That(endCave.Location, Is.EqualTo(startCave.Location.ToDirection(Direction.East)));
                Assert.That(startCave.PlayerDirection, Is.Null);
                Assert.That(endCave.PlayerDirection, Is.EqualTo(Direction.East));
            });
        }

        [Test]
        public void EngineMovePlayerLast4WayTunnelSouthLastNorth()
        {
            MovePlayer4WayTunnelHelper(Direction.North, Direction.South, out var startCave, out var endCave);
            Assert.Multiple(() =>
            {
                Assert.That(endCave.Location, Is.EqualTo(startCave.Location.ToDirection(Direction.South)));
                Assert.That(startCave.PlayerDirection, Is.Null);
                Assert.That(endCave.PlayerDirection, Is.EqualTo(Direction.South));
            });
        }

        [Test]
        public void EngineMovePlayerLast4WayTunnelSouthLastEast()
        {
            MovePlayer4WayTunnelHelper(Direction.East, Direction.South, out var startCave, out var endCave);
            Assert.Multiple(() =>
            {
                Assert.That(endCave.Location, Is.EqualTo(startCave.Location));
                Assert.That(endCave.PlayerDirection, Is.EqualTo(Direction.South));
            });
        }

        [Test]
        public void EngineMovePlayerLast4WayTunnelSouthLastSouth()
        {
            MovePlayer4WayTunnelHelper(Direction.South, Direction.South, out var startCave, out var endCave);
            Assert.Multiple(() =>
            {
                Assert.That(endCave.Location, Is.EqualTo(startCave.Location));
                Assert.That(endCave.PlayerDirection, Is.EqualTo(Direction.South));
            });
        }

        [Test]
        public void EngineMovePlayerLast4WayTunnelSouthLastWest()
        {
            MovePlayer4WayTunnelHelper(Direction.West, Direction.South, out var startCave, out var endCave);
            Assert.Multiple(() =>
            {
                Assert.That(endCave.Location, Is.EqualTo(startCave.Location.ToDirection(Direction.South)));
                Assert.That(startCave.PlayerDirection, Is.Null);
                Assert.That(endCave.PlayerDirection, Is.EqualTo(Direction.South));
            });
        }

        [Test]
        public void EngineMovePlayerLast4WayTunnelWestLastNorth()
        {
            MovePlayer4WayTunnelHelper(Direction.North, Direction.West, out var startCave, out var endCave);
            Assert.Multiple(() =>
            {
                Assert.That(endCave.Location, Is.EqualTo(startCave.Location));
                Assert.That(endCave.PlayerDirection, Is.EqualTo(Direction.West));
            });
        }

        [Test]
        public void EngineMovePlayerLast4WayTunnelWestLastEast()
        {
            MovePlayer4WayTunnelHelper(Direction.East, Direction.West, out var startCave, out var endCave);
            Assert.Multiple(() =>
            {
                Assert.That(endCave.Location, Is.EqualTo(startCave.Location.ToDirection(Direction.West)));
                Assert.That(startCave.PlayerDirection, Is.Null);
                Assert.That(endCave.PlayerDirection, Is.EqualTo(Direction.West));
            });
        }

        [Test]
        public void EngineMovePlayerLast4WayTunnelWestLastSouth()
        {
            MovePlayer4WayTunnelHelper(Direction.South, Direction.West, out var startCave, out var endCave);
            Assert.Multiple(() =>
            {
                Assert.That(endCave.Location, Is.EqualTo(startCave.Location.ToDirection(Direction.West)));
                Assert.That(startCave.PlayerDirection, Is.Null);
                Assert.That(endCave.PlayerDirection, Is.EqualTo(Direction.West));
            });
        }

        [Test]
        public void EngineMovePlayerLast4WayTunnelWestLastWest()
        {
            MovePlayer4WayTunnelHelper(Direction.West, Direction.West, out var startCave, out var endCave);
            Assert.Multiple(() =>
            {
                Assert.That(endCave.Location, Is.EqualTo(startCave.Location));
                Assert.That(endCave.PlayerDirection, Is.EqualTo(Direction.West));
            });
        }

        [Test]
        public void EngineMovePlayerBadDirection()
        {
            var engine = new Engine(DifficultyOptionsBasic, new RandomHelper());
            Assert.Throws<ArgumentOutOfRangeException>(() => engine.MovePlayer((Direction)42));
        }

        [Test]
        public void EngineMovePlayerNotRunning()
        {
            var engine = new Engine(DifficultyOptionsBasic, new RandomHelper());
            var expected = engine.PlayerLocation;
            var cavern = engine.Map[engine.PlayerLocation];
            var direction = CavernTests.GetValidDirection(cavern);

            engine.EndGame(GameState.Idle);
            engine.MovePlayer(direction);

            Assert.That(engine.Map[engine.PlayerLocation].Location, Is.EqualTo(expected));
        }

        [Test]
        public void EngineMovePlayerEndGameWumpus()
        {
            var engine = new Engine(DifficultyOptionsMinimal, new RandomHelper());
            var wumpus = MapTests.GetCavernWhere(engine.Map, c => c.HasWumpus).Single();
            var direction = CavernTests.GetValidDirection(wumpus);

            engine.SetPlayerLocation(wumpus[direction]!.Location, direction);
            engine.MovePlayer(CavernTests.Opposite(direction));

            Assert.That(engine.GameState, Is.EqualTo(GameState.Eaten));
        }

        [Test]
        public void EngineMovePlayerEndGamePit()
        {
            var engine = new Engine(new DifficultyOptions(GameDifficulty.Normal, 0, 0, 0, 3), new RandomHelper());
            var pit = MapTests.GetCavernWhere(engine.Map, c => c.IsPit).First();
            var direction = CavernTests.GetValidDirection(pit);

            ClearWumpus(engine.Map);

            engine.SetPlayerLocation(engine.Map[pit[direction]!.Location]!.Location, direction);
            engine.MovePlayer(CavernTests.Opposite(direction));

            
            Assert.That(engine.GameState, Is.EqualTo(GameState.Pit));
        }

        [Test]
        public void EngineMovePlayerIntoBatCarry()
        {
            var engine = new Engine(new DifficultyOptions(GameDifficulty.Normal, 1, 50, 0, 0), new RandomHelper());
            var wumpus = MapTests.GetCavernWhere(engine.Map, c => c.HasWumpus).Single();
            var bat = MapTests.GetCavernWhere(engine.Map, c => c.HasBat).Single();
            var direction = CavernTests.GetValidDirection(bat);

            ClearWumpus(engine.Map);

            engine.SetPlayerLocation(engine.Map[bat[direction]!.Location]!.Location, direction);

            var randomMock = new Mock<IRandom>();
            randomMock.SetupSequence(x => x.Next(It.IsAny<int>())).Returns(0).Returns((int)wumpus.Location.Column).Returns((int)wumpus.Location.Row);
            engine.Random = randomMock.Object;
            engine.MovePlayer(CavernTests.Opposite(direction));

            Assert.That(engine.PlayerLocation, Is.Not.EqualTo(bat.Location));
        }

        [Test]
        public void EngineMovePlayerIntoBatNoCarry()
        {
            var engine = new Engine(new DifficultyOptions(GameDifficulty.Normal, 1, 50, 0, 0), new RandomHelper());
            var bat = MapTests.GetCavernWhere(engine.Map, c => c.HasBat).First();
            var direction = CavernTests.GetValidDirection(bat);

            ClearWumpus(engine.Map);

            engine.SetPlayerLocation(engine.Map[bat[direction]!.Location]!.Location, direction);

            var randomMock = new Mock<IRandom>();
            randomMock.Setup(x => x.Next(It.IsAny<int>())).Returns(99);
            engine.Random = randomMock.Object;
            engine.MovePlayer(CavernTests.Opposite(direction));

            Assert.That(engine.PlayerLocation, Is.EqualTo(bat.Location));
        }

        [Test]
        public void EngineMovePlayerBatSavesFromWumpus()
        {
            var engine = new Engine(DifficultyOptionsMinimal, new RandomHelper());
            var wumpus = MapTests.GetCavernWhere(engine.Map, c => c.HasWumpus).Single();
            var direction = CavernTests.GetValidDirection(wumpus);

            wumpus.HasBat = true;
            engine.SetPlayerLocation(wumpus[direction]!.Location, direction);
            engine.MovePlayer(CavernTests.Opposite(direction));

            Assert.Multiple(() =>
            {
                Assert.That(engine.GameState, Is.EqualTo(GameState.Running));
                Assert.That(engine.PlayerLocation, Is.Not.EqualTo(wumpus.Location));
            });
        }

        [Test]
        public void EngineMovePlayerBatSavesFromPit()
        {
            var engine = new Engine(new DifficultyOptions(GameDifficulty.Normal, 0, 0, 0, 1), new RandomHelper());
            var pit = MapTests.GetCavernWhere(engine.Map, c => c.IsPit).Single();
            var direction = CavernTests.GetValidDirection(pit);

            ClearWumpus(engine.Map);
            pit.HasBat = true;

            engine.SetPlayerLocation(engine.Map[pit[direction]!.Location].Location, direction);
            engine.MovePlayer(CavernTests.Opposite(direction));

            Assert.Multiple(() =>
            {
                Assert.That(engine.GameState, Is.EqualTo(GameState.Running));
                Assert.That(engine.PlayerLocation, Is.Not.EqualTo(pit.Location));
            });
        }

        [Test]
        public void EngineMovePlayerBatSavesFromWumpusAndPit()
        {
            var engine = new Engine(DifficultyOptionsMinimal, new RandomHelper());
            var wumpus = MapTests.GetCavernWhere(engine.Map, c => c.HasWumpus).Single();
            var direction = CavernTests.GetValidDirection(wumpus);

            wumpus.IsPit = true;
            wumpus.HasBat = true;

            engine.SetPlayerLocation(wumpus[direction]!.Location, direction);
            engine.MovePlayer(CavernTests.Opposite(direction));

            Assert.Multiple(() =>
            {
                Assert.That(engine.GameState, Is.EqualTo(GameState.Running));
                Assert.That(engine.PlayerLocation, Is.Not.EqualTo(wumpus.Location));
            });
        }

        [Test]
        public void EngineFireArrowVictory()
        {
            var engine = new Engine(DifficultyOptionsMinimal, new RandomHelper());
            var wumpus = MapTests.GetCavernWhere(engine.Map, c => c.HasWumpus).Single();
            var direction = CavernTests.GetValidDirection(wumpus);

            engine.SetPlayerLocation(wumpus[direction]!.Location, direction);
            engine.FireArrow(CavernTests.Opposite(direction));

            Assert.That(engine.GameState, Is.EqualTo(GameState.Won));
        }

        [Test]
        public void EngineFireArrowMiss()
        {
            var engine = new Engine(DifficultyOptionsMinimal, new RandomHelper());
            var wumpus = MapTests.GetCavernWhere(engine.Map, c => c.HasWumpus).Single();
            var direction = CavernTests.GetValidDirection(wumpus);

            engine.SetPlayerLocation(wumpus[direction]!.Location, direction);
            engine.Map[engine.PlayerLocation][direction] = engine.Map[engine.PlayerLocation];
            engine.FireArrow(direction);

            Assert.That(engine.GameState, Is.EqualTo(GameState.Missed));
        }

        [Test]
        public void EngineFireArrowMissWall()
        {
            var engine = new Engine(DifficultyOptionsMinimal, new RandomHelper());
            var wumpus = MapTests.GetCavernWhere(engine.Map, c => c.HasWumpus).Single();
            var direction = CavernTests.GetValidDirection(wumpus);

            engine.SetPlayerLocation(wumpus[direction]!.Location, direction);
            _ = direction switch
            {
                Direction.North => engine.Map[engine.PlayerLocation].North = null,
                Direction.East => engine.Map[engine.PlayerLocation].East = null,
                Direction.South => engine.Map[engine.PlayerLocation].South = null,
                Direction.West => engine.Map[engine.PlayerLocation].West = null,
                _ => null
            };
            
            engine.FireArrow(direction);

            Assert.That(engine.GameState, Is.EqualTo(GameState.Missed));
        }

        private static void MovePlayerHelper(Direction direction, out Cavern startCave, out Cavern endCave)
        {
            Engine engine = null!;
            Cavern? startCavern = null;

            while (startCavern == null)
            {
                engine = new Engine(DifficultyOptionsMinimal, new RandomHelper());
                startCavern = MapTests.GetCavernWhere(engine.Map, c => c[direction] != null && (c.IsCave || c.NumExits < 4)).FirstOrDefault();
            }

            ClearWumpus(engine.Map);
            engine.SetPlayerLocation(startCavern.Location, direction);
            engine.MovePlayer(direction);

            startCave = startCavern;
            endCave = engine.Map[engine.PlayerLocation];
        }

        private static void MovePlayer4WayTunnelHelper(Direction lastDirection, Direction toDirection, out Cavern startCave, out Cavern endCave)
        {
            Engine engine = null!;
            Cavern? startCavern = null;

            while (startCavern == null)
            {
                engine = new Engine(DifficultyOptions.Hard, new RandomHelper());
                startCavern = MapTests.GetCavernWhere(engine.Map, c => !c.IsCave && c.NumExits == 4).FirstOrDefault();
            }

            engine.Map.ResetBats();
            engine.SetPlayerLocation(startCavern.Location, lastDirection);
            engine.SetLastDirection(lastDirection);
            startCave = startCavern;

            engine.MovePlayer(toDirection);
            endCave = engine.Map[engine.PlayerLocation];
        }

        private static void ClearWumpus(Map map)
        {
            map.Caverns.Cast<Cavern>().ToList().ForEach(c => c.HasWumpus = false);
        }
    }
}
