using UnityEngine;

namespace HaroLibs {
    public class FloatRandomValueProvider : RandomValueProvider<float> {

        [SerializeField] protected float min, max;

        public virtual float Get( params object[] args ) => Random.Range( min, max );
    }

}
