using System.Drawing;
using System.Text;
using LabApi.Features.Stores;
using LabApi.Features.Wrappers;
using MEC;
using NorthwoodLib.Pools;
using PlayerRoles;
using RueI.Displays;
using RueI.Elements;
using RueI.Extensions.HintBuilding;
using RueI.Parsing.Enums;

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

        public Player Player;

        public Display Display;

        public void Validate()
        {
            Display.Elements.Clear();
            Display.Elements.Add(Player.Role == RoleTypeId.Spectator ? HintManager.SpectatorElement : HintManager.ScpElement);
        }

        public static void UpdateAndValidateAll()
        {
            foreach (Player player in Player.ReadyList.Where(player => player.Role == RoleTypeId.Spectator || player.IsSCP))
            {
                DisplayDataStore store = player.GetDataStore<DisplayDataStore>();
                store.Validate();
                store.Display.Update();
            }
        }
    }
    
    public static class HintManager
    {
        private static Config Config => Plugin.Instance.Config!;
        
        private static List<string> ActiveSpectatorMessages = new();
        private static List<string> ActiveScpMessages = new();

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

        private static string Content<T>(List<T> list)
        {
            StringBuilder builder = StringBuilderPool.Shared.Rent();

            int fontSize = Config.FontSize;
            
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
        
        private static string ScpContent(DisplayCore core) => Content(ActiveScpMessages);
        
        private static string SpectatorContent(DisplayCore core) => Content(ActiveSpectatorMessages);
    }
}