using LabApi.Features.Wrappers;
using LabApi.Loader;

namespace TextChat
{
    public static class Events
    {
        private static Translation Translation => Plugin.Instance.Translation;
        
        /// <summary>
        /// Invoked whenever a message is sent.
        /// </summary>
        public static event Action<Player, string> SentMessage;
        
        /// <summary>
        /// Invoked whenever a proximity message is sent
        /// </summary>
        public static event Action<Player, string> SentProximityMessage;
        
        /// <summary>
        /// Invoked whenever a person in a global based voice channel sends a message, i.e. SCP or Spectator.
        /// </summary>
        public static event Action<Player, string> SentGlobalMessage;
        
        public static string TrySendMessage(Player player, string text)
        {
            if (text.Length > Plugin.Instance.Config.MaxMessageLength) 
                return Translation.ContentTooLong;
            
            // prevents people from putting their own styles into the text
            text = $"<noparse>{text.Replace("</noparse>", "")}</noparse>";

            string validationText = text.Replace(".", "").Replace(",", "").Replace("!", "").Replace("?", "");

            if (Plugin.Instance.Config.BannedWords.Any(bannedWord => validationText.Contains(bannedWord)))
            {
                return Translation.ContainsBadWord;
            }
            
            if (player.IsAlive && !player.IsSCP)
            {
                Component.TrySpawn(player, text);
                SentMessage?.Invoke(player, text);
                SentProximityMessage?.Invoke(player, text);
                return null;
            }

            if (!player.IsAlive || player.IsSCP)
            {
                if (PluginLoader.EnabledPlugins.FirstOrDefault(x => x.Name == "TextChat.RueI") == null) 
                    return Translation.NotValidRole;
                SentMessage?.Invoke(player, text);
                SentGlobalMessage?.Invoke(player, text);
                return null;
            }

            return Translation.NotValidRole;
        }
    }
}