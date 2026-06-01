using UnityEngine;

namespace HaroLibs {
    public class AudioClipCollectionProvider : CollectionProvider<AudioClip> {

        [SerializeField] AudioClip[] clips;
        public override AudioClip[] Providers => clips;
    }

}
