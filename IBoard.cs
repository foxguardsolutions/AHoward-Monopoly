namespace Monopoly
{
    public interface IBoard
    {
        IPropertyGroup[] PropertyGroups { get; set; }
        int PropertyCount { get; set; }

        int GetPropertyPositionFromName(string name);
        IProperty GetPropertyFromIndex(int index);
        IProperty GetPropertyFromName(string name);
        IPropertyGroup GetGroupFromProperty(IProperty property);
        void MovePlayerToProperty(IPlayer player, IProperty property, bool canCollectGo = true);
    }
}
