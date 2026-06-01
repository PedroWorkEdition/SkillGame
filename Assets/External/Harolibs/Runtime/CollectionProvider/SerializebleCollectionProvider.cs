using UnityEngine;

namespace HaroLibs {
    public abstract class SerializebleCollectionProvider<T> : CollectionProvider<T> {

        [SerializeField] T[] values;

        public override T[] Providers => values;

    }

}
