namespace SeaSlugAPI
{
    public class SeaSlugDictionary
    {
        public Dictionary<int, string> Data { get; private set; }

        public SeaSlugDictionary()
        {
            Data = new Dictionary<int, string>();

            InitializeDictionary();
        }

        public void InitializeDictionary()
        {
            Data.Add(0, "JanolusCristatus");
            Data.Add(1, "JanolusHyalinus");
            Data.Add(2, "AeolidiellaGlauca");
            Data.Add(3, "AeolidiaPapillosa");
            Data.Add(4, "AeolidiaPapillosaComplex");
            Data.Add(5, "AeolidiaFilomenae");
        }
    }
}
