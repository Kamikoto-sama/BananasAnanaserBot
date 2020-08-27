namespace BotServicesLibrary.Abstractions
{
    public enum KeyboardType
    {
        /// <summary>
        /// Used for getting keyboard from your realized presets on creating message to send.
        /// </summary>
        PresetName,
        /// <summary>
        /// Used to create a new keyboard from <see cref="KeyboardCreationParams"/>
        /// </summary>
        Specified, 
        /// <summary>
        /// For your needs.
        /// </summary>
        Custom
    }
}