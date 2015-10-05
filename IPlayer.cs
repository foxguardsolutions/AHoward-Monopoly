namespace Monopoly
{
    public interface IPlayer
    {
        string Name { get; }
        int Position { get; set; }
        IBoard GameBoard { get; }
        IRandomGenerator Generator { get; }

        int RollDie();
        void TakeTurn();
    }
}
