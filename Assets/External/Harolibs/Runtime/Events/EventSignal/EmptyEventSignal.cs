using System;
using UnityEngine;

namespace HaroLibs {
    public class EmptyEventSignal : ScriptableObject {

        event Action EventTrigger;

        public void Register( Action action ) => EventTrigger += action;
        public void Unregister( Action action ) => EventTrigger -= action;

        public void Raise() => EventTrigger?.Invoke();

    }

}
