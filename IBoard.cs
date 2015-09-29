namespace Monopoly
{
    public interface IBoard
    {
        IProperty[] Properties { get; set; }
        int PropertyCount { get; }
    }
}
