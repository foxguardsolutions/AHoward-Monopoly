﻿using System;
using Ninject;

namespace Monopoly
{
    public class MonopolyCLI
    {
        public static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                PrintUsage();
                return 1;
            }

            StandardKernel kernel = new StandardKernel(new ServiceModule(args));
            IGame game = kernel.Get<Game>();
            game.Players = kernel.Get<PlayerDeque>();
            game.Play();

            return 0;
        }

        public static void PrintUsage()
        {
            Console.WriteLine(
                "Usage: {0} <player 1> <player 2> [player 3] [player4] ...",
                AppDomain.CurrentDomain.FriendlyName);
        }
    }
}
