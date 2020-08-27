using System;
using System.Collections.Generic;

namespace BotServicesLibrary.Abstractions
{
    /// <summary>
    /// Params for creating "Specified" keyboard for <see cref="KeyboardAbstraction"/>
    /// </summary>
    public class KeyboardCreationParams
    {
        public IReadOnlyList<IReadOnlyList<ButtonAbstraction>> Buttons { get; }
        public bool IsOneTime { get; }
        public bool IsInline { get; }

        public KeyboardCreationParams(IReadOnlyList<IReadOnlyList<ButtonAbstraction>> buttons, bool isOneTime = false, bool isInline = false)
        {
            Buttons = buttons ?? throw new ArgumentNullException(nameof(buttons));
            IsOneTime = isOneTime;
            IsInline = isInline;
        }

        public KeyboardCreationParams(bool isOneTime, bool isInline, params IReadOnlyList<ButtonAbstraction>[] buttons)
        {
            Buttons = buttons ?? throw new ArgumentNullException(nameof(buttons));
            IsOneTime = isOneTime;
            IsInline = isInline;
        }
    }
}