using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HaroLibs {
    public abstract class RandomValueProvider<T> : ScriptableObject {

        public virtual T Get( IEnumerable<T> source ) => source.ElementAt( Random.Range( 0, source.Count() ) );

    }

}
