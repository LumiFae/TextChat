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

            if (arguments.Count < 1)
            {
                response = Plugin.Instance.Translation.NoContent;
                return false;
            }

            string text = string.Join(" ", arguments).Trim();

            if (string.IsNullOrEmpty(text))
            {
                response = Plugin.Instance.Translation.NoContent;
                return false;
            }

            string resp = Events.TrySendMessage(player, text);
            response = resp ?? Plugin.Instance.Translation.Successful;
            return resp == null;
            
            Logger.Debug($"Player {player} has executed the chat command with {text}. The command {(resp == null ? "succeeded" : $"failed with the error {resp}")}", Config.Debug);
            
        }

        public string Command { get; } = "chat";
        public string[] Aliases { get; } = Array.Empty<string>();
        public string Description { get; } = "Send a message!";
    }
}