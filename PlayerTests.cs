using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Monopoly
{
    [TestFixture]
    public class PlayerTests
    {
        [TestCase("Snoop Dog", Result = "Snoop Dog")]
        [TestCase("Player B", Result = "Player B")]
        public string PlayerHasAName(string inputName)
        {
            Player playa = new Player(inputName);
            return playa.Name;
        }

        [TestCase(0, Result = 0)]
        [TestCase(1, Result = 1)]
        [TestCase(25, Result = 25)]
        [TestCase(40, Result = 0)]
        [TestCase(45, Result = 5)]
        public int PlayerPositionProperlyWrapsAroundIndicesOfGameBoard(int position)
        {
            Player playa = new Player(position: position);
            return playa.Position;
        }
    }
}
