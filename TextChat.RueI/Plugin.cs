using System.ComponentModel;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Plugins;
using PlayerRoles;
using RueI.Extensions.HintBuilding;

namespace TextChat.RueI
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; private set; }
        
        public override void Enable()
        {
            Instance = this;
            Events.SentGlobalMessage += OnSentGlobalMessage;
            PlayerEvents.ChangingRole += OnChangingRole;
        }

        public override void Disable()
        {
            Instance = null;
            Events.SentGlobalMessage -= OnSentGlobalMessage;
            PlayerEvents.ChangingRole -= OnChangingRole;
        }

        private void OnChangingRole(PlayerChangingRoleEventArgs ev) => DisplayDataStore.UpdateAndValidateAll();

        private void OnSentGlobalMessage(Player player, string text)
        {
            if(player.Role == RoleTypeId.Spectator) HintManager.AddSpectatorChatMessage(string.Format(Config!.Prefix, player.DisplayName) + text);
            else if(player.Team == Team.SCPs) HintManager.AddScpChatMessage(string.Format(Config!.Prefix, player.DisplayName) + text);
        }

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