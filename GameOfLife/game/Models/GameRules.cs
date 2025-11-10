using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace game.Models
{
    public class GameRules
    {
        public HashSet<int> Birth { get; set; } = new HashSet<int>();
        public HashSet<int> Survival { get; set; } = new HashSet<int>();

        public GameRules(bool defaultRules = true)
        {
            if (defaultRules)
            {
                Birth.Add(2);
                Survival.Add(2);
                Survival.Add(3);
            }
        }

        public static GameRules FromString(string s)
        {
            var rules = new GameRules(false);
            var parts = s.Split('/');
            foreach (var p in parts)
            {
                if (!IsValidString(s))
                    throw new ArgumentException($"Invalid rules string: {s}. Must match B<digits>/S<digits> with digits 1-9.");

                if (p.StartsWith("B"))
                    foreach (var c in p.Substring(1)) rules.Birth.Add(c - '0');
                else if (p.StartsWith("S"))
                    foreach (var c in p.Substring(1)) rules.Survival.Add(c - '0');
            }
                return rules;
        }

        public static bool IsValidString(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return false;

            var pattern = @"^B[1-9]+/S[1-9]+$";
            return Regex.IsMatch(s.Trim(), pattern);
        }

        public override string ToString()
        {
            return $"B{string.Concat(Birth)}/S{string.Concat(Survival)}";
        }
    }
}