using System.Linq;
using NUnit.Framework;

namespace Monopoly
{
    [TestFixture]
    public class PlayerDequeTests
    {
        private IBoard gameBoard;
        private IRandomGenerator generator;
        private PlayerFactory playerFactory;
        private string[] players;

        [SetUp]
        public void Setup()
        {
            generator = new RandomGeneratorMoc();
            string[] properties =
            {
                "a", "b", "c"
            };

            gameBoard = new Board(new PropertyFactory(properties));
            players = new string[]
            {
                "a", "b", "c", "d"
            };

            playerFactory = new PlayerFactory(players, generator, gameBoard);
        }

        [Test]
        public void ShuffleReordersPlayers()
        {
            // Unfortunately the shuffled version with this generator is the same :(
            string[] shuffledPlayers = new string[]
            {
                players[0],
                players[1],
                players[2],
                players[3],
            };

            PlayerDeque deque = new PlayerDeque(generator, playerFactory);
            Assert.IsTrue(shuffledPlayers.SequenceEqual(deque.Select(x => x.Name).ToArray()));
        }

        [Test]
        public void AdvanceMovesFirstPlayerToLastPosition()
        {
            PlayerDeque deque = new PlayerDeque(generator, playerFactory);
            var first = deque[0];
            deque.AdvanceDeque();
            Assert.AreEqual(first, deque[deque.Count - 1]);
        }

        [Test]
        public void TakeTurnAdvancesPlayerAndPushesHimToBackOfDeque()
        {
            PlayerDeque deque = new PlayerDeque(generator, playerFactory);
            var first = deque[0];
            var position = first.Position;
            deque.TakeTurn();
            Assert.AreNotEqual(position, first.Position);
            Assert.AreEqual(first, deque[deque.Count - 1]);
        }
    }
}
