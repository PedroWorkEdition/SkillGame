namespace HaroLibs { 
    public class AudioDataValueContainer : ValueContainerBase<AudioData> {

        public void Play() => Value.Play();
        public void Play( char arg ) => Value.Play( arg );

    } 
}
