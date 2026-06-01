namespace HaroLibs {
    public abstract class PrivateUniqueSingleton<T> : PrivateSingleton<T> where T : class {

        protected override void Awake() {
            if (Instance == null) base.Awake();
            else Destroy( gameObject );
        }

    }
}
