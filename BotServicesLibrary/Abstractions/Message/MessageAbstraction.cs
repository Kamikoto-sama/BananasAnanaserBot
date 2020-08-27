using System;
using System.Collections.Generic;
using System.Linq;

namespace BotServicesLibrary.Abstractions
{
    /// <summary>
    /// Class representing a message abstraction for universal data transportation inside <see cref="BotService{TUserId}"/>
    /// </summary>
    public class MessageAbstraction
    { 
        public string Text { get; }

        public IReadOnlyDictionary<string, object> Attachments { get; }

        public MessageAbstraction(string text, params (string key, object value)[] attachments)
        {
            if(string.IsNullOrEmpty(text))
                throw new ArgumentException("Message abstraction text can't be null or empty");
            
            Text = text;
            Attachments = attachments.ToDictionary(x => x.key, y => y.value);
        }

        public static implicit operator MessageAbstraction(string s) => new MessageAbstraction(s);
    }
}