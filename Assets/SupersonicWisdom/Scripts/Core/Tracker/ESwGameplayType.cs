using System;
using System.Runtime.Serialization;

namespace SupersonicWisdomSDK
{
    [Serializable]
    public enum ESwGameplayType
    {
        [EnumMember]
        Level,
        [EnumMember]
        Meta
    }
}