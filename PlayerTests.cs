using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Monopoly
{
    [TestFixture]
    public class PlayerTests
    {
        private Player _lastPlayerToEndTurn = null;

        [TestCase("Snoop Dog", Result = "Snoop Dog")]
        [TestCase("Player B", Result = "Player B")]
        public string PlayerHasAName(string inputName)
        {
            Player playa = new Player(inputName);
            return playa.Name;
        }

        [TestCase(0, Result = 0)]
        [TestCase(1, Result = 1)]
        [TestCase(25, Result = 25)]
        [TestCase(40, Result = 0)]
        [TestCase(45, Result = 5)]
        public int PlayerPositionProperlyWrapsAroundIndicesOfGameBoard(int position)
        {
            Player playa = new Player(position: position);
            return playa.Position;
        }

        [Test]
        public void RollDieReturnsANumberBetween1And6()
        {
            Player playa = new Player();
            List<int> rolls = new List<int>();
            for (int i = 0; i < 100; i++)
            {
                rolls.Add(playa.RollDie());
            }

            Assert.IsTrue(rolls.All(x => x <= 6 && x >= 1));
        }

        [Test]
        public void TakeTurnAdvancesPlayersPosition()
        {
            Player playa = new Player();
            Assert.AreEqual(0, playa.Position);
            playa.TakeTurn();
            Assert.AreNotEqual(0, playa.Position);
            Assert.IsTrue(playa.Position >= 2 && playa.Position <= 12);
        }

        public void OnTurnEnded(Player player)
        {
            _lastPlayerToEndTurn = player;
        }

        [Test]
        public void TakeTurnInvokesTurnEnded()
        {
            Player playa = new Player();
            playa.TurnEnded += OnTurnEnded;
            Assert.AreEqual(null, _lastPlayerToEndTurn);
            playa.TakeTurn();
            Assert.AreEqual(playa, _lastPlayerToEndTurn);
        }
    }
}
