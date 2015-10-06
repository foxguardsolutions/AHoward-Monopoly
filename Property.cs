namespace Monopoly
{
    public class Property : IProperty
    {
        public string Name { get; set; }
        public IPlayer Owner { get; set; }
        public int Price { get; }
        public int Rent { get; }
        public bool Mortgaged { get; set; }
        public string Action { get; }
        public int MapIndex { get; }

        public Property(string name = "", int price = 0, int rent = 0, string action = "", int mapIndex = 0)
        {
            Name = name;
            Price = price;
            Rent = rent;
            Action = action;
            MapIndex = mapIndex;
            Owner = null;
        }

        public bool IsOwned()
        {
            return Owner != null;
        }
    }
}