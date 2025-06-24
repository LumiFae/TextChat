using System.ComponentModel;

namespace TextChat
{
    public class Config
    {
        [Description("The height offset of the text based on the player")]
        public float HeightOffset { get; set; } = 0.9f;

        [Description("The size of the text")] public float TextSize { get; set; } = 0.1f;

        [Description("How long will it take for a message to disappear, or switch to the next message")]
        public float MessageExpireTime { get; set; } = 7;

        public int MaxMessageLength { get; set; } = 34;

        [Description("A list of words/multiple words that are banned")]
        public string[] BannedWords { get; set; } = Array.Empty<string>();
    }
}