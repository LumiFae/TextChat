using LabApi.Features.Wrappers;

namespace TextChat
{
    public static class Events
    {
        public static string TrySendMessage(Player player, string text)
        {
            if (text.Length > 34) return "Message content is too long.";
            if (player.IsAlive && !player.IsSCP)
            {
                Component.Spawn(player, $"<size={Plugin.Instance.Config.TextSize}em>{Plugin.Instance.Translation.Prefix}<noparse>{text.Replace("</noparse>", "")}</noparse></size>");
                return null;
            }

            return null;
        }
    }
}