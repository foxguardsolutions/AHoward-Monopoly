namespace Monopoly
{
    public class Property : IProperty
    {
        public string Name { get; set; }
        public int Price { get; }
        public int Rent { get; }
        public int MapIndex { get; }

        public Property(string name = "", int price = 0, int rent = 0, int mapIndex = 0)
        {
            Name = name;
            Price = price;
            Rent = rent;
            MapIndex = mapIndex;
        }
    }
}