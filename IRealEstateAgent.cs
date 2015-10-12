using System.Collections.Generic;

namespace Monopoly
{
    public interface IRealEstateAgent
    {
        void Process(IPlayer player, int modifier = 1);
        void PayRent(IPlayer player, IProperty property, int rentModifier = 1);
        void PlayerBoughtProperty(IPlayer player, IProperty property);
        bool PropertyIsOwned(IProperty property);
        IPlayer GetPropertyOwner(IProperty property);
        List<IProperty> GetAllPropertiesOwnedByPlayer(IPlayer player);
    }
}
