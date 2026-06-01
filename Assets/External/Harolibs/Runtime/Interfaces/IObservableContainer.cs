using System;

namespace HaroLibs {
    public interface IObservableContainer<T> {

        void Register( Action<T> val );
        void Unregister( Action<T> val );

    }
}
