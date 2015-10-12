using Newtonsoft.Json;

namespace Monopoly
{
    public class DeckFactory
    {
        public static Card[] GenerateCards(string cardData)
        {
            return JsonConvert.DeserializeObject<Card[]>(cardData);
        }
    }
}
