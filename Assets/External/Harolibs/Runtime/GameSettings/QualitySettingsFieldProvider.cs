using UnityEngine;

namespace HaroLibs {
    public class QualitySettingsFieldProvider : GameSettingsFieldProvider {

        enum QualitySettingField { QualityLevel }

        [SerializeField] QualitySettingField targetField;

        public void SetValue( int val ) {
            switch (targetField) {
                case QualitySettingField.QualityLevel: QualitySettings.SetQualityLevel(val); break;
                default: Debug.Log( $"Setting value of type {val.GetType()}' to {targetField}' is not supported" ); break;
            }
        }

        internal override object ReadValue() => targetField switch {
            QualitySettingField.QualityLevel => QualitySettings.GetQualityLevel(),
            _ => null
        };
    }
}
