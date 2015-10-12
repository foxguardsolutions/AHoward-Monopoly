using System.IO;
using Ninject.Modules;

namespace Monopoly
{
    public class ServiceModule : NinjectModule
    {
        private string[] _playerNames;
        private const string propertyFile = "json\\propertyGroups.json";

        public ServiceModule(string[] playerNames)
        {
            _playerNames = playerNames;
        }

        public override void Load()
        {
            Bind<IRandomGenerator>().To<RandomGenerator>();
            Bind<IProperty>().To<Property>();
            Bind<IPropertyGroup>().To<PropertyGroup>();
            Bind<IBoard>().To<Board>().InSingletonScope()
                .WithConstructorArgument("propertyGroupData", File.ReadAllText(propertyFile));
            Bind<IPlayerFactory>().To<PlayerFactory>()
                .WithConstructorArgument("names", _playerNames);
            Bind<IJailer>().To<Jailer>().InSingletonScope();
            Bind<IBanker>().To<Banker>().InSingletonScope();
            Bind<ICardDealer>().To<CardDealer>().InSingletonScope();
            Bind<IMortgageBroker>().To<MortgageBroker>().InSingletonScope();
            Bind<IRealEstateAgent>().To<RealEstateAgent>().InSingletonScope();
            Bind<IGame>().To<Game>().InSingletonScope();
        }
    }
}
