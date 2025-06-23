using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
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

        public Translation Translation;
        
        public override void Enable()
        {
            if(!string.IsNullOrEmpty(Config?.ProximityChatWebhook))
            {
                _proximityClient = new();
                _proximityClient.BaseAddress = new(Config.ProximityChatWebhook);
            }

            if (!string.IsNullOrEmpty(Config?.OtherChatWebhook))
            {
                _otherClient = new();
                _otherClient.BaseAddress = new(Config.OtherChatWebhook);
            }
            
            Events.SentMessage += OnSentMessage;
        }

        public override void Disable()
        {
            _proximityClient?.Dispose();
            _otherClient?.Dispose();
            
            Events.SentMessage -= OnSentMessage;
        }

        public override void LoadConfigs()
        {
            this.TryLoadConfig("translation.yml", out Translation translation);
            Translation = translation ?? new Translation();
            
            base.LoadConfigs();
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

            var data = new
            {
                content = string.Format(tuple.text, ev.Player.Nickname, ev.Player.UserId, ev.Text.Replace("<noparse>", "").Replace("</noparse>", ""))
            };

            StringContent content = new(JsonSerializer.Serialize(data));
            content.Headers.ContentType = new("application/json");
            
            tuple.client.PostAsync("", content);
        }

        public override string Name { get; } = "TextChat.Discord";
        public override string Description { get; } = "Discord webhook logging for TextChat";
        public override string Author { get; } = "LumiFae";
        public override Version Version { get; } = new(1, 0, 0);
        public override Version RequiredApiVersion { get; } = new(1, 0, 2);
    }

    public class Config
    {
        [Description("The webhook URL of your channel")]
        public string ProximityChatWebhook { get; set; }
        
        [Description("This webhook will only ever be triggered if you have a plugin that manages other chats, like TextChat.RueI.")]
        public string OtherChatWebhook { get; set; }
    }

    public class Translation
    {
        [Description("The message to send when a proximity message is sent. {0} is the nickname, {1} is the user's id and {2} is the text content sent.")]
        public string ProximityMessage { get; set; } = "Player `{0}` (`{1}`) has sent the message `{2}` in proximity chat.";

        [Description("")]
        public string OtherMessage { get; set; } = "Player `{0}` (`{1}`) has sent the message `{2}` in other chat.";
    }
}