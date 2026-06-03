using System;
using System.Reflection;

namespace SkillGame.Utils {
    public interface ISaveDataProvider<out T> where T : IWritebleData {

        string ID { get; }
        public string TypeName => typeof( T ).FullName;
        public string AssemblyName => typeof( T ).Assembly.FullName;
        T GetSaveData();
        void LoadData( object data );

        public Type GetWritebleDataType() => Assembly.Load( AssemblyName ).GetType( TypeName );

    }

    public interface IWritebleData { }

}
