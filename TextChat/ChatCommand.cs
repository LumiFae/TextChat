using CommandSystem;
using LabApi.Features.Wrappers;

namespace TextChat
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ChatCommand : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);
            if (player == null)
            {
                response = "Only players can send messages.";
                return false;
            }
            
            string resp = Events.TrySendMessage(player, string.Join(" ", arguments));
            response = resp ?? Plugin.Instance.Translation.Successful;
            return resp == null;
        }

        public string Command { get; } = "chat";
        public string[] Aliases { get; } = Array.Empty<string>();
        public string Description { get; } = "Send a message!";
    }
}