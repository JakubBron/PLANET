using System.Collections.Generic;

namespace game.Models
{
    public class GameRules
    {
        public HashSet<int> Birth { get; set; } = new HashSet<int>();
        public HashSet<int> Survival { get; set; } = new HashSet<int>();

        public GameRules()
        {
            Birth.Add(2);
            Survival.Add(2);
            Survival.Add(3);
        }

        public static GameRules FromString(string s)
        {
            var rules = new GameRules();
            var parts = s.Split('/');
            foreach (var p in parts)
            {
                if (p.StartsWith("B"))
                    foreach (var c in p.Substring(1)) rules.Birth.Add(c - '0');
                else if (p.StartsWith("S"))
                    foreach (var c in p.Substring(1)) rules.Survival.Add(c - '0');
            }
            return rules;
        }

        public override string ToString()
        {
            return $"B{string.Concat(Birth)}/S{string.Concat(Survival)}";
        }
    }
}