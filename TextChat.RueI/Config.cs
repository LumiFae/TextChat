using System.ComponentModel;
using RueI.Utils.Enums;

namespace TextChat.RueI
{
    public class Config
    {
        [Description("The amount of time til a message expires/deletes.")]
        public float MessageExpireTime { get; set; } = 10;

        [Description("The amount of time a user must wait before sending a new message.")]
        public float MessageCooldown { get; set; } = 2;

        [Description("The content that appears before a message, {0} is the player nickname")]
        public string Prefix { get; set; } = "<color=green>{0}: </color>";

        [Description("The default size of the text, in pixels, also a server specific setting for per person.")]
        public int FontSize { get; set; } = 28;

        [Description("The alignment of the hint.")]
        public AlignStyle Alignment { get; set; } = AlignStyle.Left;

        [Description("The vertical position of the hint.")]
        public int VerticalPosition { get; set; } = 200;
    }
}