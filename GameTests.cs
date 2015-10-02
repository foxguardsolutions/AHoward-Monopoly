using System.IO;
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
            string data = File.ReadAllText("json\\propertyGroups.json");
            gameBoard = new Board(data);
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
            player.Position = 26;
            game.AdvancePlayer(player);
            Assert.AreEqual(10, player.Position);
        }

        [TestCase(38)]
        [TestCase(36)]
        public void AdvancePlayerGivesPlayer200DollarsForPassingOrLandingOnGo(int start)
        {
            Player player = new Player(generator, gameBoard, "A");
            player.Position = start;
            Assert.AreEqual(0, player.Money);
            game.AdvancePlayer(player);
            Assert.AreEqual(200, player.Money);
        }

        [TestCase(300, Result = true)]
        [TestCase(200, Result = false)]
        public bool PlayerBuysPropertyWhenAppropriateFundsAreAvailable(int fundsAvailable)
        {
            var player = playerDeque.CurrentPlayer;
            player.Money = fundsAvailable;
            player.Position = 2;
            game.AdvancePlayer(player);

            var property = gameBoard.GetPropertyFromIndex(player.Position);

            return player.Money == fundsAvailable - property.Price
                   && property.Owner == player;
        }

        [Test]
        public void UtilityRentIsAFunctionOfLastDiceRollAndUtilitiesOwned()
        {
            var playera = playerDeque.CurrentPlayer;
            playera.Money = 500;
            playera.Position = 8;
            game.TakeTurn(playera);

            var playerb = playerDeque.CurrentPlayer;
            playerb.Money = 500;
            playerb.Position = 8;
            game.TakeTurn(playerb);
            Assert.AreEqual(500 - (4 * 4), playerb.Money);

            playera.Position = 24;
            game.TakeTurn(playera);
            playerb.Position = 24;
            playerb.Money = 500;
            game.TakeTurn(playerb);
            Assert.AreEqual(500 - (10 * 4), playerb.Money);
        }
    }
}
