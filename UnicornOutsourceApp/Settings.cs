using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicornOutsourceApp
{
    class Settings
    {
        public bool LogRounds { get; set; }
        public bool LogMatches { get; set; }
        public double PrimaryModifier { get; set; } = 1.5;
        public double SecondaryModifier { get; set; } = 1;
        public int MatchesInTournament { get; set; } = 3;
        public int RoundsInMatch { get; set; } = 3;

        public int UncommonScore { get; set; }
        public int RareScore { get; set; }
        public int EpicScore { get; set; }
        public int LegendaryScore { get; set; }

        public double CritModifier { get; set; }
    }
}
