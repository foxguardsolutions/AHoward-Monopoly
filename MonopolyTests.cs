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
        public void CreatingAMonopolyCreatesPlayersWithRandomOrder()
        {
            string[] playerNames = new string[] { "Player A", "Player B", "Player C", "Player D" };

            List<string[]> results = new List<string[]>();
            for (int i = 0; i < 100; i++)
            {
                Monopoly game = new Monopoly(playerNames);
                string[] newOrder = Enumerable.Range(0, 4).Select(x => game.GetPlayer(x).Name).ToArray();
                results.Add(newOrder);
            }

            Assert.IsFalse(results.All(x => x.SequenceEqual(playerNames)));
        }

        [Test]
        public void WhenAPlayerEndsHisTurnPlayerTurnIndexIncreases()
        {
            string[] playerNames = new string[] { "player a", "player b" };
            Monopoly game = new Monopoly(playerNames);
            Assert.AreEqual(0, game.PlayerTurnIndex);
            game.GetPlayer(0).TakeTurn();
            Assert.AreEqual(1, game.PlayerTurnIndex);
            game.GetPlayer(1).TakeTurn();
            Assert.AreEqual(0, game.PlayerTurnIndex);
        }

        [Test]
        public void WhenAllPlayersHaveTakenATurnCompletedRoundsIsIncreased()
        {
            string[] playerNames = new string[] { "player a", "player b" };
            Monopoly game = new Monopoly(playerNames);
            Assert.AreEqual(0, game.CompletedRounds);
            game.GetPlayer(0).TakeTurn();
            game.GetPlayer(1).TakeTurn();
            Assert.AreEqual(1, game.CompletedRounds);
        }
    }
}
