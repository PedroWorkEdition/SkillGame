using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HaroLibsEditor {
    //WIP
    internal class ActionPopup : PopupWindowContent {

        readonly ActionPopupContext[] _ctx;

        public ActionPopup( ActionPopupContext[] context ) => _ctx = context;

        public override Vector2 GetWindowSize() => new( 200, 50 * _ctx.Length );

        public override void OnGUI( Rect rect ) { }

        public override void OnOpen() {
            var root = editorWindow.rootVisualElement;
            root.AddRange( _ctx.Select( ctx => ListElement( ctx.LabelText, ctx.OnClicked ) ).ToArray() );
        }

        Label ListElement( string txt, Action action ) {
            var lbl = new Label( txt );
            lbl.RegisterCallback<ClickEvent>( _ => action?.Invoke() );
            return lbl;
        }

        internal struct ActionPopupContext {
            public string LabelText;
            public Action OnClicked;
        }
    }
}
