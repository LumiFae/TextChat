using LabApi.Features.Wrappers;

namespace TextChat.API.EventArgs
{
    public class SentProximityMessageEventArgs : IEventArgs
    {
        public SentProximityMessageEventArgs(Player player, string text)
        {
            Player = player;
            Text = text;
        }

        public Player Player { get; }
        public string Text { get; set; }
    }
}