using System;

namespace HaroLibsEditor{
    public interface ISearchEntryUtil {
        int Level { get; }
        Type DataType { get; }
        string Name { get; }
        object GetUserData();
    }
}
