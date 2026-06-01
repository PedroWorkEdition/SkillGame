namespace HaroLibs {
    public abstract class PublicSingleton<T> : PrivateSingleton<T> where T : class {

        public static T GlobalInstance => Instance;

    }
}
