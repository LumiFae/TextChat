using System.ComponentModel;
using System.Net.Http;
using System.Text;
using LabApi.Features.Wrappers;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using TextChat.API.Enums;
using TextChat.API.EventArgs;

namespace TextChat.Discord
{
    public class Plugin : Plugin<Config>
    {
        private HttpClient _proximityClient;
        private HttpClient _otherClient;
        private HttpClient _invalidClient;

        public Translation Translation;

        public override void Enable()
        {
            if (!string.IsNullOrEmpty(Config?.ProximityChatWebhook))
            {
                _proximityClient = new();
                _proximityClient.BaseAddress = new(Config.ProximityChatWebhook);
            }

            if (!string.IsNullOrEmpty(Config?.OtherChatWebhook))
            {
                _otherClient = new();
                _otherClient.BaseAddress = new(Config.OtherChatWebhook);
            }

            if (!string.IsNullOrEmpty(Config?.InvalidMessageWebhook))
            {
                _invalidClient = new();
                _invalidClient.BaseAddress = new(Config.InvalidMessageWebhook);
            }

            Events.SentMessage += OnSentMessage;

            if (_invalidClient is not null)
                Events.SendingInvalidMessage += OnSendingInvalidMessage;
        }

        public override void Disable()
        {
            _proximityClient?.Dispose();
            _otherClient?.Dispose();

            Events.SentMessage -= OnSentMessage;

            if (_invalidClient is not null)
                Events.SendingInvalidMessage -= OnSendingInvalidMessage;
        }

        public override void LoadConfigs()
        {
            this.TryLoadConfig("translation.yml", out Translation translation);
            Translation = translation ?? new Translation();

            base.LoadConfigs();
        }

        private void OnSendingInvalidMessage(SendingInvalidMessageEventArgs ev)
        {
            SendMessage(_invalidClient, Translation.InvalidMessage, ev.Player, ev.Text);
        }

        private void OnSentMessage(SentMessageEventArgs ev)
        {
            (HttpClient client, string text) tuple = ev.Type switch
            {
                MessageType.Proximity => (_proximityClient, Translation.ProximityMessage),
                MessageType.Other => (_otherClient, Translation.OtherMessage),
                _ => (null, null)
            };

            if (tuple.client == null) return;

            SendMessage(tuple.client, tuple.text, ev.Player, ev.Text);
        }

        private void SendMessage(HttpClient client, string toFormat, Player player, string text)
        {
            string contentValue = string.Format(toFormat, player.Nickname, player.UserId, text.Replace("<noparse>", "").Replace("</noparse>", ""));
            string escapedContent = contentValue.Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
            
            string jsonString = $"{{\"content\":\"{escapedContent}\"}}";
            
            StringContent content = new (jsonString, Encoding.UTF8, "application/json");

            client.PostAsync("", content);
        }

        public override string Name { get; } = "TextChat.Discord";
        public override string Description { get; } = "Discord webhook logging for TextChat";
        public override string Author { get; } = "LumiFae";
        public override Version Version { get; } = new(1, 1, 1);
        public override Version RequiredApiVersion { get; } = new(1, 0, 2);
    }

    public class Config
    {
        [Description("The webhook URL of your channel")]
        public string ProximityChatWebhook { get; set; }

        [Description("This webhook will only ever be triggered if you have a plugin that manages other chats, like TextChat.RueI.")]
        public string OtherChatWebhook { get; set; }

        [Description("This webhook will be the location messages are sent that are blocked by the banned word list.")]
        public string InvalidMessageWebhook { get; set; }
    }

    public class Translation
    {
        [Description(
            "The message to send when a proximity message is sent. {0} is the nickname, {1} is the user's id and {2} is the text content sent.")]
        public string ProximityMessage { get; set; } = "Player `{0}` (`{1}`) has sent the message `{2}` in proximity chat.";

        [Description(
            "The message to send when a message is sent that is external to proximity messages, will only trigger if you have a plugin to manage this.")]
        public string OtherMessage { get; set; } = "Player `{0}` (`{1}`) has sent the message `{2}` in other chat.";

        [Description("The message to send when an invalid message is caught, {3} is the type of chat.")]
        public string InvalidMessage { get; set; } =
            "Player `{0}` (`{1}`) tried to send the message `{2}` but failed because it got blocked by the banned word list.";
    }
}