using UnityEngine;
using UnityEngine.Audio;

namespace HaroLibs {
    public class AudioSettingsFieldProvider : GameSettingsFieldProvider {

        const float k_minLevel = .001f, k_maxLevel = 1f;
        const byte k_powerVoltage = 20;

        [SerializeField] AudioMixer mixer;
        [SerializeField] string targetVariable;

        public void SetValue( float val ) => SetAudioVolume( targetVariable, val );
        void SetAudioVolume( string channel, float level ) => mixer.SetFloat( channel, Mathf.Log( ProcessAudioLevelInput( level ) ) * k_powerVoltage );
        float ProcessAudioLevelInput( float level ) => Mathf.Clamp( level, k_minLevel, k_maxLevel );

        internal override object ReadValue() {
            if (!mixer.GetFloat( targetVariable, out var val )) return 0;
            return Mathf.Exp( val/k_powerVoltage );
        }
    }
}
