namespace Monopoly
{
    public interface IQueue
    {
        int Count { get; }
        IPlayer CurrentPlayer { get; }
        void Shuffle();
        void AdvanceDeque();
    }
}
