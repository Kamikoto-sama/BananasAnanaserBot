using System;

namespace BotServicesLibrary
{
    public readonly struct GlobalId : IEquatable<GlobalId>
    {
        public readonly string ServiceId;

        public readonly string UserId;

        public GlobalId(string serviceId, string userId)
        {
            ServiceId = serviceId;
            UserId = userId;
        }

        public bool Equals(GlobalId other)
        {
            return ServiceId == other.ServiceId && UserId == other.UserId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ServiceId, UserId);
        }

        public override string ToString() => $"{ServiceId}-{UserId}";
    }
}