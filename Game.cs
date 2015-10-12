namespace Monopoly
{
    public class Game : IGame
    {
        private const int _roundsToPlay = 20;

        public IBoard GameBoard { get; }
        public PlayerDeque Players { get; set; } = null;

        public IJailer Jailer { get; set; }
        public IMortgageBroker Broker { get; set; }
        public IRealEstateAgent Agent { get; set; }
        public ICardDealer Dealer { get; set; }
        public IBanker Banker { get; set; }
        
        public Game(IBoard gameBoard, IJailer jailer, IMortgageBroker broker,
                    IRealEstateAgent agent, ICardDealer dealer, IBanker banker)
        {
            GameBoard = gameBoard;
            Jailer = jailer;
            Broker = broker;
            Agent = agent;
            Dealer = dealer;
            Banker = banker;
        }

        public void Play()
        {
            int roundsPlayed = 0;
            while (roundsPlayed < _roundsToPlay)
            {
                PlayRound();
                roundsPlayed++;
            }
        }

        private void PlayRound()
        {
            for (int i = 0; i < Players.Count; i++)
            {
                BeginPlayerTurn(Players.CurrentPlayer);
                TakeTurn(Players.CurrentPlayer);
                EndPlayerTurn(Players.CurrentPlayer);
            }
        }

        public void TakeTurn(IPlayer player)
        {
            if (!Jailer.IsInJail(player))
            {
                Agent.Process(player);
                Jailer.Process(player);
                Dealer.Process(player);
                Banker.Process(player);
            }
        }

        public void BeginPlayerTurn(IPlayer player)
        {
            player.RollBothDice();
            Jailer.Process(player);

            if (!Jailer.IsInJail(player))
            {
                Broker.Process(player);
                var nextIndex = (player.Position + player.LastDiceRoll) % GameBoard.PropertyCount;
                var nextProperty = GameBoard.GetPropertyFromIndex(nextIndex);
                GameBoard.MovePlayerToProperty(player, nextProperty, true);
            }
        }

        public void EndPlayerTurn(IPlayer player)
        {
            if (!Jailer.IsInJail(player))
            {
                Broker.Process(player);
            }

            Players.AdvanceDeque();
        }
    }
}
