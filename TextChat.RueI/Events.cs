using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using RueI.Displays;
using RueI.Extensions;
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
        }

        public static void Unregister()
        {
            TextChat.Events.SendingOtherMessage -= OnSendingMessage;
            TextChat.Events.SentOtherMessage -= OnSentMessage;
            TextChat.Events.SendingProximityHint -= OnSendingProximityHint;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= OnSettingValueReceived;
            PlayerEvents.ChangedRole -= OnChangedRole;
        }


        private static void OnSettingValueReceived(ReferenceHub hub, ServerSpecificSettingBase setting)
        {
            if (setting.SettingId != Plugin.Instance.EnableDisableSetting.SettingId &&
                setting.SettingId != Plugin.Instance.TextSizeSetting.SettingId) return;

            Player player = Player.Get(hub);

            if (player == null || !Plugin.CheckIfValidRole(player)) return;

            DisplayDataStore store = player.GetDataStore<DisplayDataStore>();
            store.Validate();
            store.Display.Update();
        }

        private static void OnSendingProximityHint(SendingProximityHintEventArgs ev)
        {
            ev.IsAllowed = false;
            DisplayCore.Get(ev.Player.ReferenceHub).SetElemTemp(ev.HintContent, 300,
                TimeSpan.FromSeconds(TextChat.Plugin.Instance.Config.MessageExpireTime), new());
        }

        private static void OnSendingMessage(SendingOtherMessageEventArgs ev)
        {
            if (!Plugin.CheckIfValidRole(ev.Player)) return;
            DisplayDataStore store = ev.Player.GetDataStore<DisplayDataStore>();
            if (!store.Cooldown.IsReady)
            {
                ev.Response = "You are sending too many messages!";
                return;
            }

            store.Cooldown.Trigger(Config!.MessageCooldown);
        }

        private static void OnSentMessage(SentOtherMessageEventArgs ev)
        {
            if (!ev.Player.IsAlive) HintManager.AddSpectatorChatMessage(string.Format(Config!.Prefix, ev.Player.DisplayName) + ev.Text);
            else if (ev.Player.IsSCP) HintManager.AddScpChatMessage(string.Format(Config!.Prefix, ev.Player.DisplayName) + ev.Text);
        }

        private static void OnChangedRole(PlayerChangedRoleEventArgs ev) => DisplayDataStore.UpdateAndValidateAll();
    }
}