using LabApi.Features.Wrappers;

namespace TextChat.EventArgs
{
    public class SendingProximityMessageEventArgs : IDeniable
    {
        public SendingProximityMessageEventArgs(Player player, string text)
        {
            Player = player;
            Text = text;
        }
        
        public Player Player { get; }
        public string Text { get; }
        public string Response { get; set; }
    }
}