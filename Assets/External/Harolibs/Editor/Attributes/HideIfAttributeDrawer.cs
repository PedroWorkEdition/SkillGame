using UnityEngine;
using UnityEditor;
using HaroLibs;
using UnityEngine.UIElements;
using System.Reflection;
using System.Linq;

namespace HaroLibsEditor {

    [CustomPropertyDrawer( typeof( HideIfAttribute ) )]
    public class HideIfAttributeDrawer : DecoratorDrawer, System.IDisposable {

        HideIfAttribute _att;
        System.Func<bool> _predicate;
        VisualElement _targetElement;

        public override VisualElement CreatePropertyGUI() {
            EditorApplication.update += OnUpdate;
            _att = attribute as HideIfAttribute;
            return new DecoratorEventHandler( attribute.GetType().Name, OnLoaded );
        }

        void OnLoaded( VisualElement targetElement, Object obj ) {
            var member = obj.GetType().GetMember( _att.TargetMember, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ).FirstOrDefault();
            var type = member.GetMemberType();
            if (type == typeof( bool ))
                _predicate = () => member.GetMemberValue<bool>( obj );
            else if (typeof( Object ).IsAssignableFrom( type ))
                _predicate = () => !(member.GetMemberValue( obj ) as Object);
            _targetElement = targetElement;
            OnUpdate();
        }

        void OnUpdate() {
            if (_predicate == null) return;
            var pred = _predicate();
            var result = pred ^ _att.Reverse;
            _targetElement.SetActive( result );
        }

        public void Dispose() => EditorApplication.update -= OnUpdate;
        
    }

}
