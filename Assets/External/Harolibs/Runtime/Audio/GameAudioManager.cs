using UnityEngine;
using UnityEngine.Audio;

namespace HaroLibs {

    [CreateAssetMenu( fileName = "GameAudioManager", menuName = "HaroLibs/Audio/GameAudioManager" )]
    public class GameAudioManager : ScriptableObject {

        [SerializeField] AudioMixer mixer;
        public AudioMixer Mixer => mixer;

    }
}
