using System;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HaroLibs {
    public class GameAudioHandler : PrivateUniqueSingleton<GameAudioHandler> {

        [SerializeField] AudioSourceLink[] sourceLinks;

        protected override void OnDestroy() { 
            base.OnDestroy();
            ClearToken();
        }

        static CancellationTokenSource _token;

        static void ClearToken() {
            _token?.Cancel();
            _token?.Dispose();
            _token = null;
        }

        public static void PlayAudio( AudioData data, params object[] args ) {
            var source = Instance.sourceLinks.FirstOrDefault( src => src.Channel == data.Channel )?.Source;
            if (!source) return;
            if (data.StopPrevious)
                source.Stop();
            source.volume = data.Volume;
            source.pitch = data.GetPitch( args );
            if (data.IsOneShot) {
                source.PlayOneShot( data.GetClip( args ) );
                return;
            }
            ClearToken();
            _token = new CancellationTokenSource();
            FadeTracks( source, data.GetClip( args ) ).AttachExternalCancellation( _token.Token ).Forget();
        }

        static async UniTask FadeTracks( AudioSource source, AudioClip clip ) {
            try {
                while (source.volume > 0) {
                    await UniTask.Yield( _token.Token );
                    source.volume -= Time.deltaTime * 2;
                }
                source.Stop();
                source.clip = clip;
                source.Play();
                while (source.volume < 1) {
                    await UniTask.Yield( _token.Token );
                    source.volume += Time.deltaTime * 2;
                }
            } catch {
                source.Stop();
            }
        }

    }

    [Serializable]
    public class AudioSourceLink {

        [Dropdown( nameof( GetMixerChannels ) )]
        public string Channel;
        public AudioSource Source;

        public string[] GetMixerChannels() {
#if UNITY_EDITOR
            var manager = AssetDatabase.LoadAssetAtPath<GameAudioManager>( AssetDatabase.GUIDToAssetPath( AssetDatabase.FindAssets( "t:GameAudioManager" )[ 0 ] ) );
            return manager.Mixer.FindMatchingGroups( string.Empty ).Select( group => group.name ).ToArray();
#else
            return Array.Empty<string>();
#endif
        }

    }

}
