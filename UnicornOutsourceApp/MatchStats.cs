namespace UnicornOutsourceApp
{
    public class RoundStats
    {
        public string Primary { get; set; }
        public string Secondary { get; set; }

        public RoundStats(string pr, string sec)
        {
            Primary = pr;
            Secondary = sec;
        }

        public RoundStats() { }

        public override bool Equals(object obj)
        {
            var anotherStat = obj as RoundStats;
            if (anotherStat == null)
                return base.Equals(obj);
            return Primary == anotherStat.Primary && Secondary == anotherStat.Secondary;

        }
    }
}