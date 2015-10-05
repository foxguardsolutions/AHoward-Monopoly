namespace Monopoly
{
    public class Board : IBoard
    {
        public IProperty[] Properties { get; set; }
        public int PropertyCount
        {
            get { return Properties.Length; }
        }

        public Board(IPropertyFactory propertyFactory)
        {
            Properties = propertyFactory.GenerateProperties();
        }
    }
}