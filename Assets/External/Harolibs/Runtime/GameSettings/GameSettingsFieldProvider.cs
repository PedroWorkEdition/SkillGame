using UnityEngine;

namespace HaroLibs {
    public abstract class GameSettingsFieldProvider : ScriptableObject {

        internal abstract object ReadValue();

    }
}
