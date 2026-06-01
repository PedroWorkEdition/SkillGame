using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace HaroLibsEditor {
    internal class MouseDataManipulator : MouseManipulator {

        readonly Action<Vector2> _getMousePos;
        internal MouseDataManipulator( Action<Vector2> getMousePos ) => _getMousePos = getMousePos;
        protected override void RegisterCallbacksOnTarget() => target.RegisterCallback<MouseDownEvent>( OnMouseDown );
        protected override void UnregisterCallbacksFromTarget() => target.UnregisterCallback<MouseDownEvent>( OnMouseDown );
        void OnMouseDown( MouseDownEvent evt ) {
            if (!CanStartManipulation( evt )) return;
            _getMousePos?.Invoke( evt.originalMousePosition );
        }
    }
}
