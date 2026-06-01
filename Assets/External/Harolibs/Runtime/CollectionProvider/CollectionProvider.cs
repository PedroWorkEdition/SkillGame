using UnityEngine;

namespace HaroLibs {
    public abstract class CollectionProvider<T> : ScriptableObject {

        public abstract T[] Providers { get; }

    }

}
