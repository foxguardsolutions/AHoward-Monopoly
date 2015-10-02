using System.IO;
using Ninject.Modules;

namespace Monopoly
{
    public class ServiceModule : NinjectModule
    {
        private string[] _playerNames;
        private string[] _propertyNames =
        {
            "Go",
            "Mediterranean Avenue",
            "Community Chest",
            "Baltic Avenue",
            "Income Tax",
            "Reading RailRoad",
            "Oriental Avenue",
            "Chance",
            "Vermont Avenue",
            "Connecticut Avenue",
            "Jail",
            "St. Charles Place",
            "Electric Company",
            "States Avenue",
            "Virginia Avenue",
            "Pennsylvania Railroad",
            "St. James Place",
            "Community Chest",
            "Tennessee Avenue",
            "New York Avenue",
            "Free Parking",
            "Kentucky Avenue",
            "Chance",
            "Indiana Avenue",
            "Illinois Avenue",
            "B. & O. Railroad",
            "Atlantic Avenue",
            "Ventnor Avenue",
            "Water Works",
            "Marvin Gardens",
            "Go To Jail",
            "Pacific Avenue",
            "North Carolina Avenue",
            "Community Chest",
            "Pennsylvania Avenue",
            "Short Line Railroad",
            "Chance",
            "Park Place",
            "Luxury Tax",
            "Boardwalk",
        };

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
            Bind<IPlayerDeque>().To<PlayerDeque>();
            Bind<IGame>().To<Game>().InSingletonScope();
        }
    }
}
