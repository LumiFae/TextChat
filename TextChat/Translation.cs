namespace TextChat
{
    public class Translation
    {
        public string Prefix { get; set; } = "<color=green>💬: </color>";

        public string CommandName { get; set; } = "chat";

        public string CommandDescription { get; set; } = "Send a text chat message";
        
        public string Successful { get; set; } = "Successfully sent message.";

        public string ContentTooLong { get; set; } = "Message content is too long.";

        public string ContainsBadWord { get; set; } = "Your message contains words blocked by the server.";

        public string NotValidRole { get; set; } = "You are not a valid role to send a message";
    }
}