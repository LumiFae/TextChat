using LabApi.Features.Stores;
using LabApi.Features.Wrappers;
using PlayerRoles.Subroutines;
using RueI.API;
using RueI.API.Elements;

namespace TextChat.RueI
{
    public sealed class DisplayDataStore
    {
        private static Dictionary<Player, DisplayDataStore> _dictionary = new();

        public static DisplayDataStore Get(Player player) =>
            _dictionary.GetOrAdd(player, () => new DisplayDataStore(player));

        internal static void Destroy(Player player) => _dictionary.Remove(player);
        
        public static Tag Tag = new("TextChat Global Chat");

        private DisplayDataStore(Player player)
        {
            Owner = player;
        }
        
        public Player Owner { get; }

        public readonly AbilityCooldown Cooldown = new();

        public void Validate()
        {
            RueDisplay display = RueDisplay.Get(Owner);
            if (!Owner.IsAlive) display.Show(Tag, HintManager.SpectatorElement);
            else if (Owner.IsSCP) display.Show(Tag, HintManager.ScpElement);
            else display.Remove(Tag);
        }

        public static void UpdateAndValidateScps()
        {
            foreach (Player player in Player.ReadyList)
            {
                if (!player.IsSCP)
                    return;
                
                RueDisplay.Get(player).Update();
            }
        }

        public static void UpdateAndValidateSpectators()
        {
            foreach (Player player in Player.ReadyList)
            {
                if (player.IsAlive)
                    return;
                
                RueDisplay.Get(player).Update();
            }
        }
    }
}