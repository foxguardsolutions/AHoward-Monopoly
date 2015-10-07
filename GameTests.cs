using System;
using System.IO;
using System.Runtime.Remoting.Messaging;
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
        private CardDeck communityChest, chance;

        [SetUp]
        public void Setup()
        {
            generator = new RandomGeneratorMoc();
            string propertyData = File.ReadAllText("json\\propertyGroups.json");
            gameBoard = new Board(propertyData);
            string[] players =
            {
                "a", "b", "c", "d", "e"
            };

            playerDeque = new PlayerDeque(generator, new PlayerFactory(players, generator, gameBoard));

            string communityChestData = File.ReadAllText("json\\communityChestCards.json");
            string chanceData = File.ReadAllText("json\\chanceCards.json");
            communityChest = new CardDeck(communityChestData, generator);
            chance = new CardDeck(chanceData, generator);

            game = new Game(gameBoard, communityChest, chance);
            game.Players = playerDeque;
        }

        [Test]
        public void AdvancePlayerMovesPlayerForward()
        {
            Player player = new Player(generator, gameBoard, "a");
            Assert.AreEqual(0, player.Position);
            player.RollBothDice();
            game.AdvancePlayer(player);
            Assert.AreEqual(4, player.Position);
        }

        [Test]
        public void AdvancePlayerMovesPlayerToJailWhenLandingOnGoToJail()
        {
            Player player = new Player(generator, gameBoard, "a");
            player.Position = 26;
            player.RollBothDice();
            game.AdvancePlayer(player);
            Assert.AreEqual(10, player.Position);
        }

        // In this case the player buys the property he lands on
        [TestCase(39, Result = 140)]
        [TestCase(36, Result = 200)]
        public int AdvancePlayerGivesPlayer200DollarsForPassingOrLandingOnGo(int start)
        {
            Player player = new Player(generator, gameBoard, "A");
            player.Position = start;
            Assert.AreEqual(0, player.Money);
            player.RollBothDice();
            game.AdvancePlayer(player);
            return player.Money;
        }

        [TestCase(300, Result = true)]
        [TestCase(200, Result = false)]
        public bool PlayerBuysPropertyWhenAppropriateFundsAreAvailable(int fundsAvailable)
        {
            var player = playerDeque.CurrentPlayer;
            player.Money = fundsAvailable;
            player.Position = 2;
            player.RollBothDice();
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

            // Reset consecutive doubles so they don't go to jail
            playera.ConsecutiveDoublesRolled = 0;
            playerb.ConsecutiveDoublesRolled = 0;

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

            // Reset consecutive doubles so they don't go to jail
            playera.ConsecutiveDoublesRolled = 0;
            playerb.ConsecutiveDoublesRolled = 0;

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

        [TestCase(1, Result = false)]
        [TestCase(2, Result = false)]
        [TestCase(3, Result = true)]
        public bool RollingDoublesThreeConsecutiveTimesLandsPlayerInJail(int numberOfTurns)
        {
            var player = playerDeque.CurrentPlayer;
            for (int i = 0; i < numberOfTurns; i++)
            {
                game.TakeTurn(player);
            }

            return player.IsInJail;
        }

        [Test]
        public void RollingDoublesWhileInJailReleasesPlayerFromJail()
        {
            var player = playerDeque.CurrentPlayer;
            player.IsInJail = true;
            game.TakeTurn(player);
            Assert.IsFalse(player.IsInJail);
        }

        [Test]
        public void RollingDoublesWhileInJailAdvancesPlayerOnBoardAfterRelease()
        {
            var player = playerDeque.CurrentPlayer;
            player.IsInJail = true;
            player.Position = 10;
            game.TakeTurn(player);
            Assert.IsFalse(player.IsInJail);
            Assert.AreEqual(14, player.Position);
        }

        private void NonDoubleSetup()
        {
            generator = new AlternateRandomGeneratorMoc();
            string data = File.ReadAllText("json\\propertyGroups.json");
            gameBoard = new Board(data);
            string[] players =
            {
                "a", "b", "c", "d", "e"
            };

            playerDeque = new PlayerDeque(generator, new PlayerFactory(players, generator, gameBoard));
            game = new Game(gameBoard, communityChest, chance);
            game.Players = playerDeque;
        }

        [TestCase(100, Result = true)]
        [TestCase(1100, Result = false)]
        public bool PlayerBuysOutOfJailWhenHeHasEnoughMoney(int playerMoney)
        {
            NonDoubleSetup();
            var player = playerDeque.CurrentPlayer;
            player.IsInJail = true;
            player.Money = playerMoney;
            game.BeginPlayerTurn(player);
            return player.IsInJail;
        }

        [TestCase(0, Result = true)]
        [TestCase(1, Result = true)]
        [TestCase(2, Result = true)]
        [TestCase(3, Result = false)]
        public bool PlayerIsForcedToBuyOutOfJailOnThirdTurn(int consecutiveJailTurns)
        {
            NonDoubleSetup();
            var player = playerDeque.CurrentPlayer;
            player.IsInJail = true;
            player.ConsecutiveTurnsInJail = consecutiveJailTurns;
            game.BeginPlayerTurn(player);
            return player.IsInJail;
        }

        private void AdvanceDeckToCard(CardDeck deck, string cardName)
        {
            while (deck.TopCard.Name != cardName)
            {
                deck.AdvanceDeck();
            }
        }

        [Test]
        public void DrawingChanceCardGoToJailSendsPlayerToJail()
        {
            AdvanceDeckToCard(chance, "Go To Jail");
            var player = playerDeque.CurrentPlayer;
            player.Position = 3;
            Assert.IsFalse(player.IsInJail);
            game.TakeTurn(player);
            Assert.IsTrue(player.IsInJail);

            var jailIndex = gameBoard.GetPropertyPositionFromName("Jail");
            Assert.AreEqual(jailIndex, player.Position);
        }

        [Test]
        public void DrawingCommunityChestCardGoToJailSendsPlayerToJail()
        {
            AdvanceDeckToCard(communityChest, "Go To Jail");
            var player = playerDeque.CurrentPlayer;
            player.Position = 38;
            Assert.IsFalse(player.IsInJail);
            game.TakeTurn(player);
            Assert.IsTrue(player.IsInJail);

            var jailIndex = gameBoard.GetPropertyPositionFromName("Jail");
            Assert.AreEqual(jailIndex, player.Position);
        }

        [TestCase("Advance to Go", Result = 0)]
        [TestCase("Advance to Illinois Ave", Result = 24)]
        [TestCase("Advance to St. Charles Place", Result = 11)]
        [TestCase("Take a walk on the boardwalk", Result = 39)]
        public int DrawingChanceCardAdvanceToPropertySendsPlayerToProperty(string cardName)
        {
            AdvanceDeckToCard(chance, cardName);
            var player = playerDeque.CurrentPlayer;
            player.Position = 3;
            game.TakeTurn(player);
            return player.Position;
        }

        [Test]
        public void DrawingCommunityChestCardAdvanceToGoSendsPlayerToGoAndAwards200Dollars()
        {
            AdvanceDeckToCard(communityChest, "Advance to Go");
            var player = playerDeque.CurrentPlayer;
            player.Position = 29;
            player.Money = 0;
            game.TakeTurn(player);
            Assert.AreEqual(0, player.Position);
            Assert.AreEqual(200, player.Money);
        }

        [TestCase("Advance to Nearest Utility", 3, Result = 12)]
        [TestCase("Advance to Nearest Utility", 18, Result = 28)]
        [TestCase("Advance to Nearest RailRoad", 3, Result = 15)]
        [TestCase("Advance to Nearest RailRoad", 18, Result = 25)]
        [TestCase("Advance to Nearest RailRoad", 32, Result = 5)]
        public int DrawingChanceCardMoveToNearestPropertyTypeMovesPlayerToTheNearestOfThatGroup(string cardName, int startPosition)
        {
            AdvanceDeckToCard(chance, cardName);
            var player = playerDeque.CurrentPlayer;
            player.Position = startPosition;
            game.TakeTurn(player);
            return player.Position;
        }

        [Test]
        public void
            WhenUtilityIsOwnedAndPlayerDrawsAdvanceToNearestUtilityChanceCardPlayerPaysTenTimesAmountShownOnDice()
        {
            AdvanceDeckToCard(chance, "Advance to Nearest Utility");
            var player = playerDeque.CurrentPlayer;
            player.Position = 8;
            player.Money = 350;
            game.TakeTurn(player);
            Assert.AreEqual(200, player.Money);

            player = playerDeque.CurrentPlayer;
            player.Money = 40;
            player.Position = 3;
            game.TakeTurn(player);

            Assert.AreEqual(0, player.Money);
        }

        [Test]
        public void
            WhenRailRoadIsOwnedAndPlayerDrawsAdvanceToNearestRailRoadChanceCardPlayerPaysTwiceAmountOfRent()
        {
            AdvanceDeckToCard(chance, "Advance to Nearest RailRoad");
            var player = playerDeque.CurrentPlayer;
            player.Position = 11;
            player.Money = 350;
            game.TakeTurn(player);
            Assert.AreEqual(150, player.Money);

            player = playerDeque.CurrentPlayer;
            player.Money = 50;
            player.Position = 3;
            game.TakeTurn(player);

            Assert.AreEqual(0, player.Money);
        }

        [TestCase("Bank pays you $50", Result = 50)]
        [TestCase("Your building and loan matures", Result = 150)]
        [TestCase("You won a crossword Competition", Result = 100)]
        [TestCase("Pay Poor Tax", Result = -15)]
        public int BankPaysOrTakesAmountShownOnCardWhenDrawingMoneyAlteringChanceCard(string cardName)
        {
            AdvanceDeckToCard(chance, cardName);
            var player = playerDeque.CurrentPlayer;
            player.Position = 3;
            game.TakeTurn(player);
            return player.Money;
        }

        [Test]
        public void MovePlayerBackSpacesMovesPlayerBackNumberOfSpacesIndicatedOnCard()
        {
            AdvanceDeckToCard(chance, "Go Back 3 Spaces");
            var player = playerDeque.CurrentPlayer;
            player.Position = 3;
            game.TakeTurn(player);
            Assert.AreEqual(4, player.Position);
        }

        [TestCase("Elected chairman of the board", 50)]
        public void PlayerPaysAllOtherPlayersAmountShownOnCardForSomeCards(string cardName, int AmountPaid)
        {
            AdvanceDeckToCard(chance, cardName);
            var player = playerDeque.CurrentPlayer;
            player.Position = 3;
            game.TakeTurn(player);

            foreach (var otherplayer in playerDeque)
            {
                if (otherplayer != player)
                {
                    Assert.AreEqual(AmountPaid, otherplayer.Money);
                }
            }

            Assert.AreEqual(AmountPaid * (playerDeque.Count - 1) * -1, player.Money);
        }

        [TestCase("Grand Opera Night", 50)]
        [TestCase("It's your birthday!", 10)]
        public void PlayerCollectsFromAllOtherPlayersAmountShownOnCardForSomeCards(string cardName, int AmountCollected)
        {
            AdvanceDeckToCard(communityChest, cardName);
            var player = playerDeque.CurrentPlayer;
            player.Position = 29;
            game.TakeTurn(player);

            foreach (var otherplayer in playerDeque)
            {
                if (otherplayer != player)
                {
                    Assert.AreEqual(-1 * AmountCollected, otherplayer.Money);
                }
            }

            Assert.AreEqual(AmountCollected * (playerDeque.Count - 1), player.Money);
        }

        [Test]
        public void GettingAGetOutOfJailFreeCardRemovesItFromTheDeck()
        {
            AdvanceDeckToCard(chance, "Get out of Jail Free");
            var card = chance.TopCard;
            var player = playerDeque.CurrentPlayer;
            player.Position = 3;
            game.TakeTurn(player);

            Assert.AreEqual(card, player.GetOutOfJailFreeCards[0]);
            Assert.IsFalse(chance.Contains(card));
        }

        [Test]
        public void UsingAGetOutOfJailFreeCardAddsItBackToTheDeck()
        {
            NonDoubleSetup();
            AdvanceDeckToCard(chance, "Get out of Jail Free");
            var card = chance.TopCard;
            var player = playerDeque.CurrentPlayer;
            player.Position = 0;
            game.TakeTurn(player);

            player.Position = 23;
            game.TakeTurn(player);
            Assert.IsTrue(player.IsInJail);
            game.TakeTurn(player);

            Assert.IsFalse(player.IsInJail);
            Assert.AreEqual(0, player.GetOutOfJailFreeCards.Count);
            Assert.IsTrue(chance.Contains(card));
        }
    }
}
