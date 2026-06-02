using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SkillGame.Utils {

    public static class PersistentSystem {

        const string k_fileName = "save.json";

        static string _filePath;

        static Dictionary<string, ISaveDataProvider<IWritebleData>> _data;
        static Dictionary<string, DataBlock> _cachedData;

        static SaveDataContainer _container;

        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterAssembliesLoaded )]
        public static void Initialize() {
            _cachedData = new();
            _data = new();
            _filePath = $"{Application.persistentDataPath}/{k_fileName}";
            Load();
        }

        public static void Save() {
            var blocks = _data.Keys.Select( dt => new DataBlock( _data[ dt ] ) ).ToArray()
                                   .Concat( _cachedData.Values ).ToArray();
            var val = JsonUtility.ToJson( _container = new SaveDataContainer( blocks ) );
            _container.Initialize();
            File.WriteAllText( _filePath, val );
            Debug.Log( $"Saved succesfully at: {_filePath}" );
        }

        public static void Load() {
            if (!File.Exists( _filePath )) {
                Debug.Log( "No save file found" );
                return;
            }
            var val = File.ReadAllText( _filePath );
            _container = JsonUtility.FromJson<SaveDataContainer>( val );
            _container.Initialize();
            foreach (var block in _container.Data)
                _cachedData.Add( block.ID, block );
            Debug.Log( "Save Data Loaded" );
        }

        public static void ApplyLoadedData( string id ) {
            if (!_data.ContainsKey( id )) {
                Debug.Log( $"No data to load for {id}" );
                return;
            }
            if (_container == null) {
                Debug.Log( "Save data not loaded" );
                return;
            }
            var loadedData = _container.GetBlock( id )?.TargetData;
            if (_cachedData.TryGetValue( id, out var block )) {
                loadedData = block.TargetData;
                _cachedData.Remove( id );
            }
            _data[ id ].LoadData( JsonUtility.FromJson( loadedData, _data[ id ].GetWritebleDataType() ) as IWritebleData );
        }

        public static void RegisterData( ISaveDataProvider<IWritebleData> data ) {
            var id = data.ID;
            if (_data.ContainsKey( id )) {
                Debug.Log( $"Already registered writeble data: '{id}'" );
                return;
            }
            _data.Add( id, data );
        }

        public static void UnregisterData( ISaveDataProvider<IWritebleData> data ) {
            if (!_cachedData.ContainsKey( data.ID ))
                _cachedData.Add( data.ID, null );
            _cachedData[ data.ID ] = new DataBlock( data );
            _data.Remove( data.ID );
        }

        public static void ClearSaveFile() {
            if (!File.Exists( _filePath )) return;
            File.Delete( _filePath );
            _cachedData.Clear();
            _container = null;
        }

    }

    [Serializable]
    internal class SaveDataContainer {

        public DataBlock[] Data;

        Dictionary<string, DataBlock> _map;

        public SaveDataContainer( DataBlock[] blocks ) => Data = blocks;
        public void Initialize() {
            _map = new();
            foreach (var block in Data)
                _map.Add( block.ID, block );
        }

        public DataBlock GetBlock( string id ) => _map.TryGetValue( id, out var data ) ? data : null;

    }

    [Serializable]
    internal class DataBlock {

        public string TypeName, ID, TargetData;

        public DataBlock( ISaveDataProvider<IWritebleData> data ) => (TypeName, ID, TargetData) = (data.TypeName, data.ID, JsonUtility.ToJson( data.GetSaveData() ));

    }

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
