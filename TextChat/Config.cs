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

        [Description("This option tells the client whether their command has failed if true. If false then it won't say it failed, but will continue to show the right translation.")]
        public bool CanFail { get; set; } = true;

        [Description("This option enables debug logging for the plugin, good for debugging issues.")]
        public bool Debug { get; set; } = false;
    }
}