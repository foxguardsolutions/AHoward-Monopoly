namespace Monopoly
{
    public interface IJailer
    {
        ICardDealer Dealer { get; set; }
        void Process(IPlayer player);
        void SendPlayerToJail(IPlayer player);
        bool IsInJail(IPlayer player);
        void GivePlayerGetOutJailFreeCard(IPlayer player, Card card);
    }
}
