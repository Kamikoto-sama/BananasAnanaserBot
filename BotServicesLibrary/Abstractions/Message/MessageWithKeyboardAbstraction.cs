using System;

namespace BotServicesLibrary.Abstractions
{
    public class MessageWithKeyboardAbstraction : MessageAbstraction
    {
        public KeyboardAbstraction Keyboard { get; }

        public MessageWithKeyboardAbstraction(string text, KeyboardAbstraction keyboard, params (string key, object value)[] arguments) 
            : base(text, arguments)
        {
            Keyboard = keyboard ?? throw new ArgumentNullException(nameof(keyboard));
        }
    }
}