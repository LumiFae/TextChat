using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Loader;

namespace TextChat
{
    public sealed class Plugin : LabApi.Loader.Features.Plugins.Plugin
    {
        public static Plugin Instance { get; private set; }

        public Config Config { get; private set; }

        public Translation Translation { get; private set; }

        public override void Enable()
        {
            Instance = this;

            MessageChecker.Register();
            
            PlayerEvents.ChangingRole += OnChangingRole;
        }

        public override void Disable()
        {
            Instance = null!;

            MessageChecker.Unregister();
            
            PlayerEvents.ChangingRole -= OnChangingRole;
        }

        private void OnChangingRole(PlayerChangingRoleEventArgs ev)
        {
            if (!Component.CanSpawn(ev.NewRole) && Component.ContainsPlayer(ev.Player))
                Component.RemovePlayer(ev.Player);
        }

        public override void LoadConfigs()
        {
            this.TryLoadConfig("config.yml", out Config config);
            Config =
                config ??
                new Config();

            this.TryLoadConfig("translation.yml", out Translation translation);
            Translation =
                translation ??
                new Translation();

            base.LoadConfigs();
        }

        public override string Name { get; } = "TextChat";
        public override string Description { get; } = "Adds text chat functionality to global and proximity areas.";
        public override string Author { get; } = "LumiFae";
        public override Version Version { get; } = new(1, 2, 0);
        public override Version RequiredApiVersion { get; } = new(1, 0, 2);
    }
}