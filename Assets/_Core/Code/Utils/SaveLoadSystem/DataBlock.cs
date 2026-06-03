using System;
using UnityEngine;

namespace SkillGame.Utils {
    [Serializable]
    internal class DataBlock {

        public string TypeName, ID, TargetData;

        public DataBlock( ISaveDataProvider<IWritebleData> data ) => (TypeName, ID, TargetData) = (data.TypeName, data.ID, JsonUtility.ToJson( data.GetSaveData() ));

    }

}
