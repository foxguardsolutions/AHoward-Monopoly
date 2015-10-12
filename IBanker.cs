namespace Monopoly
{
    public interface IBanker
    {
        void Process(IPlayer player);
        void PayPlayer(IPlayer player, int amount);
        void CollectFromPlayer(IPlayer player, int amount);
    }
}
