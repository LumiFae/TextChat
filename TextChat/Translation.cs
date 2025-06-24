namespace TextChat
{
    public class Translation
    {
        public string Prefix { get; set; } = "<color=green>💬: </color>";

        public string Successful { get; set; } = "Successfully sent message.";

        public string CurrentMessage { get; set; } = "Current message:\n{0}";

        public string ContentTooLong { get; set; } = "Message content is too long.";

        public string ContainsBadWord { get; set; } = "Your message contains words blocked by the server.";

        public string NotValidRole { get; set; } = "You are not a valid role to send a message";

        public string NoContent { get; set; } = "You can not send an empty message.";
    }
}