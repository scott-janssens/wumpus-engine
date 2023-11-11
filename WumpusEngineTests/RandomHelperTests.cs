using System.Diagnostics.CodeAnalysis;
using WumpusEngine;

namespace WumpusEngineTests
{
    [ExcludeFromCodeCoverage]
    public class RandomHelperTests
    {
        [Test]
        public void RandomHelperCtorNoParms()
        {
            var ticks1 = DateTime.Now.Ticks;
            var actual = new RandomHelper();
            var ticks2 = DateTime.Now.Ticks;

            Assert.That(actual.Seed, Is.InRange((int)ticks1, (int)ticks2));
        }

        [Test]
        public void RandomHelperCtorSeed()
        {
            var seed = (int)DateTime.Now.Ticks;
            var actual = new RandomHelper(seed);

            Assert.That(actual.Seed, Is.EqualTo(seed));
        }

        [Test]
        public void RandomHelperNext()
        {
            var actual = new RandomHelper(42).Next(100);
            Assert.That(actual, Is.EqualTo(66));
        }

        [Test]
        public void RandomHelperSetSeed()
        {
            var random = new RandomHelper(42);
            var actual1 = random.Next(66);

            random.Seed = 23;
            var actual2 = random.Next(100);

            Assert.Multiple(() =>
            {
                Assert.That(actual1, Is.EqualTo(44));
                Assert.That(actual2, Is.EqualTo(74));
            });
        }
    }
}
