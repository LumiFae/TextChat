using LabApi.Features.Wrappers;

namespace TextChat.API.EventArgs
{
    public class SentOtherMessageEventArgs : IEventArgs
    {
        public SentOtherMessageEventArgs(Player player, string text)
        {
            Player = player;
            Text = text;
        }

        public Player Player { get; }
        public string Text { get; set; }
    }
}