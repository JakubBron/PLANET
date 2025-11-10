using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace game.Helpers
{
    public class GameRulesValidator : ValidationRule
    {
        private static readonly Regex pattern = new Regex(@"^B[1-9]+/S[1-9]+$");

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string input = (value ?? "").ToString().Trim();

            if (string.IsNullOrEmpty(input))
                return new ValidationResult(false, "Rules cannot be empty");

            if (!pattern.IsMatch(input))
                return new ValidationResult(false, "Invalid format. Use B<number>/S<number>, digits cannot be 0");

            return ValidationResult.ValidResult;
        }
    }
}
