namespace Monopoly
{
    public interface IMortgageBroker
    {
        IRealEstateAgent EstateAgent { get; set; }
        void Process(IPlayer player);
        bool IsPropertyMortgaged(IProperty property);
        void MortgageProperty(IPlayer player, IProperty property);
        void UnmortgageProperty(IPlayer player, IProperty property);
    }
}
