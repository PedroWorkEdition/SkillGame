using UnityEngine;

namespace HaroLibs {
    public class MinMaxAttribute : PropertyAttribute { 

        public float Min, Max;

        public MinMaxAttribute( float min, float max ) => (Min, Max) = (min, max);

    }
    
}
