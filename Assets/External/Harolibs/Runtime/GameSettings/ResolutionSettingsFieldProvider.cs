using System.Linq;
using UnityEngine;

namespace HaroLibs {

    public class ResolutionSettingsFieldProvider : GameSettingsFieldProvider {

        enum ResolutionSettingTarget { Resolution, FullScreen }

        [SerializeField] ResolutionSettingTarget targetField;

        public void SetValue( bool val ) {
            switch (targetField) {
                case ResolutionSettingTarget.FullScreen: Screen.fullScreen = val; break;
                default: Debug.Log( $"Setting value of type {val.GetType()}' to {targetField}' is not supported" ); break;
            }
        }

        public void SetValue( int val ) {
            switch (targetField) {
                case ResolutionSettingTarget.Resolution:
                    var resolution = Screen.resolutions[ val ];
                    Screen.SetResolution( resolution.width, resolution.height, Screen.fullScreen );
                    break;
                default: Debug.Log( $"Setting value of type {val.GetType()}' to {targetField}' is not supported" ); break;
            }
        }

        internal override object ReadValue() => targetField switch {
            ResolutionSettingTarget.Resolution => GetResolutionIndex(),
            ResolutionSettingTarget.FullScreen => Screen.fullScreen,
            _ => null
        };

        int GetResolutionIndex() {
            int index = Screen.resolutions.ToList().IndexOf( Screen.currentResolution );
            if (index >= 0) return index;
            Debug.Log( "Resolution was not set correctly" );
            var currentRes = new Vector2( Screen.width, Screen.height );
            float closestDistante = float.MaxValue;
            int closestIndex = -1;
            for (int i = 0; i < Screen.resolutions.Length; i++) {
                float distance = Vector2.Distance( currentRes, ResToVector( Screen.resolutions[ i ] ) );
                if (distance < closestDistante) {
                    closestDistante = distance;
                    closestIndex = i;
                }
            }
            return closestIndex;
        }

        Vector2 ResToVector( Resolution res ) => new ( res.width, res.height );

    }
}
