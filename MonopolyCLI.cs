using System;
using System.IO;
using Ninject;

namespace Monopoly
{
    public class MonopolyCLI
    {
        private StandardKernel _kernel;
        private IGame game;

        public static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                PrintUsage();
                return 1;
            }

            var cli = new MonopolyCLI(args);
            cli.Play();

            return 0;
        }

        public MonopolyCLI(string[] playerNames)
        {
            _kernel = new StandardKernel(new ServiceModule(playerNames));
            game = _kernel.Get<Game>();
            HookUpMortgageBroker();
            HookUpGame();
            HookUpCardDealer();
        }

        public void Play()
        {
            game.Play();
        }

        public void HookUpMortgageBroker()
        {
            var broker = _kernel.Get<MortgageBroker>();
            var agent = _kernel.Get<RealEstateAgent>();
            broker.EstateAgent = agent;
            game.Broker = broker;
        }

        public void HookUpCardDealer()
        {
            var communityChest = new CardDeck(
                    File.ReadAllText("json\\communityChestCards.json"),
                    _kernel.Get<RandomGenerator>());
            var chance =
                new CardDeck(
                    File.ReadAllText("json\\chanceCards.json"),
                    _kernel.Get<RandomGenerator>());

            var dealer = _kernel.Get<CardDealer>();
            dealer.CommunityChestDeck = communityChest;
            dealer.ChanceDeck = chance;
            dealer.GameBoard = game.GameBoard;
            dealer.JailKeeper = game.Jailer;
            dealer.EstateAgent = game.Agent;
            dealer._players = game.Players;
            game.Dealer = dealer;
            game.Jailer.Dealer = dealer;
        }

        public void HookUpGame()
        {
            game.Players = _kernel.Get<PlayerDeque>();
        }

        public static void PrintUsage()
        {
            Console.WriteLine(
                "Usage: {0} <player 1> <player 2> [player 3] [player4] ...",
                AppDomain.CurrentDomain.FriendlyName);
        }
    }
}
