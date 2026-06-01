using UnityEngine;
using UnityEngine.InputSystem;
using UltEvents;

namespace FashionThoughts {
    public class InputRelay : InputRelayBase {

        [SerializeField] UltEvent onStarted, onPerformed, onReleased;

        protected override void ActionStarted( InputAction.CallbackContext ctx ) => onStarted?.Invoke();
        protected override void ActionPerformed( InputAction.CallbackContext ctx ) => onPerformed?.Invoke();
        protected override void ActionCanceled( InputAction.CallbackContext ctx ) => onReleased?.Invoke();

    }

}
