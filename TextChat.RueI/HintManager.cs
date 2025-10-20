using System.Text;
using MEC;
using NorthwoodLib.Pools;
using RueI.API.Elements;
using RueI.API.Elements.Enums;
using RueI.Utils;
using UserSettings.ServerSpecific;

namespace TextChat.RueI
{
    public static class HintManager
    {
        private static Config Config => Plugin.Instance.Config!;

        private static readonly List<string> ActiveSpectatorMessages = [];
        private static readonly List<string> ActiveScpMessages = [];

        internal static readonly DynamicElement ScpElement = new(Config.VerticalPosition, ScpContent)
        {
            VerticalAlign = VerticalAlign.Up
        };
        
        internal static readonly DynamicElement SpectatorElement = new(Config.VerticalPosition, SpectatorContent)
        {
            VerticalAlign = VerticalAlign.Up
        };

        internal static void AddSpectatorChatMessage(string text)
        {
            ActiveSpectatorMessages.Add(text);
            DisplayDataStore.UpdateAndValidateSpectators();

            Timing.CallDelayed(Config.MessageExpireTime, () =>
            {
                ActiveSpectatorMessages.Remove(text);
                DisplayDataStore.UpdateAndValidateSpectators();
            });
        }

        internal static void AddScpChatMessage(string text)
        {
            ActiveScpMessages.Add(text);
            DisplayDataStore.UpdateAndValidateScps();

            Timing.CallDelayed(Config.MessageExpireTime, () =>
            {
                ActiveScpMessages.Remove(text);
                DisplayDataStore.UpdateAndValidateScps();
            });
        }

        private static string Content<T>(List<T> list, (SSTwoButtonsSetting, SSSliderSetting)? settings)
        {
            if (settings == null)
                return string.Empty;

            StringBuilder builder = StringBuilderPool.Shared.Rent();

            int fontSize = settings.Value.Item2.SyncIntValue;

            builder.SetAlignment(Config.Alignment);
            builder.SetSize(fontSize);

            builder.Append(string.Join("\n", list));

            builder.CloseSize();
            builder.CloseAlign();

            return StringBuilderPool.Shared.ToStringReturn(builder);
        }

        private static string ScpContent(ReferenceHub hub) => Content(ActiveScpMessages, GetSettingsFromPlayer(hub));

        private static string SpectatorContent(ReferenceHub hub)
        {
            (SSTwoButtonsSetting, SSSliderSetting)? settings = GetSettingsFromPlayer(hub);

            if (settings == null || settings.Value.Item1.SyncIsB)
                return string.Empty;

            return Content(ActiveSpectatorMessages, settings);
        }

        public static (SSTwoButtonsSetting, SSSliderSetting)? GetSettingsFromPlayer(ReferenceHub hub)
        {
            try
            {
                if (!ServerSpecificSettingsSync.TryGetSettingOfUser(hub, Plugin.Instance.EnableDisableSetting.SettingId,
                        out SSTwoButtonsSetting setting))
                    return null;

                if (!ServerSpecificSettingsSync.TryGetSettingOfUser(hub, Plugin.Instance.TextSizeSetting.SettingId, out SSSliderSetting slider))
                    return null;

                return (setting, slider);
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }
    }
}