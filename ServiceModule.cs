using System.IO;
using Ninject.Modules;

namespace Monopoly
{
    public class ServiceModule : NinjectModule
    {
        private string[] _playerNames;

        public ServiceModule(string[] playerNames)
        {
            _playerNames = playerNames;
        }

        public override void Load()
        {
            Bind<IRandomGenerator>().To<RandomGenerator>();
            Bind<IPropertyGroup>().To<PropertyGroup>();
            Bind<IBoard>().To<Board>()
                .WithConstructorArgument("propertyGroupData", File.ReadAllText("json\\propertyGroups.json"));
            Bind<IPlayerFactory>().To<PlayerFactory>()
                .WithConstructorArgument("names", _playerNames);
            Bind<IQueue>().To<PlayerDeque>();
            Bind<IGame>().To<Game>().InSingletonScope();
        }
    }
}
