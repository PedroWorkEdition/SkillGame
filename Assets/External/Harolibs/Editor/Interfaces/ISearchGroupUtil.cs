namespace HaroLibsEditor {
    public interface ISearchGroupUtil {
        int Level { get; }
        string Name { get; }
        ISearchEntryUtil[] GetSearchEntries();
    }
}
