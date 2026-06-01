using HaroLibs;
using UnityEditor;
using UnityEngine.UIElements;

namespace HaroLibsEditor {
    [CustomEditor( typeof( BoolValueObserver ) )]
    public class BoolValueObserverEditor : ValueObserverBaseEditor {

        protected override VisualElement EventContainer() =>
            CreateBox().AddRange( CreateBoolField( serializedObject.FindProperty( "invert" ) ),
                                  serializedObject.CreatePropField( "onChanged" ),
                                  serializedObject.CreatePropField( "onTrue" ),
                                  serializedObject.CreatePropField( "onFalse" )
                                 );

    }
}
