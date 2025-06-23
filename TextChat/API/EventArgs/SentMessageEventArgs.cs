using LabApi.Features.Wrappers;
using TextChat.API.Enums;

namespace TextChat.API.EventArgs
{
    public class SentMessageEventArgs : IEventArgs
    {
        public SentMessageEventArgs(Player player, string text)
        {
            Player = player;
            Text = text;
        }

        public Player Player { get; }
        
        public string Text { get; set; }

        public MessageType Type => Player.IsAlive && !Player.IsSCP ? MessageType.Proximity : MessageType.Other;
    }
}