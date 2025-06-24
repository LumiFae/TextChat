using LabApi.Features.Stores;
using LabApi.Features.Wrappers;
using PlayerRoles.Subroutines;
using RueI.Displays;

namespace TextChat.RueI
{
    internal sealed class DisplayDataStore : CustomDataStore
    {
        public DisplayDataStore(Player player) : base(player)
        {
            Display = new(player.ReferenceHub);
            Validate();
        }

        public readonly Display Display;

        public readonly AbilityCooldown Cooldown = new();

        public void Validate()
        {
            Display.Elements.Clear();
            if (!Owner.IsAlive) Display.Elements.Add(HintManager.SpectatorElement);
            if (Owner.IsSCP) Display.Elements.Add(HintManager.ScpElement);
        }

        public static void UpdateAndValidateAll()
        {
            foreach (Player player in Player.ReadyList)
            {
                DisplayDataStore store = player.GetDataStore<DisplayDataStore>();
                store.Validate();
                store.Display.Update();
            }
        }
    }
}