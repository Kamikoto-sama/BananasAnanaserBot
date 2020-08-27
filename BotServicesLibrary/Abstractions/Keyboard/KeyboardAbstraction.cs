using System;
using System.Collections.Generic;
using System.Linq;

namespace BotServicesLibrary.Abstractions
{
    /// <summary>
    /// Class representing a keyboard abstraction for universal creating keyboard.
    /// </summary>
    public class KeyboardAbstraction
    {
        public KeyboardType Type { get; }

        public string PresetName { get; }
        public KeyboardCreationParams CreationParams { get; }
        public IReadOnlyDictionary<string, object> OtherArguments { get; }

        public KeyboardAbstraction(string presetName, 
            params (string key, object value)[] otherArguments)
        {
            Type = KeyboardType.PresetName;

            PresetName = presetName?.ToLower() ?? throw new ArgumentNullException(nameof(presetName));

            OtherArguments = otherArguments.ToDictionary(tuple => tuple.key, tuple => tuple.value);
        }

        public KeyboardAbstraction(KeyboardCreationParams parameters,
            params (string key, object value)[] otherArguments)
        {
            Type = KeyboardType.Specified;

            CreationParams = parameters ?? throw new ArgumentException(nameof(parameters));

            OtherArguments = otherArguments.ToDictionary(tuple => tuple.key, tuple => tuple.value);
        }

        public KeyboardAbstraction(
            IReadOnlyList<IReadOnlyList<ButtonAbstraction>> buttons,
            bool isOneTime = true,
            bool isInline = false,
            params (string key, object value)[] otherArguments)
            : this(new KeyboardCreationParams(buttons, isOneTime, isInline), otherArguments)
        { }

        public KeyboardAbstraction(IReadOnlyList<ButtonAbstraction> buttons,
            bool isOneTime = true,
            bool isInline = false,
            params (string key, object value)[] otherArguments)
            : this(new KeyboardCreationParams(new [] {buttons}, isOneTime, isInline), otherArguments)
        { }

        public KeyboardAbstraction(ButtonAbstraction button,
            bool isOneTime = true,
            bool isInline = false,
            params (string key, object value)[] otherArguments)
            : this(new KeyboardCreationParams(new[] {new[] {button}}, isOneTime, isInline), otherArguments)
        { }
    }
}