namespace HaroLibs {
    public interface ILockable {

        bool Locked { get; }

        void Lock();
        void Unlock();
    }
}
