using System.ComponentModel;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using PlayerRoles;
using RueI.Displays;
using RueI.Extensions;
using RueI.Extensions.HintBuilding;
using TextChat.API.EventArgs;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace TextChat.RueI
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; private set; }

        internal SSTwoButtonsSetting EnableDisableSetting;

        internal SSSliderSetting TextSizeSetting;
        
        public Translation Translation { get; private set; }
        
        public override void Enable()
        {
            Instance = this;
            // handle global hints
            Events.SendingOtherMessage += OnSendingMessage;
            Events.SentOtherMessage += OnSentMessage;

            EnableDisableSetting = new(null, Translation.ShouldShowSpectatorSelect, Translation.Yes, Translation.No, hint:Translation.ShouldShowSpectatorSelectHint);
            TextSizeSetting = new(null, Translation.TextSizeSlider, Mathf.Min(Config!.FontSize, 18), Mathf.Max(Config!.FontSize, 38), Config!.FontSize, true, hint:Translation.TextSizeSliderHint);

            ServerSpecificSettingsSync.DefinedSettings =
            [
                ..ServerSpecificSettingsSync.DefinedSettings ?? [],
                new SSGroupHeader("TextChat Settings", hint: "This will only modify settings inside of SCP and Spectator chats."),
                EnableDisableSetting, TextSizeSetting
            ];
            
            Events.SendingProximityHint += OnSendingProximityHint;
            
            PlayerEvents.ChangingRole += OnChangingRole;
        }

        public override void Disable()
        {
            Instance = null;
            Events.SendingOtherMessage -= OnSendingMessage;
            Events.SentOtherMessage -= OnSentMessage;
            Events.SendingProximityHint -= OnSendingProximityHint;
            PlayerEvents.ChangingRole -= OnChangingRole;
        }

        public override void LoadConfigs()
        {
            this.TryLoadConfig("translation.yml", out Translation translation);
            Translation = translation ?? new ();
            
            base.LoadConfigs();
        }

        private void OnSendingProximityHint(SendingProximityHintEventArgs ev)
        {
            ev.IsAllowed = false;
            DisplayCore.Get(ev.Player.ReferenceHub).SetElemTemp(ev.HintContent, 300, TimeSpan.FromSeconds(TextChat.Plugin.Instance.Config.MessageExpireTime), new ());
        }

        private void OnSendingMessage(SendingOtherMessageEventArgs ev)
        {
            if (!CheckIfValidRole(ev.Player)) return;
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
            else if(ev.Player.IsSCP) HintManager.AddScpChatMessage(string.Format(Config!.Prefix, ev.Player.DisplayName) + ev.Text);
        }

        private bool CheckIfValidRole(Player player) => !player.IsAlive || player.IsSCP;

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

        [Description("The default size of the text, in pixels, also a server specific setting for per person.")]
        public int FontSize { get; set; } = 28;
        
        [Description("The alignment of the hint.")]
        public HintBuilding.AlignStyle Alignment { get; set; } = HintBuilding.AlignStyle.Left;

        [Description("The vertical position of the hint.")]
        public int VerticalPosition { get; set; } = 200;
    }

    public class Translation
    {
        public string ShouldShowSpectatorSelect { get; set; } = "Show Spectator Chat?";

        public string ShouldShowSpectatorSelectHint { get; set; } = "Whether to show the spectator chat.";
        
        public string Yes { get; set; } = "Yes";
        public string No { get; set; } = "No";

        public string TextSizeSlider { get; set; } = "Text Size";
        
        public string TextSizeSliderHint { get; set; } = "How large should the text size be in the spectator/SCP chats?";
    }
}