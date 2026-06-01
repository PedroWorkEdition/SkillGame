namespace HaroLibs {
    public interface IInfoProvider {
        
        bool UseLabel { get; }
        string Label { get; }
        void OnInvoke();

    }
}
