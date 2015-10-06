namespace Monopoly
{
    public interface IPlayer
    {
        string Name { get; }
        int Position { get; set; }
        int Money { get; set; }
        IBoard GameBoard { get; }
        IRandomGenerator Generator { get; }
        int LastDiceRoll { get; set; }
        int ConsecutiveDoublesRolled { get; set; }
        int ConsecutiveTurnsInJail { get; set; }
        bool IsInJail { get; set; }

        int RollDie();
        int RollBothDice();
        void ReleaseFromJail();
    }
}
