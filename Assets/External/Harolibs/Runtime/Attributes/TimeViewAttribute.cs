using System;
using UnityEngine;

namespace HaroLibs {

    [Flags]
    public enum TimeAvailable : byte {
        Seconds = 1,
        Minutes = 2,
        Hours = 4,
        Default = 255
    }

    public class TimeViewAttribute : PropertyAttribute {

        

        public readonly TimeAvailable TimeFlags;

        public TimeViewAttribute( TimeAvailable timeFlags = TimeAvailable.Default ) => TimeFlags = timeFlags;

    }
}
