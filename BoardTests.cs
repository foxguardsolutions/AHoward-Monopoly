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
            Assert.AreEqual(40, board.PropertyCount);
        }

        [Test]
        public void PropertiesAreInitializedWithNamesInOrder()
        {
            Board board = new Board();
            for (int index = 0; index < board.PropertyCount; index++)
            {
                Assert.AreEqual(board.PropertyNames[index], board.GetPropertyName(index));
            }
        }
    }
}
