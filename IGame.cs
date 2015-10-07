namespace Monopoly
{
    public interface IGame
    {
        IQueue Players { get; set; }
        IBoard GameBoard { get; }
        CardDeck CommunityChest { set; }
        CardDeck Chance { set; }

        void Play();
    }
}
