namespace Monopoly
{
    public interface IProperty
    {
        string Name { get; set; }
        int Price { get; }
        int Rent { get; }
        int MapIndex { get; }
    }
}
