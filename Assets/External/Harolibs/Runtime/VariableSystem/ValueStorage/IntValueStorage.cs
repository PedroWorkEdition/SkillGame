using System;
using UnityEngine;

namespace HaroLibs {
    public class IntValueStorage : ValueStorageBase<int> {

        public void Add( int val ) => SetValue( val + Value );
        public void Add( IntValueContainer val ) => SetValue( val + Value );
        public void Add( float val ) => SetValue( Mathf.RoundToInt( val ) + Value );
        public void Add( FloatValueContainer val ) => SetValue( Mathf.RoundToInt( val ) + Value );

    }
}
