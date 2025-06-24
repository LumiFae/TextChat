using LabApi.Features.Wrappers;
using TextChat.API.EventArgs;

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
        /// Invoked when a message is invalid by it containing a banned word.
        /// </summary>
        public static event Action<SendingInvalidMessageEventArgs> SendingInvalidMessage;

        /// <summary>
        /// Invoked whenever a message is sent.
        /// </summary>
        public static event Action<SentMessageEventArgs> SentMessage;

        /// <summary>
        /// Invoked before sending a proximity message.
        /// </summary>
        public static event Action<SendingProximityMessageEventArgs> SendingProximityMessage;

        /// <summary>
        /// Invoked whenever a proximity message is sent.
        /// </summary>
        public static event Action<SentProximityMessageEventArgs> SentProximityMessage;

        /// <summary>
        /// Invoked before a current message hint is sent to the player, allows for overwriting the hint sending method.
        /// </summary>
        public static event Action<SendingProximityHintEventArgs> SendingProximityHint;

        /// <summary>
        /// Invoked when a chat message spawns above a players head.
        /// </summary>
        public static event Action<SpawnedProximityChatEventArgs> SpawnedProximityChat;

        /// <summary>
        /// Invoked before sending a message not controlled by this plugin.
        /// </summary>
        public static event Action<SendingOtherMessageEventArgs> SendingOtherMessage;

        /// <summary>
        /// Invoked whenever a person that doesn't have an allowed role sends a message.
        /// </summary>
        public static event Action<SentOtherMessageEventArgs> SentOtherMessage;

        public static string TrySendMessage(Player player, string text)
        {
            if (text.Length > Plugin.Instance.Config.MaxMessageLength)
                return Translation.ContentTooLong;

            SendingMessageEventArgs sendingMsgEventArgs = OnSendingMessage(player, text);

            if (sendingMsgEventArgs.Response != null)
                return sendingMsgEventArgs.Response;

            text = sendingMsgEventArgs.Text.Trim();

            // prevents people from putting their own styles into the text
            text = $"<noparse>{text.Replace("</noparse>", "")}</noparse>";

            if (!IsTextAllowed(text))
            {
                SendingInvalidMessage?.Invoke(new(player, text));
                return Translation.ContainsBadWord;
            }

            if (player.IsAlive && !player.IsSCP)
            {
                SendingProximityMessageEventArgs sendingProximityMessageEventArgs =
                    OnSendingProximityMessage(player, text);

                if (sendingProximityMessageEventArgs.Response != null)
                    return sendingProximityMessageEventArgs.Response;

                Component.TrySpawn(player, text);

                SentMessage?.Invoke(new(player, text));
                SentProximityMessage?.Invoke(new(player, text));

                return null;
            }

            if (SendingOtherMessage == null)
                return Translation.NotValidRole;

            SendingOtherMessageEventArgs sendingOtherMessageEventArgs = OnSendingOtherMessage(player, text);

            if (sendingOtherMessageEventArgs.Response != null)
                return sendingOtherMessageEventArgs.Response;

            SentMessage?.Invoke(new(player, text));
            SentOtherMessage?.Invoke(new(player, text));

            return null;
        }

        public static bool IsTextAllowed(string text)
        {
            string validationText = text.Replace(".", "").Replace(",", "").Replace("!", "").Replace("?", "");

            return !validationText.Split(' ').Any(word => Plugin.Instance.Config.BannedWords.Any(x => x == word));
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

        public static SendingOtherMessageEventArgs OnSendingOtherMessage(Player player, string text)
        {
            SendingOtherMessageEventArgs ev = new(player, text);
            SendingOtherMessage?.Invoke(ev);
            return ev;
        }

        public static SendingProximityHintEventArgs OnSendingProximityHint(Player player, string text, string hintContent)
        {
            SendingProximityHintEventArgs ev = new(player, text, hintContent);
            SendingProximityHint?.Invoke(ev);
            return ev;
        }

        public static SpawnedProximityChatEventArgs OnSpawnedProximityChat(Player player, string text)
        {
            SpawnedProximityChatEventArgs ev = new(player, text);
            SpawnedProximityChat?.Invoke(ev);
            return ev;
        }
    }
}