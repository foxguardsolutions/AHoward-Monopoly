using System;
using System.Collections.Generic;
using System.Linq;

namespace Monopoly
{
    public class CardDealer : ICardDealer
    {
        private readonly Dictionary<string, Action<IPlayer, Card>> actionCards;
        public IBoard GameBoard { get; set; }
        public IRealEstateAgent EstateAgent { get; set; }
        public IJailer JailKeeper { get; set; }
        public CardDeck ChanceDeck { get; set; }
        public CardDeck CommunityChestDeck { get; set; }
        public PlayerDeque _players { get; set; }

        public CardDealer()
        {
            actionCards = new Dictionary<string, Action<IPlayer, Card>>()
            {
                { "Move To Property", MovePlayerToPropertyIndicatedByCard },
                { "Move To Nearest Property", MovePlayerToNearestProperty },
                { "Bank pays player", BankPaysPlayer },
                { "Player pays bank", PlayerPaysBank },
                { "Send Player To Jail", SendPlayerToJail },
                { "Pay other players", PlayerPaysAllOtherPlayers },
                { "Collect from other players", OtherPlayersPayPlayer },
                { "Move Player Back Spaces", MovePlayerBackSpaces },
                { "Player gets Get out of Jail Free card", AwardGetOutOfJailFreeCard }
            };
        }

        
        public void Process(IPlayer player)
        {
            var property = GameBoard.GetPropertyFromIndex(player.Position);
            var propertyGroup = GameBoard.GetGroupFromProperty(property);
            if (propertyGroup.Name == "Card Group")
            {
                DrawCard(player, property);
            }
        }

        public void PutCardBackInDeck(Card card)
        {
            if (ChanceDeck.ContainsCard(card))
            {
                ChanceDeck.AddCard(card);
            }
            else
            {
                CommunityChestDeck.AddCard(card);
            }
        }

        private void DrawCard(IPlayer player, IProperty property)
        {
            var deck = (property.Name == "Chance") ? ChanceDeck : CommunityChestDeck;
            var card = deck.TopCard;

            Console.WriteLine("{0} drew card: {1}", player.Name, card.Name);

            if (actionCards.ContainsKey(card.Action))
            {
                actionCards[card.Action](player, card);
            }

            if (deck.TopCard == card)
            {
                deck.AdvanceDeck();
            }
        }

        private void MovePlayerToPropertyIndicatedByCard(IPlayer player, Card card)
        {
            var property = GameBoard.GetPropertyFromName(card.AssociatedProperty);
            GameBoard.MovePlayerToProperty(player, property);
            EstateAgent.Process(player, card.Modifier);
        }

        private void MovePlayerToNearestProperty(IPlayer player, Card card)
        {
            var property = GameBoard.GetPropertyFromName(card.AssociatedProperty);
            var group = GameBoard.GetGroupFromProperty(property);
            var moveTo = FindClosestPropertyInGroupToPlayer(group, player);
            GameBoard.MovePlayerToProperty(player, moveTo);
            EstateAgent.Process(player, card.Modifier);
        }

        private IProperty FindClosestPropertyInGroupToPlayer(IPropertyGroup group, IPlayer player)
        {
            var property = group.Properties[0];
            foreach (var p in group.Properties)
            {
                property = (FindDistanceForward(player, p)) < FindDistanceForward(player, property) ? p : property;
            }

            return property;
        }

        private int FindDistanceForward(IPlayer player, IProperty targetProperty)
        {
            return (targetProperty.MapIndex - player.Position + GameBoard.PropertyCount) % GameBoard.PropertyCount;
        }

        private void BankPaysPlayer(IPlayer player, Card card)
        {
            Console.WriteLine("Player {0} paid ${1} for drawing card: {2}", player.Name, card.Amount, card.Name);
            player.Money += card.Amount;
        }

        private void PlayerPaysBank(IPlayer player, Card card)
        {
            Console.WriteLine("Player {0} pays bank ${1} for drawing card: {2}", player.Name, card.Amount, card.Name);
            player.Money -= card.Amount;
        }

        private void SendPlayerToJail(IPlayer player, Card card)
        {
            JailKeeper.SendPlayerToJail(player);
        }

        private void PlayerPaysAllOtherPlayers(IPlayer player, Card card)
        {
            var otherPlayers = _players.ToList().Where(x => x != player).ToList();
            foreach (var otherPlayer in otherPlayers)
            {
                player.Money -= card.Amount;
                otherPlayer.Money += card.Amount;
                Console.WriteLine("{0} paid {1} ${2}", player.Name, otherPlayer.Name, card.Amount);
            }
        }

        private void OtherPlayersPayPlayer(IPlayer player, Card card)
        {
            var otherPlayers = _players.ToList().Where(x => x != player).ToList();
            foreach (var otherPlayer in otherPlayers)
            {
                player.Money += card.Amount;
                otherPlayer.Money -= card.Amount;
                Console.WriteLine("{1} paid {0} ${2}", player.Name, otherPlayer.Name, card.Amount);
            }
        }

        private void MovePlayerBackSpaces(IPlayer player, Card card)
        {
            var index = (GameBoard.PropertyCount + player.Position - card.Amount) % GameBoard.PropertyCount;
            var property = GameBoard.GetPropertyFromIndex(index);
            GameBoard.MovePlayerToProperty(player, property, false);
        }

        private void AwardGetOutOfJailFreeCard(IPlayer player, Card card)
        {
            JailKeeper.GivePlayerGetOutJailFreeCard(player, card);
            if (ChanceDeck.ContainsCard(card))
            {
                ChanceDeck.RemoveCard(card);
            }
            else
            {
                CommunityChestDeck.RemoveCard(card);
            }
        }
    }
}
