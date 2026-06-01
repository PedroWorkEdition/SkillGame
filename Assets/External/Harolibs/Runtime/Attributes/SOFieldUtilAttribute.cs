using System;
using UnityEngine;

namespace HaroLibs {
    public class SOFieldUtilAttribute : PropertyAttribute {

        [Flags]
        public enum DisplayMode : byte {
            Default = 0,
            Create = 1,
            Properties = 2,
            Edit = 4,
            Complete = 255
        }

        public readonly DisplayMode Display;
        public readonly bool RemoveLabel;

        public SOFieldUtilAttribute( DisplayMode display = DisplayMode.Create | DisplayMode.Properties, bool removeLabel = false ) =>
                                     (Display, RemoveLabel) = (display, removeLabel);

        public SOFieldUtilAttribute( byte display, bool removeLabel = false ) =>
                                     (Display, RemoveLabel) = (( DisplayMode )display, removeLabel);

    }
}
