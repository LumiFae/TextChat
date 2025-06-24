using LabApi.Features.Stores;
using LabApi.Features.Wrappers;
using PlayerRoles;
using PlayerRoles.Subroutines;
using RueI.Displays;

namespace TextChat.RueI
{
    internal sealed class DisplayDataStore : CustomDataStore
    {
        public DisplayDataStore(Player player) : base(player)
        {
            Player = player;
            Display = new(player.ReferenceHub);
            Validate();
        }

        public readonly Player Player;

        public readonly Display Display;

        public readonly AbilityCooldown Cooldown = new ();

        public void Validate()
        {
            Display.Elements.Clear();
            if(!Player.IsAlive) Display.Elements.Add(HintManager.SpectatorElement);
            if(Player.IsSCP) Display.Elements.Add(HintManager.ScpElement);
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