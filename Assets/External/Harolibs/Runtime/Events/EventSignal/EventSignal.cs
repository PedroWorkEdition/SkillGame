using System;
using UltEvents;
using UnityEngine;

namespace HaroLibs {
    public abstract class EventSignal<T> : ScriptableObject, IObservableContainer<T> {

        event Action<T> EventTrigger;

        [SerializeField] UltEvent<T> onEmmited;

        public void Register( Action<T> action ) => EventTrigger += action;
        public void Unregister( Action<T> action ) => EventTrigger -= action;

        public void Emmit( T data ) { 
            EventTrigger?.Invoke( data );
            onEmmited?.Invoke( data );
        }

        public void Emmit( ContainerBase<T> data ) {
            EventTrigger?.Invoke( data );
            onEmmited?.Invoke( data );
        }

    }

}
