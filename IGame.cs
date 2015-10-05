namespace Monopoly
{
    public interface IGame
    {
        IPlayerDeque Players { get; set; }
        IBoard GameBoard { get; }

        void Play();
    }
}
