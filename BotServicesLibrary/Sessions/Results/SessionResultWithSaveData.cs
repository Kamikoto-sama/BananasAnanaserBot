using BotServicesLibrary.Abstractions;
using System;
using System.Collections.Generic;

namespace BotServicesLibrary
{
    public class SessionResultWithSaveData : SessionResult
    {
        public ISaveData SaveData { get; }

        public SessionResultWithSaveData(IReadOnlyCollection<MessageAbstraction> messages, ISaveData saveData, bool isSessionEnd = false)
            : base(messages, isSessionEnd)
        {
            SaveData = saveData ?? throw new ArgumentNullException(nameof(saveData));
        }

        public SessionResultWithSaveData(ISaveData saveData, bool isSessionEnd, params MessageAbstraction[] messages) 
            : this(messages, saveData, isSessionEnd)
        { }

        public SessionResultWithSaveData(ISaveData saveData, params MessageAbstraction[] messages)
            : this(messages, saveData, false)
        { }
    }
}