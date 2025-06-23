using System.ComponentModel;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Plugins;
using PlayerRoles;
using RueI.Extensions.HintBuilding;
using TextChat.API.EventArgs;

namespace TextChat.RueI
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; private set; }
        
        public override void Enable()
        {
            Instance = this;
            Events.SendingOtherMessage += OnSendingMessage;
            Events.SentOtherMessage += OnSentMessage;
            PlayerEvents.ChangingRole += OnChangingRole;
        }

        public override void Disable()
        {
            Instance = null;
            Events.SendingOtherMessage -= OnSendingMessage;
            Events.SentOtherMessage -= OnSentMessage;
            PlayerEvents.ChangingRole -= OnChangingRole;
        }

        private void OnSendingMessage(SendingOtherMessageEventArgs ev)
        {
            DisplayDataStore store = ev.Player.GetDataStore<DisplayDataStore>();
            if (!store.Cooldown.IsReady)
            {
                ev.Response = "You are sending too many messages!";
                return;
            }
            store.Cooldown.Trigger(Config!.MessageCooldown);
        }

        private void OnSentMessage(SentOtherMessageEventArgs ev)
        {
            if(ev.Player.Role == RoleTypeId.Spectator) HintManager.AddSpectatorChatMessage(string.Format(Config!.Prefix, ev.Player.DisplayName) + ev.Text);
            else if(ev.Player.Team == Team.SCPs) HintManager.AddScpChatMessage(string.Format(Config!.Prefix, ev.Player.DisplayName) + ev.Text);
        }

        private void OnChangingRole(PlayerChangingRoleEventArgs ev) => DisplayDataStore.UpdateAndValidateAll();

        public override string Name { get; } = "TextChat.RueI";
        public override string Description { get; } = "Adds global chat functionality using RueI for hints.";
        public override string Author { get; } = "LumiFae";
        public override Version Version { get; } = new (1, 0, 0);
        public override Version RequiredApiVersion { get; } = new(1, 0, 2);
    }
    
    public class Config
    {
        [Description("The amount of time til a message expires/deletes.")]
        public float MessageExpireTime { get; set; } = 10;

        [Description("The amount of time a user must wait before sending a new message.")]
        public float MessageCooldown { get; set; } = 2;
        
        [Description("The content that appears before a message, {0} is the player nickname")]
        public string Prefix { get; set; } = "<color=green>{0}: </color>";

        [Description("The size of the text, in pixels.")]
        public int FontSize { get; set; } = 28;
        
        [Description("The alignment of the hint.")]
        public HintBuilding.AlignStyle Alignment { get; set; } = HintBuilding.AlignStyle.Left;

        [Description("The vertical position of the hint.")]
        public int VerticalPosition { get; set; } = 200;
    }
}