using Newtonsoft.Json;

namespace UnicornOutsourceApp
{
    public class Unicorn
    {
        public PartsOfBody PartsOfBody { get; set; } = new PartsOfBody();

        public int Strength { get; set; }
        public int Agility { get; set; }
        public int Speed { get; set; }
        public int Intelligance { get; set; }
        public int Charm { get; set; }

        [JsonIgnore]
        public int TournamentWins { get; set; }

        [JsonIgnore]
        public double SumBuf_Round { get; set; }

        [JsonIgnore]
        public double SumBuf_Match { get; set; }

        [JsonIgnore]
        public double SumBuf_Tournament { get; set; }
    }
}
