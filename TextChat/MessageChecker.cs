using System.Text.RegularExpressions;
using LabApi.Features.Console;

namespace TextChat
{
    public static class MessageChecker
    {
        private static Config Config => Plugin.Instance.Config;
        
        private static List<Regex> BannedWordRegex { get; set; }
        private static readonly Regex NoParseRegex = new("/noparse", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static List<Regex> BannedRegex { get; set; }

        public static void Register()
        {
            BannedWordRegex = Config.BannedWords.Select(matcher =>
            {
                Logger.Debug($"Creating a regex from {matcher}.", Config.Debug);
                matcher = Regex.Escape(matcher);
                matcher = matcher.Replace(@"\*", ".*");
                matcher = $"^{matcher}$";

                return new Regex(matcher, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }).ToList();

            BannedRegex = Config.BannedRegex.Select(regex => new Regex(regex, RegexOptions.Compiled)).ToList();
        }

        public static void Unregister()
        {
            BannedWordRegex.Clear();
            BannedWordRegex = null;
            
            BannedRegex.Clear();
            BannedRegex = null;
        }

        public static string NoParse(string text) =>
            $"<noparse>{NoParseRegex.Replace(text.Replace(@"\", @"\\"), "").Replace("<>", "")}</noparse>";
        
        public static bool IsTextAllowed(string text)
        {
            string validationText = text.Replace(".", "").Replace(",", "").Replace("?", "").Replace("'", "");
            
            if (validationText.Split(' ').Any(word => BannedWordRegex.Any(x => DoesWordMatch(word, x)))) return false;

            return !BannedRegex.Any(regex => regex.IsMatch(text));
        }
        
        public static bool DoesWordMatch(string word, Regex matcher) => matcher.IsMatch(word);
    }
}