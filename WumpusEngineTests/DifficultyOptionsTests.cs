using System.Diagnostics.CodeAnalysis;
using WumpusEngine;

namespace WumpusEngineTests
{
    [ExcludeFromCodeCoverage]
    public class DifficultyOptionsTests
    {
        [Test]
        public void DifficultyOptionsEasy()
        {
            var actual = DifficultyOptions.Easy;

            Assert.Multiple(() =>
            {
                Assert.That(actual.Difficulty, Is.EqualTo(GameDifficulty.Easy));
                Assert.That(actual.BatCount, Is.EqualTo(1));
                Assert.That(actual.BatCarryPct, Is.EqualTo(50));
                Assert.That(actual.MaxTunnels, Is.EqualTo(5));
                Assert.That(actual.NumPits, Is.EqualTo(1));
            });
        }

        [Test]
        public void DifficultyOptionsNormal()
        {
            var actual = DifficultyOptions.Normal;

            Assert.Multiple(() =>
            {
                Assert.That(actual.Difficulty, Is.EqualTo(GameDifficulty.Normal));
                Assert.That(actual.BatCount, Is.EqualTo(2));
                Assert.That(actual.BatCarryPct, Is.EqualTo(66));
                Assert.That(actual.MaxTunnels, Is.EqualTo(15));
                Assert.That(actual.NumPits, Is.EqualTo(2));
            });
        }

        [Test]
        public void DifficultyOptionsHard()
        {
            var actual = DifficultyOptions.Hard;

            Assert.Multiple(() =>
            {
                Assert.That(actual.Difficulty, Is.EqualTo(GameDifficulty.Hard));
                Assert.That(actual.BatCount, Is.EqualTo(2));
                Assert.That(actual.BatCarryPct, Is.EqualTo(75));
                Assert.That(actual.MaxTunnels, Is.EqualTo(24));
                Assert.That(actual.NumPits, Is.EqualTo(3));
            });
        }

        [Test]
        public void DifficultyOptionsFromDifficultyEasy()
        {
            var actual = DifficultyOptions.FromGameDifficulty(GameDifficulty.Easy);

            Assert.Multiple(() =>
            {
                Assert.That(actual.Difficulty, Is.EqualTo(GameDifficulty.Easy));
                Assert.That(actual.BatCount, Is.EqualTo(1));
                Assert.That(actual.BatCarryPct, Is.EqualTo(50));
                Assert.That(actual.MaxTunnels, Is.EqualTo(5));
                Assert.That(actual.NumPits, Is.EqualTo(1));
            });
        }

        [Test]
        public void DifficultyOptionsFromDifficultyNormal()
        {
            var actual = DifficultyOptions.FromGameDifficulty(GameDifficulty.Normal);

            Assert.Multiple(() =>
            {
                Assert.That(actual.Difficulty, Is.EqualTo(GameDifficulty.Normal));
                Assert.That(actual.BatCount, Is.EqualTo(2));
                Assert.That(actual.BatCarryPct, Is.EqualTo(66));
                Assert.That(actual.MaxTunnels, Is.EqualTo(15));
                Assert.That(actual.NumPits, Is.EqualTo(2));
            });
        }

        [Test]
        public void DifficultyOptionsFromDifficultyHard()
        {
            var actual = DifficultyOptions.FromGameDifficulty(GameDifficulty.Hard);

            Assert.Multiple(() =>
            {
                Assert.That(actual.Difficulty, Is.EqualTo(GameDifficulty.Hard));
                Assert.That(actual.BatCount, Is.EqualTo(2));
                Assert.That(actual.BatCarryPct, Is.EqualTo(75));
                Assert.That(actual.MaxTunnels, Is.EqualTo(24));
                Assert.That(actual.NumPits, Is.EqualTo(3));
            });
        }

        [Test]
        public void DifficultyOptionsFromBadValue ()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => DifficultyOptions.FromGameDifficulty((GameDifficulty)42));
        }
    }
}
