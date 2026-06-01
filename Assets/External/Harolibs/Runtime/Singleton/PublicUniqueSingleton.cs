namespace HaroLibs {

    public abstract class PublicUniqueSingleton<T> : PrivateUniqueSingleton<T> where T : class {

        public static T GlobalInstance => Instance;

    }
}
