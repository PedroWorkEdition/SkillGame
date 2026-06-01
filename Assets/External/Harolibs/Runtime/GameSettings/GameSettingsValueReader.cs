using UnityEngine;
using UnityEngine.Events;

namespace HaroLibs {
    public class GameSettingsValueReader : MonoBehaviour {

        enum InitializationType { None, Awake, Start, OnEnable }

        [SerializeField] GameSettingsFieldProvider targetProvider;
        [SerializeField] InitializationType initialization = InitializationType.OnEnable;
        [SerializeField] UnityEvent<int> onIntRead;
        [SerializeField] UnityEvent<float> onFloatRead;
        [SerializeField] UnityEvent<bool> onBoolRead;

        private void Awake() {
            if (initialization == InitializationType.Awake)
                ReadValue();
        }

        private void Start() {
            if (initialization == InitializationType.Start)
                ReadValue();
        }

        private void OnEnable() {
            if (initialization == InitializationType.OnEnable)
                ReadValue();
        }

        public void ReadValue() {
            var val = targetProvider.ReadValue();
            switch (val) {
                case int intVal: onIntRead?.Invoke( intVal ); break;
                case bool boolVal: onBoolRead?.Invoke( boolVal ); break;
                case float floatVal: onFloatRead?.Invoke( floatVal ); break;
                default: Debug.Log( $"Could not handle value of type '{val.GetType()}' from provider of type {targetProvider.GetType()}'" ); break;
            }
        }

    }
}
