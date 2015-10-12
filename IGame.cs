namespace Monopoly
{
    public interface IGame
    {
        PlayerDeque Players { get; set; }
        IBoard GameBoard { get; }
        IJailer Jailer { get; set; }
        IMortgageBroker Broker { get; set; }
        IRealEstateAgent Agent { get; set; }
        ICardDealer Dealer { get; set; }
        IBanker Banker { get; set; }

        void Play();
        void TakeTurn(IPlayer player);
    }
}
