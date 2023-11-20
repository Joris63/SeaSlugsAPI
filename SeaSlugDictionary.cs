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
            Data.Add(0, "Aeolidia Filomenae");
            Data.Add(1, "Aeolidia Papillosa");
            Data.Add(2, "Aeolidia Papillosa Complex");
            Data.Add(3, "Aeolidiella Glauca");
            Data.Add(4, "Janolus Cristatus");
            Data.Add(5, "Janolus Hyalinus");
        }
    }
}