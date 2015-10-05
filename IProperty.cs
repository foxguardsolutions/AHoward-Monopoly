namespace Monopoly
{
    public interface IProperty
    {
        string Name { get; set; }
        IPlayer Owner { get; set; }
        int Price { get; }
        int Rent { get; }
        bool Mortgaged { get; set; }
        string Action { get; }
        int MapIndex { get; }

        bool IsOwned();
    }
}
