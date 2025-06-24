namespace TextChat.RueI
{
    public class Translation
    {
        public string Header { get; set; } = "TextChat Settings";

        public string HeaderHint { get; set; } = "This will only modify settings inside of SCP and Spectator chats.";

        public string ShouldShowSpectatorSelect { get; set; } = "Show Spectator Chat?";

        public string ShouldShowSpectatorSelectHint { get; set; } = "Whether to show the spectator chat.";

        public string Yes { get; set; } = "Yes";
        public string No { get; set; } = "No";

        public string TextSizeSlider { get; set; } = "Text Size";

        public string TextSizeSliderHint { get; set; } = "How large should the text size be in the spectator/SCP chats?";
    }
}