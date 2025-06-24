using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using PlayerRoles;
using RueI.Displays;
using RueI.Extensions;
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
            
            ServerSpecificSettingsSync.SendToAll();
            
            ServerSpecificSettingsSync.ServerOnSettingValueReceived += OnSettingValueReceived;
            
            Events.SendingProximityHint += OnSendingProximityHint;
            
            PlayerEvents.ChangedRole += OnChangedRole;
        }

        public override void Disable()
        {
            Instance = null;
            Events.SendingOtherMessage -= OnSendingMessage;
            Events.SentOtherMessage -= OnSentMessage;
            Events.SendingProximityHint -= OnSendingProximityHint;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= OnSettingValueReceived;
            PlayerEvents.ChangedRole -= OnChangedRole;
        }

        public override void LoadConfigs()
        {
            this.TryLoadConfig("translation.yml", out Translation translation);
            Translation = translation ?? new ();
            
            base.LoadConfigs();
        }

        private void OnSettingValueReceived(ReferenceHub hub, ServerSpecificSettingBase setting)
        {
            if (setting.SettingId != EnableDisableSetting.SettingId && setting.SettingId != TextSizeSetting.SettingId) return;
            
            Player player = Player.Get(hub);
            
            if (player == null || !CheckIfValidRole(player)) return;
                
            DisplayDataStore store = player.GetDataStore<DisplayDataStore>();
            store.Validate();
            store.Display.Update();
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

        private void OnChangedRole(PlayerChangedRoleEventArgs ev) => DisplayDataStore.UpdateAndValidateAll();

        public override string Name { get; } = "TextChat.RueI";
        public override string Description { get; } = "Adds global chat functionality using RueI for hints.";
        public override string Author { get; } = "LumiFae";
        public override Version Version { get; } = new (1, 0, 0);
        public override Version RequiredApiVersion { get; } = new(1, 0, 2);
    }
}