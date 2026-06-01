using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HaroLibs {

    public static class AudioSourceExtensions {
        
        public static void PlayRandomClip( this AudioSource source, ICollection<AudioClip> clips ) {
            if (clips.Count == 0) return;
            source.clip = clips.ElementAt( Random.Range( 0, clips.Count ) );
            source.Play();
        }


    }

}
