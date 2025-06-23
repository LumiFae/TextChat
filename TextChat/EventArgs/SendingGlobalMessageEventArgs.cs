using LabApi.Features.Wrappers;

namespace TextChat.EventArgs
{
    public class SendingGlobalMessageEventArgs : IDeniable
    {
        public SendingGlobalMessageEventArgs(Player player, string text)
        {
            Player = player;
            Text = text;
        }
        
        public Player Player { get; }
        public string Text { get; }
        public string Response { get; set; }
    }
}