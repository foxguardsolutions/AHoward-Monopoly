using System.Security.Cryptography.X509Certificates;

namespace Monopoly
{
    public interface IPlayerDeque
    {
        int Count { get; }
        IPlayer PreviousPlayer { get; }
        void Shuffle();
        void TakeTurn();
    }
}
