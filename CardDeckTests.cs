using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Monopoly
{
    [TestFixture]
    public class CardDeckTests
    {
        private CardDeck deck;

        [SetUp]
        public void Setup()
        {
            string deckData = File.ReadAllText("json\\chanceCards.json");
            deck = new CardDeck(deckData, new AlternateRandomGeneratorMoc());
        }

        [Test]
        public void ShuffleMixesOrderOfCards()
        {
            var unshuffledDeck = new List<Card>(deck);
            Assert.IsTrue(deck.SequenceEqual(unshuffledDeck));
            deck.Shuffle();
            Assert.IsFalse(deck.SequenceEqual(unshuffledDeck));
        }

        [Test]
        public void AdvanceDeckMovesTopCardToTheBack()
        {
            var topCard = deck.TopCard;
            deck.AdvanceDeck();
            Assert.AreEqual(topCard, deck[deck.Count - 1]);
        }
    }
}
