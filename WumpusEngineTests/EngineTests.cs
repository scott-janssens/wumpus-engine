using Lea;
using Moq;
using System.Diagnostics.CodeAnalysis;
using WumpusEngine;
using WumpusEngine.Events;

namespace WumpusEngineTests
{
    [ExcludeFromCodeCoverage]
    public class EngineTests
    {
        private static readonly DifficultyOptions DifficultyOptionsBasic = new(GameDifficulty.Normal, 0, 0, 15, 0);
        private static readonly DifficultyOptions DifficultyOptionsMinimal = new(GameDifficulty.Normal, 0, 0, 0, 0);
        private Mock<IEventAggregator> _eventAggregatorMock;

        [SetUp]
        public void Setup()
        {
            _eventAggregatorMock = new Mock<IEventAggregator>();
        }

        [Test]
        public void EngineCtor()
        {
            var actual = new Engine(_eventAggregatorMock.Object, DifficultyOptionsBasic, new RandomHelper());

            Assert.Multiple(() =>
            {
                Assert.That(actual.GameState, Is.EqualTo(GameState.Running));
                Assert.That(actual.Map.Caverns.Cast<Cavern>().Count(), Is.EqualTo(48));
            });
        }

        [Test]
        public void StartNewGame()
        {
            var engine = new Engine(_eventAggregatorMock.Object, DifficultyOptions.Hard, new RandomHelper());

            var game1Difficulty = engine.Map.DifficultyOptions.Difficulty;
            var game1RandomHelper = engine.Random;

            engine.StartNewGame();

            var game2Difficulty = engine.Map.DifficultyOptions.Difficulty;
            var game2RandomHelper = engine.Random;

            Assert.Multiple(() =>
            {
                Assert.That(game2Difficulty, Is.EqualTo(game1Difficulty));
                Assert.That(game2RandomHelper, Is.Not.SameAs(game1RandomHelper));
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

            _eventAggregatorMock.Verify(x => x.Publish(It.Is<CavernUpdated>(e => e.Cavern == endCave)), Times.Once);
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

            _eventAggregatorMock.Verify(x => x.Publish(It.Is<CavernUpdated>(e => e.Cavern == endCave)), Times.Once);
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

            _eventAggregatorMock.Verify(x => x.Publish(It.Is<CavernUpdated>(e => e.Cavern == endCave)), Times.Once);
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

            _eventAggregatorMock.Verify(x => x.Publish(It.Is<CavernUpdated>(e => e.Cavern == endCave)), Times.Once);
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
            var engine = new Engine(_eventAggregatorMock.Object, DifficultyOptionsBasic, new RandomHelper());
            Assert.Throws<ArgumentOutOfRangeException>(() => engine.MovePlayer((Direction)42));
        }

        [Test]
        public void EngineMovePlayerNotRunning()
        {
            var engine = new Engine(_eventAggregatorMock.Object, DifficultyOptionsBasic, new RandomHelper());
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
            var engine = new Engine(_eventAggregatorMock.Object, DifficultyOptionsMinimal, new RandomHelper());
            var wumpus = MapTests.GetCavernWhere(engine.Map, c => c.HasWumpus).Single();
            var direction = CavernTests.GetValidDirection(wumpus);

            engine.SetPlayerLocation(wumpus[direction]!.Location, direction);
            engine.MovePlayer(CavernTests.Opposite(direction));

            Assert.Multiple(() =>
            {
                Assert.That(engine.GameState, Is.EqualTo(GameState.Eaten));
                Assert.That(engine.EndGameMessage.Length, Is.GreaterThan(0));
            });
        }

        [Test]
        public void EngineMovePlayerEndGamePit()
        {
            var engine = new Engine(_eventAggregatorMock.Object, new DifficultyOptions(GameDifficulty.Normal, 0, 0, 0, 3), new RandomHelper());
            var pit = MapTests.GetCavernWhere(engine.Map, c => c.IsPit).First();
            var direction = CavernTests.GetValidDirection(pit);

            ClearWumpus(engine.Map);

            engine.SetPlayerLocation(engine.Map[pit[direction]!.Location]!.Location, direction);
            engine.MovePlayer(CavernTests.Opposite(direction));

            Assert.Multiple(() =>
            {
                Assert.That(engine.GameState, Is.EqualTo(GameState.Pit));
                Assert.That(engine.EndGameMessage.Length, Is.GreaterThan(0));
            });
        }

        [Test]
        public void EngineMovePlayerIntoBatCarry()
        {
            var engine = new Engine(_eventAggregatorMock.Object, new DifficultyOptions(GameDifficulty.Normal, 0, 100, 0, 0), new RandomHelper());
            var wumpus = MapTests.GetCavernWhere(engine.Map, c => c.HasWumpus).Single();
            var batStartCavern = engine.Map[wumpus.Location.ToDirection(Direction.North).ToDirection(Direction.East)];
            var direction = CavernTests.GetValidDirection(batStartCavern);

            batStartCavern.HasBat = true;

            ClearWumpus(engine.Map);

            engine.SetPlayerLocation(engine.Map[batStartCavern[direction]!.Location]!.Location, direction);
            var newPlayerLocation = wumpus.Location;
            var newBatLocation = wumpus.Location.ToDirection(Direction.South).ToDirection(Direction.West);

            engine.Map[newBatLocation].IsCave = true;
            engine.Map[newBatLocation].HasBat = false;

            var randomMock = new Mock<IRandom>();
            randomMock.SetupSequence(x => x.Next(It.IsAny<uint>()))
                .Returns((int)newPlayerLocation.Row)
                .Returns((int)newPlayerLocation.Column)
                .Returns((int)newBatLocation.Row)
                .Returns((int)newBatLocation.Column);
            engine.Random = randomMock.Object;
            engine.MovePlayer(CavernTests.Opposite(direction));

            Assert.Multiple(() =>
            {
                Assert.That(engine.PlayerLocation, Is.Not.EqualTo(batStartCavern.Location));
                Assert.That(engine.PlayerLocation, Is.EqualTo(newPlayerLocation));
                Assert.That(batStartCavern.HasBat, Is.False);
                Assert.That(engine.Map[newBatLocation].HasBat, Is.True);
            });

            _eventAggregatorMock.Verify(x => x.Publish(It.Is<BatMoved>(x =>
                x.StartLocation.Equals(batStartCavern.Location) &&
                x.PlayerLocation.Equals(engine.PlayerLocation) &&
                x.BatLocation.Equals(newBatLocation) &&
                x.GameStateChanged == false)));
        }

        [Test]
        public void EngineMovePlayerIntoBatNoCarry()
        {
            var engine = new Engine(_eventAggregatorMock.Object, new DifficultyOptions(GameDifficulty.Normal, 1, 50, 0, 0), new RandomHelper());
            var bat = MapTests.GetCavernWhere(engine.Map, c => c.HasBat).First();
            var direction = CavernTests.GetValidDirection(bat);

            ClearWumpus(engine.Map);
            engine.SetPlayerLocation(bat.Location.ToDirection(direction), direction);

            var randomMock = new Mock<IRandom>();
            randomMock.Setup(x => x.Next(It.IsAny<int>())).Returns(99);
            engine.Random = randomMock.Object;
            engine.MovePlayer(CavernTests.Opposite(direction));

            Assert.That(engine.PlayerLocation, Is.EqualTo(bat.Location));
            _eventAggregatorMock.Verify(x => x.Publish(It.IsAny<BatMoved>()), Times.Never);
         }

        [Test]
        public void EngineMovePlayerBatSavesFromWumpus()
        {
            var engine = new Engine(_eventAggregatorMock.Object, DifficultyOptionsMinimal, new RandomHelper());
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
            var engine = new Engine(_eventAggregatorMock.Object, new DifficultyOptions(GameDifficulty.Normal, 0, 0, 0, 1), new RandomHelper());
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
            var engine = new Engine(_eventAggregatorMock.Object, DifficultyOptionsMinimal, new RandomHelper());
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
        public void EngineMovePlayerBatDropsOnWumpus()
        {
            var engine = new Engine(_eventAggregatorMock.Object, new DifficultyOptions(GameDifficulty.Normal, 0, 100, 0, 0), new RandomHelper());

            var wumpus = MapTests.GetCavernWhere(engine.Map, c => c.HasWumpus).Single();
            var batStart = engine.Map[wumpus.Location.ToDirection(Direction.East).ToDirection(Direction.North)];
            var batEnd = engine.Map[wumpus.Location.ToDirection(Direction.West)];
            var direction = CavernTests.GetValidDirection(batStart);

            batStart.HasBat = true;
            batEnd.IsCave = true;

            var randomMock = new Mock<IRandom>();
            randomMock.SetupSequence(x => x.Next(It.IsAny<uint>()))
                .Returns((int)wumpus.Location.Row)
                .Returns((int)wumpus.Location.Column)
                .Returns((int)batEnd.Location.Row)
                .Returns((int)batEnd.Location.Column);
            engine.Random = randomMock.Object;

            engine.SetPlayerLocation(engine.Map[batStart[direction]!.Location].Location, direction);
            engine.MovePlayer(CavernTests.Opposite(direction));

            Assert.Multiple(() =>
            {
                Assert.That(engine.GameState, Is.EqualTo(GameState.Eaten));
                Assert.That(engine.PlayerLocation, Is.EqualTo(wumpus.Location));
                Assert.That(batStart.HasBat, Is.False);
                Assert.That(batEnd.HasBat, Is.True);
                Assert.That(engine.EndGameMessage.Length, Is.GreaterThan(0));
            });

            _eventAggregatorMock.Verify(x => x.Publish(It.Is<BatMoved>(x =>
                x.StartLocation.Equals(batStart.Location) &&
                x.PlayerLocation.Equals(engine.PlayerLocation) &&
                x.BatLocation.Equals(batEnd.Location) &&
                x.GameStateChanged == true)));

            _eventAggregatorMock.Verify(x => x.Publish(It.Is<GameStateChanged>(x => x.OldGameState == GameState.Running && x.NewGameState == GameState.Eaten)));
        }

        [Test]
        public void EngineMovePlayerBatDropsOnPit()
        {
            var engine = new Engine(_eventAggregatorMock.Object, new DifficultyOptions(GameDifficulty.Normal, 1, 100, 0, 0), new RandomHelper());

            ClearWumpus(engine.Map);
            
            var batStart = MapTests.GetCavernWhere(engine.Map, c => c.HasBat).Single();
            var pit = engine.Map[batStart.Location.ToDirection(Direction.North).ToDirection(Direction.East)];
            var direction = CavernTests.GetValidDirection(batStart);
            var batEnd = engine.Map[pit.Location.ToDirection(Direction.East)];

            pit.IsCave = pit.IsPit = true;
            batEnd.IsCave = true;

            var randomMock = new Mock<IRandom>();
            randomMock.SetupSequence(x => x.Next(It.IsAny<uint>()))
                .Returns((int)pit.Location.Row)
                .Returns((int)pit.Location.Column)
                .Returns((int)batEnd.Location.Row)
                .Returns((int)batEnd.Location.Column);
            engine.Random = randomMock.Object;

            engine.SetPlayerLocation(engine.Map[batStart[direction]!.Location].Location, direction);
            engine.MovePlayer(CavernTests.Opposite(direction));

            Assert.Multiple(() =>
            {
                Assert.That(engine.GameState, Is.EqualTo(GameState.Pit));
                Assert.That(engine.PlayerLocation, Is.EqualTo(pit.Location));
                Assert.That(batStart.HasBat, Is.False);
                Assert.That(batEnd.HasBat, Is.True);
                Assert.That(engine.EndGameMessage.Length, Is.GreaterThan(0));
            });

            _eventAggregatorMock.Verify(x => x.Publish(It.Is<BatMoved>(x =>
                x.StartLocation.Equals(batStart.Location) &&
                x.PlayerLocation.Equals(engine.PlayerLocation) &&
                x.BatLocation.Equals(batEnd.Location) &&
                x.GameStateChanged == true)));

            _eventAggregatorMock.Verify(x => x.Publish(It.Is<GameStateChanged>(x => x.OldGameState == GameState.Running && x.NewGameState == GameState.Pit)));
        }

        [Test]
        public void EngineFireArrowVictory()
        {
            var engine = new Engine(_eventAggregatorMock.Object, DifficultyOptionsMinimal, new RandomHelper());
            var wumpus = MapTests.GetCavernWhere(engine.Map, c => c.HasWumpus).Single();
            var direction = CavernTests.GetValidDirection(wumpus);

            engine.SetPlayerLocation(wumpus[direction]!.Location, direction);
            engine.HandleKeyboardEvent(" ");
            engine.HandleKeyboardEvent(DirectionToArrowKey(CavernTests.Opposite(direction)));

            Assert.Multiple(() =>
            {
                Assert.That(engine.GameState, Is.EqualTo(GameState.Won));
                Assert.That(engine.EndGameMessage.Length, Is.GreaterThan(0));
            });

            _eventAggregatorMock.Verify(x => x.Publish(It.Is<GameStateChanged>(x => x.OldGameState == GameState.Firing && x.NewGameState == GameState.Won)));
        }

        private static string DirectionToArrowKey(Direction direction)
        {
            return direction switch
            {
                Direction.North => "ArrowUp",
                Direction.East => "ArrowRight",
                Direction.South => "ArrowDown",
                Direction.West => "ArrowLeft",
                _ => throw new NotImplementedException()
            };
        }

        [Test]
        public void EngineFireArrowMiss()
        {
            var engine = new Engine(_eventAggregatorMock.Object, DifficultyOptionsMinimal, new RandomHelper());
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
            var engine = new Engine(_eventAggregatorMock.Object, DifficultyOptionsMinimal, new RandomHelper());
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

        [Test]
        public void EngineKeyboardRunningArrowUp()
        {
            var engine = new Engine(_eventAggregatorMock.Object, DifficultyOptionsMinimal, new RandomHelper());
            SetupAllConnectedMap(engine.Map);

            var playerStart = engine.PlayerLocation;
            _eventAggregatorMock.Reset();

            engine.HandleKeyboardEvent("ArrowUp");

            Assert.Multiple(() =>
            {
                Assert.That(engine.PlayerLocation, Is.EqualTo(playerStart.ToDirection(Direction.North)));
                Assert.That(engine.Map[playerStart].PlayerDirection, Is.Null);
                Assert.That(engine.Map[engine.PlayerLocation].PlayerDirection, Is.EqualTo(Direction.North));
            });

            _eventAggregatorMock.Verify(x => x.Publish(It.Is<CavernUpdated>(e => e.Cavern == engine.Map[playerStart])), Times.Once);
        }

        [Test]
        public void EngineKeyboardRunningArrowRight()
        {
            var engine = new Engine(_eventAggregatorMock.Object, DifficultyOptionsMinimal, new RandomHelper());
            SetupAllConnectedMap(engine.Map);

            var playerStart = engine.PlayerLocation;
            _eventAggregatorMock.Reset();

            engine.HandleKeyboardEvent("ArrowRight");

            Assert.Multiple(() =>
            {
                Assert.That(engine.PlayerLocation, Is.EqualTo(playerStart.ToDirection(Direction.East)));
                Assert.That(engine.Map[playerStart].PlayerDirection, Is.Null);
                Assert.That(engine.Map[engine.PlayerLocation].PlayerDirection, Is.EqualTo(Direction.East));
            });

            _eventAggregatorMock.Verify(x => x.Publish(It.Is<CavernUpdated>(e => e.Cavern == engine.Map[playerStart])), Times.Once);
        }

        [Test]
        public void EngineKeyboardRunningArrowDown()
        {
            var engine = new Engine(_eventAggregatorMock.Object, DifficultyOptionsMinimal, new RandomHelper());
            SetupAllConnectedMap(engine.Map);

            var playerStart = engine.PlayerLocation;
            _eventAggregatorMock.Reset();

            engine.HandleKeyboardEvent("ArrowDown");

            Assert.Multiple(() =>
            {
                Assert.That(engine.PlayerLocation, Is.EqualTo(playerStart.ToDirection(Direction.South)));
                Assert.That(engine.Map[playerStart].PlayerDirection, Is.Null);
                Assert.That(engine.Map[engine.PlayerLocation].PlayerDirection, Is.EqualTo(Direction.South));
            });

            _eventAggregatorMock.Verify(x => x.Publish(It.Is<CavernUpdated>(e => e.Cavern == engine.Map[playerStart])), Times.Once);
        }

        [Test]
        public void EngineKeyboardRunningArrowLeft()
        {
            var engine = new Engine(_eventAggregatorMock.Object, DifficultyOptionsMinimal, new RandomHelper());
            SetupAllConnectedMap(engine.Map);

            var playerStart = engine.PlayerLocation;
            _eventAggregatorMock.Reset();

            engine.HandleKeyboardEvent("ArrowLeft");

            Assert.Multiple(() =>
            {
                Assert.That(engine.PlayerLocation, Is.EqualTo(playerStart.ToDirection(Direction.West)));
                Assert.That(engine.Map[playerStart].PlayerDirection, Is.Null);
                Assert.That(engine.Map[engine.PlayerLocation].PlayerDirection, Is.EqualTo(Direction.West));
            });

            _eventAggregatorMock.Verify(x => x.Publish(It.Is<CavernUpdated>(e => e.Cavern == engine.Map[playerStart])), Times.Once);
        }

        [Test]
        public void EngineKeyboardRunningIgnoredKey()
        {
            var engine = new Engine(_eventAggregatorMock.Object, DifficultyOptionsMinimal, new RandomHelper());
            _eventAggregatorMock.Reset();
            engine.HandleKeyboardEvent("A");

            _eventAggregatorMock.Verify(x => x.Publish(It.IsAny<CavernUpdated>()), Times.Never);
        }

        [Test]
        public void EngineKeyboardRunningToFireMode()
        {
            var engine = new Engine(_eventAggregatorMock.Object, DifficultyOptionsMinimal, new RandomHelper());
            engine.HandleKeyboardEvent(" ");

            Assert.That(engine.GameState, Is.EqualTo(GameState.Firing));
            _eventAggregatorMock.Verify(x => x.Publish(It.IsAny<GameStateChanged>()), Times.Exactly(2));
        }

        [Test]
        public void EngineKeyboardFireModeToRunningSpace()
        {
            var engine = new Engine(_eventAggregatorMock.Object, DifficultyOptionsMinimal, new RandomHelper());
            engine.SetGameState(GameState.Firing);
            engine.HandleKeyboardEvent(" ");

            Assert.That(engine.GameState, Is.EqualTo(GameState.Running));
            _eventAggregatorMock.Verify(x => x.Publish(It.IsAny<GameStateChanged>()), Times.Exactly(2));
        }

        [Test]
        public void EngineKeyboardFireModeToRunningEscape()
        {
            var engine = new Engine(_eventAggregatorMock.Object, DifficultyOptionsMinimal, new RandomHelper());
            engine.SetGameState(GameState.Firing);
            engine.HandleKeyboardEvent("Escape");

            Assert.That(engine.GameState, Is.EqualTo(GameState.Running));
            _eventAggregatorMock.Verify(x => x.Publish(It.IsAny<GameStateChanged>()), Times.Exactly(2));
        }

        [Test]
        public void EngineKeyboardGameStateUnchanged()
        {
            var engine = new Engine(_eventAggregatorMock.Object, DifficultyOptionsMinimal, new RandomHelper());
            engine.SetGameState(GameState.Pit);
            engine.HandleKeyboardEvent(" ");

            Assert.That(engine.GameState, Is.EqualTo(GameState.Pit));
            _eventAggregatorMock.Verify(x => x.Publish(It.IsAny<GameStateChanged>()), Times.Once);
        }

        [Test]
        public void EngineKeyboardFireMode()
        {
            var engine = new Engine(_eventAggregatorMock.Object, DifficultyOptionsMinimal, new RandomHelper());
            ClearWumpus(engine.Map);
            engine.SetGameState(GameState.Firing);
            engine.HandleKeyboardEvent("ArrowUp");

            Assert.That(engine.GameState, Is.EqualTo(GameState.Missed));
            _eventAggregatorMock.Verify(x => x.Publish(It.IsAny<GameStateChanged>()), Times.Exactly(2));
        }

        private static void SetupAllConnectedMap(WumpusMap map)
        {
            ClearWumpus(map);

            foreach (var cavern in map.Caverns)
            {
                cavern.North = map[cavern.Location.ToDirection(Direction.North)];
                cavern.East = map[cavern.Location.ToDirection(Direction.East)];
                cavern.South = map[cavern.Location.ToDirection(Direction.South)];
                cavern.West = map[cavern.Location.ToDirection(Direction.West)];
            }
        }

        private void MovePlayerHelper(Direction direction, out Cavern startCave, out Cavern endCave)
        {
            Engine engine = null!;
            Cavern? startCavern = null;

            while (startCavern == null)
            {
                engine = new Engine(_eventAggregatorMock.Object, DifficultyOptionsMinimal, new RandomHelper(1900588753));
                startCavern = MapTests.GetCavernWhere(engine.Map, c => c[direction] != null && !c.Location.Equals(engine.PlayerLocation)).FirstOrDefault();
            }

            ClearWumpus(engine.Map);
            engine.SetPlayerLocation(startCavern.Location, direction);

            _eventAggregatorMock.Reset();
            engine.MovePlayer(direction);

            startCave = startCavern;
            endCave = engine.Map[engine.PlayerLocation];
        }

        private void MovePlayer4WayTunnelHelper(Direction lastDirection, Direction toDirection, out Cavern startCave, out Cavern endCave)
        {
            Engine engine = null!;
            Cavern? startCavern = null;

            while (startCavern == null)
            {
                engine = new Engine(_eventAggregatorMock.Object, DifficultyOptions.Hard, new RandomHelper());
                startCavern = MapTests.GetCavernWhere(engine.Map, c => !c.IsCave && c.NumExits == 4).FirstOrDefault();
            }

            engine.Map.ResetBats();
            engine.SetPlayerLocation(startCavern.Location, lastDirection);
            engine.SetLastDirection(lastDirection);
            startCave = startCavern;

            engine.MovePlayer(toDirection);
            endCave = engine.Map[engine.PlayerLocation];
        }

        private static void ClearWumpus(WumpusMap map)
        {
            map.Caverns.Cast<Cavern>().ToList().ForEach(c => c.HasWumpus = false);
        }
    }
}
