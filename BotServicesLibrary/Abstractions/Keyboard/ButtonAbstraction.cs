using System;

namespace BotServicesLibrary.Abstractions
{
    /// <summary>
    /// Element for creating specified keyboard in <see cref="KeyboardCreationParams"/>
    /// </summary>
    public class ButtonAbstraction
    {
        public string Text { get; }
        public string Payload { get; }

        public ButtonAbstraction(string text, string payload = null)
        {
            Text = text;

            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Button text can't be null or empty");

            Payload = payload ?? "";
        }

        public static implicit operator ButtonAbstraction(string text) => new ButtonAbstraction(text);
    }
}