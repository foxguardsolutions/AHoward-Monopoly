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

        [Test]
        public void RailRoadRentIsAFunctionOfRailRoadsOwned()
        {
            var playera = playerDeque.CurrentPlayer;
            playera.Money = 10000;
            playera.Position = 1;
            game.TakeTurn(playera);

            var playerb = playerDeque.CurrentPlayer;
            playerb.Money = 500;
            playerb.Position = 1;
            game.TakeTurn(playerb);
            Assert.AreEqual(500 - 25, playerb.Money);

            playera.Position = 11;
            game.TakeTurn(playera);
            playerb.Position = 11;
            playerb.Money = 500;
            game.TakeTurn(playerb);
            Assert.AreEqual(500 - 50, playerb.Money);

            playera.Position = 21;
            game.TakeTurn(playera);
            playerb.Position = 21;
            playerb.Money = 500;
            game.TakeTurn(playerb);
            Assert.AreEqual(500 - 100, playerb.Money);

            playera.Position = 31;
            game.TakeTurn(playera);
            playerb.Position = 31;
            playerb.Money = 500;
            game.TakeTurn(playerb);
            Assert.AreEqual(500 - 200, playerb.Money);
        }

        [Test]
        public void BeginTurnAttemptsToMortgageAPropertyIfPlayerHasLessThan100Dollars()
        {
            var player = playerDeque.CurrentPlayer;
            player.Money = 10;
            var property = gameBoard.GetPropertyFromName("Boardwalk");
            gameBoard.PlayerPurchasedProperty(player, property);
            property.Owner = player;
            game.BeginPlayerTurn(player);

            Assert.IsTrue(property.Mortgaged);
            Assert.AreEqual(310, player.Money);
        }

        [Test]
        public void EndTurnAttemptsToUnMortgageAllPropertiesIfPlayerHasEnoughMoney()
        {
            var player = playerDeque.CurrentPlayer;
            player.Money = 600;
            var boardwalk = gameBoard.GetPropertyFromName("Boardwalk");
            var parkPlace = gameBoard.GetPropertyFromName("Park Place");
            var railroad = gameBoard.GetPropertyFromName("Reading RailRoad");
            gameBoard.PlayerPurchasedProperty(player, boardwalk);
            gameBoard.PlayerPurchasedProperty(player, parkPlace);
            gameBoard.PlayerPurchasedProperty(player, railroad);
            boardwalk.Owner = player;
            parkPlace.Owner = player;
            railroad.Owner = player;
            boardwalk.Mortgaged = true;
            parkPlace.Mortgaged = true;
            railroad.Mortgaged = true;
            game.EndPlayerTurn(player);

            Assert.IsFalse(parkPlace.Mortgaged);
            Assert.IsTrue(boardwalk.Mortgaged);
            Assert.IsFalse(railroad.Mortgaged);
            Assert.AreEqual(50, player.Money);
        }
    }
}
