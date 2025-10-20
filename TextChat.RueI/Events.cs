using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using RueI.API;
using RueI.API.Elements;
using TextChat.API.EventArgs;
using UserSettings.ServerSpecific;

namespace TextChat.RueI
{
    public static class Events
    {
        public static Config Config => Plugin.Instance.Config!;

        public static void Register()
        {
            // handle global hints
            TextChat.Events.SendingOtherMessage += OnSendingMessage;
            TextChat.Events.SentOtherMessage += OnSentMessage;

            TextChat.Events.SendingProximityHint += OnSendingProximityHint;

            ServerSpecificSettingsSync.ServerOnSettingValueReceived += OnSettingValueReceived;

            PlayerEvents.ChangedRole += OnChangedRole;

            PlayerEvents.Left += OnLeft;
        }

        public static void Unregister()
        {
            TextChat.Events.SendingOtherMessage -= OnSendingMessage;
            TextChat.Events.SentOtherMessage -= OnSentMessage;
            TextChat.Events.SendingProximityHint -= OnSendingProximityHint;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= OnSettingValueReceived;
            PlayerEvents.ChangedRole -= OnChangedRole;
            PlayerEvents.Left -= OnLeft;
        }


        private static void OnSettingValueReceived(ReferenceHub hub, ServerSpecificSettingBase setting)
        {
            if (setting.SettingId != Plugin.Instance.EnableDisableSetting.SettingId &&
                setting.SettingId != Plugin.Instance.TextSizeSetting.SettingId) return;

            Player player = Player.Get(hub);

            if (player == null || !Plugin.CheckIfValidRole(player)) return;

            DisplayDataStore store = DisplayDataStore.Get(player);
            store.Validate();
        }

        private static void OnSendingProximityHint(SendingProximityHintEventArgs ev)
        {
            ev.IsAllowed = false;
            RueDisplay.Get(ev.Player.ReferenceHub).Show(new BasicElement(300, ev.HintContent),
                TimeSpan.FromSeconds(TextChat.Plugin.Instance.Config.MessageExpireTime));
        }

        private static void OnSendingMessage(SendingOtherMessageEventArgs ev)
        {
            if (!Plugin.CheckIfValidRole(ev.Player)) return;
            DisplayDataStore store = DisplayDataStore.Get(ev.Player);
            if (!store.Cooldown.IsReady)
            {
                ev.Response = "You are sending too many messages!";
                return;
            }

            store.Cooldown.Trigger(Config.MessageCooldown);
        }

        private static void OnSentMessage(SentOtherMessageEventArgs ev)
        {
            if (!ev.Player.IsAlive) HintManager.AddSpectatorChatMessage(string.Format(Config.Prefix, ev.Player.DisplayName) + ev.Text);
            else if (ev.Player.IsSCP) HintManager.AddScpChatMessage(string.Format(Config.Prefix, ev.Player.DisplayName) + ev.Text);
        }

        private static void OnChangedRole(PlayerChangedRoleEventArgs ev)
        {
            if (!ev.Player.IsReady)
                return;
            
            DisplayDataStore.Get(ev.Player).Validate();
        }

        private static void OnLeft(PlayerLeftEventArgs ev) => DisplayDataStore.Destroy(ev.Player);
    }
}