using System.Security.Cryptography.X509Certificates;

namespace Monopoly
{
    public interface IPlayerDeque
    {
        int Count { get; }
        IPlayer CurrentPlayer { get; }
        void Shuffle();
        void AdvanceDeque();
    }
}
