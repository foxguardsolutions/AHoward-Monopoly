namespace Monopoly
{
    public interface ICardDealer
    {
        IBoard GameBoard { get; set; }
        IRealEstateAgent EstateAgent { get; set; }
        IJailer JailKeeper { get; set; }
        CardDeck ChanceDeck { get; set; }
        CardDeck CommunityChestDeck { get; set; }
        PlayerDeque _players { get; set; }

        void Process(IPlayer player);
        void PutCardBackInDeck(Card card);
    }
}
