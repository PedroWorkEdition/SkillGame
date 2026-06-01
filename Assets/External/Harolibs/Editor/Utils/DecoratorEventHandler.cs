using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace HaroLibsEditor {

    public class DecoratorEventHandler : VisualElement {

        public DecoratorEventHandler( string attributeName, System.Action<VisualElement, Object> onelementLoaded ) {
            name = attributeName;
            RegisterCallbackOnce<GeometryChangedEvent>( 
                ctx => onelementLoaded?.Invoke( parent.parent[ parent.parent.childCount - 1 ], Selection.activeObject ) );
        }

    }

}
