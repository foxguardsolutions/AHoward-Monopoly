namespace Monopoly
{
    public interface IPropertyGroup
    {
        IProperty[] Properties { get; set; }
        IPlayer[] Owners { get; set; }

        bool AllPropertiesOwned();
        bool HasSingleOwner();
        IProperty GetPropertyFromPropertyIndex(int index);
        IProperty GetPropertyFromName(string name);
        void AddOwner(IPlayer player);
        IProperty[] GetPropertiesInGroupOwnedByPlayer(IPlayer player);
        int GetNumberOfPropertiesInGroupOwnedByPlayer(IPlayer player);
    }
}
