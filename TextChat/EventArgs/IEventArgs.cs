using LabApi.Features.Wrappers;

namespace TextChat.EventArgs
{
    public interface IEventArgs
    {
        /// <summary>
        /// The player sending the message
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// The text content
        /// </summary>
        public string Text { get; }
    }
}