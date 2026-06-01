using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HaroLibsEditor {
    public class ContentBox : VisualElement {

        public Label label;
        public VisualElement Content;

        readonly string _id;
        bool _isExpanded;

        /// <summary>
        /// The target type is what defines the 'foldout' id
        /// </summary>
        public ContentBox( Type targetType, string label, params VisualElement[] content ) {
            _id = $"{nameof(ContentBox)}_{targetType}_{label}";
            _isExpanded = EditorPrefs.GetBool( _id, false );
            Content = new();
            Content.style.display = _isExpanded ? DisplayStyle.Flex : DisplayStyle.None;
            Content.style.marginTop = Content.style.marginBottom = Content.style.marginLeft = Content.style.marginRight = 10;
            Content.AddRange( content );
            style.marginTop = 10;
            style.backgroundColor = new( new Color32( 48, 48, 48, 255 ) );
            style.borderBottomLeftRadius = style.borderBottomRightRadius = style.borderTopLeftRadius = style.borderTopRightRadius = 20;
            this.AddRange( CreateHeader( label ), Content );
        }
        VisualElement CreateHeader( string lbl ) {
            var container = new VisualElement();
            label = new( lbl );
            label.style.fontSize = 24;
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            container.style.paddingBottom = container.style.paddingTop = 10;
            container.RegisterCallback<ClickEvent>( ctx => {
                _isExpanded = !_isExpanded;
                Content.style.display = _isExpanded ? DisplayStyle.Flex : DisplayStyle.None;
                EditorPrefs.SetBool( _id, _isExpanded );
            } );
            return container.AddRange(label);
        }
    }
}
