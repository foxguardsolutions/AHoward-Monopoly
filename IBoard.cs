namespace Monopoly
{
    public interface IBoard
    {
        IPropertyGroup[] PropertyGroups { get; set; }
        int PropertyCount { get; }

        int GetPropertyPositionFromName(string name);
        IProperty GetPropertyFromIndex(int index);
        IProperty GetPropertyFromName(string name);
    }
}
