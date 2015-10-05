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
                "Go",
                "Connecticut Avenue",
                "Jail",
                "Go To Jail",
                "Pacific Avenue",
                "Income Tax",
                "Luxury Tax",
                "Boardwalk",
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
        public void AdvancePlayerMovesPlayerForward()
        {
            Player player = new Player(generator, gameBoard, "a");
            Assert.AreEqual(0, player.Position);
            game.AdvancePlayer(player);
            Assert.AreEqual(4, player.Position);
        }

        [Test]
        public void AdvancePlayerMovesPlayerToJailWhenLandingOnGoToJail()
        {
            Player player = new Player(generator, gameBoard, "a");
            player.Position = 7;
            game.AdvancePlayer(player);
            Assert.AreEqual(2, player.Position);
        }

        [TestCase(5)]
        [TestCase(4)]
        public void AdvancePlayerGivesPlayer200DollarsForPassingOrLandingOnGo(int start)
        {
            Player player = new Player(generator, gameBoard, "A");
            player.Position = start;
            Assert.AreEqual(0, player.Money);
            game.AdvancePlayer(player);
            Assert.AreEqual(200, player.Money);
        }
    }
}
