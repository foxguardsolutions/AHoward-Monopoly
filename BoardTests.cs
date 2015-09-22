using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Monopoly
{
    [TestFixture]
    public class BoardTests
    {
        [Test]
        public void BoardInitializes40PropertiesUponInstantiation()
        {
            Board board = new Board();
            Assert.AreEqual(40, Board.PropertyCount);
        }

        [Test]
        public void PropertiesAreInitializedWithNamesInOrder()
        {
            Board board = new Board();
            for (int index = 0; index < Board.PropertyCount; index++)
            {
                Assert.AreEqual(Board.PropertyNames[index], board.GetPropertyName(index));
            }
        }
    }
}
