namespace Monopoly
{
    public interface IPlayer
    {
        string Name { get; }
        int Position { get; set; }
        int Money { get; set; }
        IBoard GameBoard { get; }
        IRandomGenerator Generator { get; }

        int RollDie();
        int RollBothDice();
    }
}
