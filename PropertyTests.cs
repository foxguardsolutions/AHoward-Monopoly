using NUnit.Framework;

namespace Monopoly
{
    [TestFixture]
    public class PropertyTests
    {
        [TestCase("", Result = "")]
        [TestCase("Boardwalk", Result = "Boardwalk")]
        [TestCase("Park Place", Result = "Park Place")]
        public string PropertyCanBeAssignedANameFromTheConstructor(string name)
        {
            Property boardPiece = new Property(name);
            return boardPiece.Name;
        }
    }
}
