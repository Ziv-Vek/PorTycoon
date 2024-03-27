using System;
using System.Runtime.Serialization;

namespace SupersonicWisdomSDK
{
    [Serializable]
    public enum ESwLevelType
    {
        [EnumMember]
        Regular = 0,
        [EnumMember]
        Tutorial = 1,
        [EnumMember]
        Bonus = 2,
    }
}