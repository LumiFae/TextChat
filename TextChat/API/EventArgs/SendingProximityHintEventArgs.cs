using LabApi.Features.Wrappers;

namespace TextChat.API.EventArgs
{
    public class SendingProximityHintEventArgs : IEventArgs
    {
        public SendingProximityHintEventArgs(Player player, string text)
        {
            Player = player;
            Text = text;
            IsAllowed = true;
        }
        
        public Player Player { get; }
        public string Text { get; }
        
        public bool IsAllowed { get; set; }
    }
}