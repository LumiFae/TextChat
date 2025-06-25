using System.Text.RegularExpressions;
using LabApi.Features.Console;

namespace TextChat
{
    public static class MessageChecker
    {
        private static List<Regex> BannedWordRegex { get; set; }
        private static readonly Regex NoParseRegex = new("/noparse", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static void Register()
        {
            BannedWordRegex = Plugin.Instance.Config.BannedWords.Select(matcher =>
            {
                Logger.Debug($"Creating a regex from {matcher}.", Plugin.Instance.Config.Debug);
                matcher = Regex.Escape(matcher);
                matcher = matcher.Replace(@"\*", ".*");
                matcher = $"^{matcher}$";

                return new Regex(matcher, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }).ToList();
        }

        public static void Unregister()
        {
            BannedWordRegex.Clear();
            BannedWordRegex = null;
        }

        public static string NoParse(string text) =>
            $"<noparse>{NoParseRegex.Replace(text.Replace(@"\", @"\\"), "").Replace("<>", "")}</noparse>";
        
        public static bool IsTextAllowed(string text)
        {
            string validationText = text.Replace(".", "").Replace(",", "").Replace("!", "").Replace("?", "");
            
            return !validationText.Split(' ').Any(word => BannedWordRegex.Any(x => DoesWordMatch(word, x)));
        }
        
        public static bool DoesWordMatch(string word, Regex matcher) => matcher.IsMatch(word);
    }
}