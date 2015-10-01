using NUnit.Framework;

namespace Monopoly
{
    [TestFixture]
    public class GameTests
    {
        private IBoard gameBoard;
        private IRandomGenerator generator;
        private PlayerDeque playerDeque;
        private Game game;

        [SetUp]
        public void Setup()
        {
            generator = new RandomGeneratorMoc();
            string[] properties =
            {
                "a", "b", "c"
            };

            gameBoard = new Board(new PropertyFactory(properties));
            string[] players =
            {
                "a", "b", "c", "d", "e"
            };

            playerDeque = new PlayerDeque(generator, new PlayerFactory(players, generator, gameBoard));
            game = new Game(gameBoard);
            game.Players = playerDeque;
        }

        [Test]
        public void PlayAdvancesAllPlayers20RoundsWorthOfPlay()
        {
            game.Play();
            foreach (var player in playerDeque)
            {
                // each die comes up a 2, and we do that 20 times, and there are 3 properties
                Assert.AreEqual((2 * 2 * 20) % 3, player.Position);
            }
        }
    }
}
