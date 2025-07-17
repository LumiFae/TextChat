using LabApi.Features.Wrappers;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using RueI;
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

            RueIMain.EnsureInit();
            
            EnableDisableSetting = new(null, Translation.ShouldShowSpectatorSelect, Translation.Yes, Translation.No,
                hint: Translation.ShouldShowSpectatorSelectHint);
            TextSizeSetting = new(null, Translation.TextSizeSlider, Mathf.Min(Config!.FontSize, 18), Mathf.Max(Config!.FontSize, 38),
                Config!.FontSize, true, hint: Translation.TextSizeSliderHint);

            ServerSpecificSettingsSync.DefinedSettings =
            [
                ..ServerSpecificSettingsSync.DefinedSettings ?? [],
                new SSGroupHeader(Translation.Header, hint: Translation.HeaderHint),
                EnableDisableSetting, TextSizeSetting
            ];

            ServerSpecificSettingsSync.SendToAll();

            Events.Register();
        }

        public override void Disable()
        {
            Events.Unregister();

            Instance = null;
        }

        public override void LoadConfigs()
        {
            this.TryLoadConfig("translation.yml", out Translation translation);
            Translation = translation ?? new();

            base.LoadConfigs();
        }

        public static bool CheckIfValidRole(Player player) => !player.IsAlive || player.IsSCP;

        public override string Name { get; } = "TextChat.RueI";
        public override string Description { get; } = "Adds global chat functionality using RueI for hints.";
        public override string Author { get; } = "LumiFae";
        public override Version Version { get; } = new(1, 1, 0);
        public override Version RequiredApiVersion { get; } = new(1, 0, 2);
    }
}