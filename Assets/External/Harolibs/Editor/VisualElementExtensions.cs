using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HaroLibsEditor {
    public static class VisualElementExtensions {

        public static VisualElement SetActive( this VisualElement target, bool active ) {
            target.style.display = active ? DisplayStyle.Flex : DisplayStyle.None; 
            return target;
        }

        public static VisualElement SetMargin( this VisualElement target, int val ) => target.SetMargin( val, val, val, val );

        public static VisualElement SetMargin( this VisualElement target, int top, int bottom, int left, int right ) => target.WithStyle( stl => {
            stl.marginTop = top; stl.marginBottom = bottom; stl.marginLeft = left; stl.marginRight = right;
        } );

        public static VisualElement SetPadding( this VisualElement target, int val ) => target.SetPadding( val, val, val, val );

        public static VisualElement SetPadding( this VisualElement target, int top, int bottom, int left, int right ) => target.WithStyle( stl => {
            stl.paddingTop = top; stl.paddingBottom = bottom; stl.paddingLeft = left; stl.paddingRight = right;
        } );

        public static VisualElement SetBorder( this VisualElement target, int width, int radius, Color color = default ) => target.WithStyle( st => {
                st.borderBottomLeftRadius = st.borderBottomRightRadius = st.borderTopLeftRadius = st.borderTopRightRadius = radius;
                st.borderRightWidth = st.borderLeftWidth = st.borderBottomWidth = st.borderTopWidth = width;
                st.borderRightColor = st.borderLeftColor = st.borderBottomColor = st.borderTopColor = color;
            } );
        

        public static VisualElement AddClassRange( this VisualElement target, params string[] classes ) {
            foreach (string c in classes)
                target.AddToClassList( c );
            return target;
        }

        public static VisualElement AddRangeStyleSheet( this VisualElement target, params StyleSheet[] sheets ) {
            foreach (var sheet in sheets) 
                target.styleSheets.Add( sheet );
            return target;
        }

        public static VisualElement AddRangeStyleSheet( this VisualElement target, string path, params string[] styleSheetsParams ) {
            foreach (string sheet in styleSheetsParams) {
                var ss = AssetDatabase.LoadAssetAtPath( path + sheet, typeof( StyleSheet ) ) as StyleSheet;
                target.styleSheets.Add( ss );
            }
            return target;
        }

    }

}
