namespace Monopoly
{
    public interface IBoard
    {
        IPropertyGroup[] PropertyGroups { get; set; }
        int PropertyCount { get; }

        int GetPropertyPositionFromName(string name);
        IProperty GetPropertyFromIndex(int index);
        IProperty GetPropertyFromName(string name);
        void PlayerPurchasedProperty(IPlayer player, IProperty property);
        int CalculateRent(IProperty property);
        IProperty[] GetAllPropertiesOwnedByPlayer(IPlayer player);
        IPropertyGroup GetGroupFromProperty(IProperty property);
    }
}
