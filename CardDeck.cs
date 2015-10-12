using System;
using System.Collections.Generic;

namespace Monopoly
{
    public class CardDeck : List<Card>
    {
        private readonly IRandomGenerator _generator;
        private List<Card> _removedCards = new List<Card>(); 

        public Card TopCard
        {
            get { return this[0]; }
        }

        public CardDeck(string deckData, IRandomGenerator generator) : base(DeckFactory.GenerateCards(deckData))
        {
            _generator = generator;
        }

        public void Shuffle()
        {
            for (int i = 0; i < Count * 3; i++)
            {
                var temp = this[_generator.Next(0, Count - 1)];
                Remove(temp);
                Add(temp);
            }
        }

        public void AdvanceDeck()
        {
            var topCard = TopCard;
            Remove(topCard);
            Add(topCard);
        }

        public void RemoveCard(Card card)
        {
            Remove(card);
            _removedCards.Add(card);
        }

        public void AddCard(Card card)
        {
            if (!_removedCards.Contains(card))
            {
                throw new ArgumentException("Card does not belong to this deck!");
            }

            _removedCards.Remove(card);
            Add(card);
        }

        public bool ContainsCard(Card card)
        {
            return Contains(card) || _removedCards.Contains(card);
        }
    }
}
