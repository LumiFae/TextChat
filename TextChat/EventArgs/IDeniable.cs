namespace TextChat.EventArgs
{
    public interface IDeniable : IEventArgs
    {
        /// <summary>
        /// Set this to null if not to deny;
        /// </summary>
        public string Response { get; set; }
    }
}