using SkillGame.Data;
using System;
using UnityEngine;

namespace SkillGame {

    [Serializable]
    public class HealingConsumable : IConsumableItem {

        [SerializeField] int amount = 1;

        public bool Use( Inventory source ) {
            var holder = source.Holder as CharacterInventory;
            if (!holder.Character || !holder.Character.Health) return false;
            holder.Character.Health.Heal( amount );
            return true;
        }
    }

}
