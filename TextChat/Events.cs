using LabApi.Features.Wrappers;
using LabApi.Loader;
using TextChat.EventArgs;

namespace TextChat
{
    public static class Events
    {
        private static Translation Translation => Plugin.Instance.Translation;

        /// <summary>
        /// Invoked before a message is sent.
        /// </summary>
        public static event Action<SendingMessageEventArgs> SendingMessage;
        
        /// <summary>
        /// Invoked whenever a message is sent.
        /// </summary>
        public static event Action<Player, string> SentMessage;

        /// <summary>
        /// Invoked before sending a proximity message.
        /// </summary>
        public static event Action<SendingProximityMessageEventArgs> SendingProximityMessage;
        
        /// <summary>
        /// Invoked whenever a proximity message is sent.
        /// </summary>
        public static event Action<Player, string> SentProximityMessage;

        /// <summary>
        /// Invoked before sending a global message.
        /// </summary>
        public static event Action<SendingGlobalMessageEventArgs> SendingGlobalMessage;
        
        /// <summary>
        /// Invoked whenever a person in a global based voice channel sends a message, i.e. SCP or Spectator.
        /// </summary>
        public static event Action<Player, string> SentGlobalMessage;
        
        public static string TrySendMessage(Player player, string text)
        {
            if (text.Length > Plugin.Instance.Config.MaxMessageLength) 
                return Translation.ContentTooLong;

            SendingMessageEventArgs sendingMsgEventArgs = OnSendingMessage(player, text);

            if (sendingMsgEventArgs.Response != null) 
                return sendingMsgEventArgs.Response;
            
            text = sendingMsgEventArgs.Text;
            
            // prevents people from putting their own styles into the text
            text = $"<noparse>{text.Replace("</noparse>", "")}</noparse>";

            string validationText = text.Replace(".", "").Replace(",", "").Replace("!", "").Replace("?", "");

            if (validationText.Split(' ').Any(word => Plugin.Instance.Config.BannedWords.Any(x => x == word)))
                return Translation.ContainsBadWord;
            
            if (player.IsAlive && !player.IsSCP)
            {
                SendingProximityMessageEventArgs sendingProximityMessageEventArgs =
                    OnSendingProximityMessage(player, text);
                
                if (sendingProximityMessageEventArgs.Response != null) 
                    return sendingProximityMessageEventArgs.Response;
                
                Component.TrySpawn(player, text);
                SentMessage?.Invoke(player, text);
                SentProximityMessage?.Invoke(player, text);
                return null;
            }

            if (!player.IsAlive || player.IsSCP)
            {
                if (SendingGlobalMessage == null) 
                    return Translation.NotValidRole;

                SendingGlobalMessageEventArgs sendingGlobalMessageEventArgs = OnSendingGlobalMessage(player, text);

                if (sendingGlobalMessageEventArgs.Response != null) 
                    return sendingGlobalMessageEventArgs.Response;
                
                SentMessage?.Invoke(player, text);
                SentGlobalMessage?.Invoke(player, text);
                return null;
            }

            return Translation.NotValidRole;
        }

        public static SendingMessageEventArgs OnSendingMessage(Player player, string text)
        {
            SendingMessageEventArgs ev = new(player, text);
            SendingMessage?.Invoke(ev);
            return ev;
        }

        public static SendingProximityMessageEventArgs OnSendingProximityMessage(Player player, string text)
        {
            SendingProximityMessageEventArgs ev = new(player, text);
            SendingProximityMessage?.Invoke(ev);
            return ev;
        }

        public static SendingGlobalMessageEventArgs OnSendingGlobalMessage(Player player, string text)
        {
            SendingGlobalMessageEventArgs ev = new(player, text);
            SendingGlobalMessage?.Invoke(ev);
            return ev;
        }
    }
}