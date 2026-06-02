using SkillGame.Data;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SkillGame.UI {

    public class UI_InventoryItemView : MonoBehaviour {

        [SerializeField] GameObject contentContainer;
        [SerializeField] Image icon;
        [SerializeField] TMP_Text itemName, itemDescription;
        [SerializeField] TMP_Text[] additionalData;

        public void SetData( ItemData data ) {
            if (!data) {
                contentContainer.SetActive( false );
                return;
            }
            icon.sprite = data.Icon;
            itemName.text = data.ItemName;
            itemDescription.text = data.Description;
            SetAdditionalData( data );
            contentContainer.SetActive( true );
        }

        void SetAdditionalData( ItemData data ) {
            List<string> additionalText = new ();
            switch( data ) {
                case WeaponData weapon:
                    additionalText.Add( $"Damage: {weapon.Damage}" );
                    break;
                default: break;
            }
            for (int i = 0; i < additionalData.Length; i++) {
                if (i >= additionalText.Count) {
                    additionalData[ i ].gameObject.SetActive( false );
                    continue;
                }
                additionalData[ i ].text = additionalText[ i ];
                additionalData[ i ].gameObject.SetActive( true );
            }
        }

    }

}
