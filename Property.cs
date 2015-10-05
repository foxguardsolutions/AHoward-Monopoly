namespace Monopoly
{
    public class Property : IProperty
    {
        public string Name { get; set; }

        public Property(string name = "")
        {
            Name = name;
        }
    }
}