using UnityEngine;

namespace FashionThoughts {
    public class Vector2InputRelay : InputRelay<Vector2> { 

        public float ReadXValue( Vector2 val ) => val.x;
        public float ReadYValue( Vector2 val ) => val.y;
    
    }

}
