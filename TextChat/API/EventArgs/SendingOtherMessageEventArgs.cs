using LabApi.Features.Wrappers;

namespace TextChat.API.EventArgs
{
    public class SendingOtherMessageEventArgs : IDeniable
    {
        public SendingOtherMessageEventArgs(Player player, string text)
        {
            Player = player;
            Text = text;
        }

        public Player Player { get; }
        public string Text { get; }
        public string Response { get; set; }
    }
}