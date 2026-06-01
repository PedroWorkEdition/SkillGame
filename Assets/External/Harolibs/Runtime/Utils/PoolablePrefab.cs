using System;
using UnityEngine;
using UnityEngine.Events;

namespace HaroLibs {
    public class PoolablePrefab : MonoBehaviour {

        [SerializeField] UnityEvent onPickedByPool, onReleasedByPool;

        [field: NonSerialized] public string ID { get; private set; }

        internal void SetID( string id ) => ID = id;
        internal void OnPicked() => onPickedByPool?.Invoke();
        internal void OnReleased() => onReleasedByPool?.Invoke();

    }

}
