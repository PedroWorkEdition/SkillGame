using UnityEngine;
using UnityEngine.InputSystem;
using UltEvents;

namespace FashionThoughts {

    public abstract class InputRelayBase : MonoBehaviour {

        [SerializeField] InputActionReference input;

        protected virtual void Awake() {
            input.action.started += ActionStarted;
            input.action.performed += ActionPerformed;
            input.action.canceled += ActionCanceled;
        }

        protected virtual void OnEnable() => input.action.Enable();
        protected virtual void OnDisable() => input.action.Disable();

        protected abstract void ActionStarted( InputAction.CallbackContext ctx );
        protected abstract void ActionPerformed( InputAction.CallbackContext ctx );
        protected abstract void ActionCanceled( InputAction.CallbackContext ctx );

    }

    public abstract class InputRelay<T> : InputRelayBase where T : struct {

        [SerializeField] UltEvent<T> onStarted, onPerformed, onReleased;

        protected override void ActionStarted( InputAction.CallbackContext ctx ) => onStarted?.Invoke( ctx.ReadValue<T>() );
        protected override void ActionPerformed( InputAction.CallbackContext ctx ) => onPerformed?.Invoke( ctx.ReadValue<T>() );
        protected override void ActionCanceled( InputAction.CallbackContext ctx ) => onReleased?.Invoke( ctx.ReadValue<T>() );

    }

}
