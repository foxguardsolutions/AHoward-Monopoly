namespace Monopoly
{
    public class Card
    {
        public string Name { get; set; }
        public string Action { get; set; }
        public string AssociatedProperty { get; set; }
        public int Amount { get; set; }
        public int Modifier { get; set; } = 1;
    }
}
