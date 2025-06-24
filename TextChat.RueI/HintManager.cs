using System.Text;
using MEC;
using NorthwoodLib.Pools;
using RueI.Displays;
using RueI.Elements;
using RueI.Extensions.HintBuilding;
using UserSettings.ServerSpecific;

namespace TextChat.RueI
{
    public static class HintManager
    {
        private static Config Config => Plugin.Instance.Config!;

        private static readonly List<string> ActiveSpectatorMessages = new();
        private static readonly List<string> ActiveScpMessages = new();

        internal static readonly DynamicElement ScpElement = new(ScpContent, Config.VerticalPosition);
        internal static readonly DynamicElement SpectatorElement = new(SpectatorContent, Config.VerticalPosition);

        internal static void AddSpectatorChatMessage(string text)
        {
            ActiveSpectatorMessages.Add(text);
            DisplayDataStore.UpdateAndValidateAll();

            Timing.CallDelayed(Config.MessageExpireTime, () =>
            {
                ActiveSpectatorMessages.Remove(text);
                DisplayDataStore.UpdateAndValidateAll();
            });
        }

        internal static void AddScpChatMessage(string text)
        {
            ActiveScpMessages.Add(text);
            DisplayDataStore.UpdateAndValidateAll();

            Timing.CallDelayed(Config.MessageExpireTime, () =>
            {
                ActiveScpMessages.Remove(text);
                DisplayDataStore.UpdateAndValidateAll();
            });
        }

        private static string Content<T>(List<T> list, Tuple<SSTwoButtonsSetting, SSSliderSetting> settings)
        {
            if (settings == null)
                return string.Empty;

            StringBuilder builder = StringBuilderPool.Shared.Rent();

            int fontSize = settings.Item2.SyncIntValue;

            builder.SetAlignment(Config.Alignment);
            builder.SetSize(fontSize);
            // 40.665 is RueI's default line height
            builder.AddVOffset((list.Count - 1) * 40.665f);

            builder.Append(string.Join("\n", list));

            builder.CloseVOffset();
            builder.CloseSize();
            builder.CloseAlign();

            return StringBuilderPool.Shared.ToStringReturn(builder);
        }

        private static string ScpContent(DisplayCore core) => Content(ActiveScpMessages, GetSettingsFromPlayer(core.Hub));

        private static string SpectatorContent(DisplayCore core)
        {
            Tuple<SSTwoButtonsSetting, SSSliderSetting> settings = GetSettingsFromPlayer(core.Hub);

            if (settings == null || settings.Item1.SyncIsB)
                return string.Empty;

            return Content(ActiveSpectatorMessages, settings);
        }

        public static Tuple<SSTwoButtonsSetting, SSSliderSetting> GetSettingsFromPlayer(ReferenceHub hub)
        {
            try
            {
                if (!ServerSpecificSettingsSync.TryGetSettingOfUser(hub, Plugin.Instance.EnableDisableSetting.SettingId,
                        out SSTwoButtonsSetting setting))
                    return null;

                if (!ServerSpecificSettingsSync.TryGetSettingOfUser(hub, Plugin.Instance.TextSizeSetting.SettingId, out SSSliderSetting slider))
                    return null;

                return new(setting, slider);
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }
    }
}