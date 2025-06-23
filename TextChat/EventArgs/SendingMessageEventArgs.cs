using LabApi.Features.Wrappers;

namespace TextChat.EventArgs;

public class SendingMessageEventArgs : IDeniable
{
    public SendingMessageEventArgs(Player player, string text)
    {
        Player = player;
        Text = text;
    }
    
    public Player Player { get; }
    public string Text { get; set; }
    public string Response { get; set; } = null;
}