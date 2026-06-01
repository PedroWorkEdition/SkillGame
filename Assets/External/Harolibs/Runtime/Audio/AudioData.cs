using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HaroLibs {

    [CreateAssetMenu( fileName = "New AudioData", menuName = "HaroLibs/Audio/AudioData" )]
    public class AudioData : ScriptableObject {

        [field: SerializeField, Dropdown(nameof(GetMixerChannels))] 
        public string Channel { get; private set; }
        [field: SerializeField] public bool IsOneShot { get; private set; }
        [field: SerializeField] public bool Loop { get; private set; }
        [field: SerializeField] public bool StopPrevious { get; private set; }
        [field: SerializeField] public TransformValueContainer TargetTransform {  get; private set; }

        [SerializeField] AudioClip clip;
        [SerializeField] AudioClip[] randomClips;
        [field: SerializeField, Range( 0, 1 )] public float Volume { get; private set; } = 1;
        [field: SerializeField, Range( 1, 5 )] public int FrequencyLevel { get; private set; } = 2;
        [field: SerializeField] public float Pitch = 1;
        [SerializeField] FloatRandomValueProvider pitchRandomProvider;
        [SerializeField] AudioClipRandomValueProvider clipRandomProvider;
        public void Play() => GameAudioHandler.PlayAudio( this );
        public void Play( char arg ) => GameAudioHandler.PlayAudio( this, arg );
        public AudioClip GetClip( params object[] args ) {
            if (randomClips.Length == 0)
                return clip;
            return clipRandomProvider ? clipRandomProvider.Get( randomClips, args ) : randomClips[ Random.Range( 0, randomClips.Length ) ]; 
        }

        public float GetPitch( params object[] args ) => pitchRandomProvider ? pitchRandomProvider.Get( args ) : Pitch;
        
        public string[] GetMixerChannels() {
#if UNITY_EDITOR
            var manager = AssetDatabase.LoadAssetAtPath<GameAudioManager>( AssetDatabase.GUIDToAssetPath( AssetDatabase.FindAssets( "t:GameAudioManager" )[ 0 ] ) );
            return manager.Mixer.FindMatchingGroups( string.Empty ).Select( group => group.name ).ToArray();
#else
            return System.Array.Empty<string>();
#endif
        }

    }

}
