using System;

namespace Monopoly
{
    public class RandomGenerator : IRandomGenerator
    {
        private readonly Random _generator;

        public RandomGenerator()
        {
            _generator = new Random();
        }

        public int Next(int minValue, int maxValue)
        {
            return _generator.Next(minValue, maxValue + 1);
        }
    }
}
