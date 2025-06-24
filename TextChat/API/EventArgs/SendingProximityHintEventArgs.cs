using LabApi.Features.Wrappers;

namespace TextChat.API.EventArgs
{
    public class SendingProximityHintEventArgs : IEventArgs
    {
        public SendingProximityHintEventArgs(Player player, string text, string hintContent)
        {
            Player = player;
            Text = text;
            HintContent = hintContent;
        }

        public Player Player { get; }
        public string Text { get; }

        public string HintContent { get; set; }

        public bool IsAllowed { get; set; } = true;
    }
}