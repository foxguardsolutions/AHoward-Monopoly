namespace Monopoly
{
    public class AlternateRandomGeneratorMoc : IRandomGenerator
    {
        private bool _returnLow = false;

        public AlternateRandomGeneratorMoc()
        {
        }

        public int Next(int minValue, int maxValue)
        {
            _returnLow = !_returnLow;
            return (_returnLow) ? minValue : maxValue;
        }
    }
}
