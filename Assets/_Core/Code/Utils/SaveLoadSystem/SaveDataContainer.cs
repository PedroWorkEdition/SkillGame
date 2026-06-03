using System;
using System.Collections.Generic;

namespace SkillGame.Utils {
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

}
