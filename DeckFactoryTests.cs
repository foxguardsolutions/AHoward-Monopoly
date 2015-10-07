using System.IO;
using NUnit.Framework;

namespace Monopoly
{
    [TestFixture]
    public class DeckFactoryTests
    {
        [TestCase("json\\chanceCards.json", Result = 15)]
        [TestCase("json\\communityChestCards.json", Result = 17)]
        public int GenerateCardsReturnsArrayOfCardsFromData(string fileName)
        {
            string data = File.ReadAllText(fileName);
            Card[] cards = DeckFactory.GenerateCards(data);
            return cards.Length;
        }
    }
}
