using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HaroLibs {
    public class AudioClipRandomValueProvider : RandomValueProvider<AudioClip> { 

        public virtual AudioClip Get( IEnumerable<AudioClip> source, params object[] args ) {
            char target = default;
            foreach (var arg in args) {
                if (arg is char c)
                    target = c;
            }
            var hashCode = target.GetHashCode();
            var predictableIndex = hashCode % source.Count();
            return source.ElementAt( predictableIndex );
        }

    }

}
