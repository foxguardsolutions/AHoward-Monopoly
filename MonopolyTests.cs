using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Monopoly
{
    [TestFixture]
    public class MonopolyTests
    {
        [Test]
        public void RollDieReturnsANumberBetween1And6()
        {
            Monopoly game = new Monopoly();
            List<int> rolls = new List<int>();
            for (int i = 0; i < 100; i++)
            {
                rolls.Add(game.RollDie());
            }

            Assert.IsTrue(rolls.All(x => x <= 6 && x >= 1));
        }
    }
}
