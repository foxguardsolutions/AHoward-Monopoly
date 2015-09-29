namespace Monopoly
{
    public class RandomGeneratorMoc : IRandomGenerator
    {
        public RandomGeneratorMoc()
        {
        }

        public int Next(int minValue, int maxValue)
        {
            return (minValue * 2) % (maxValue + 1);
        }
    }
}
