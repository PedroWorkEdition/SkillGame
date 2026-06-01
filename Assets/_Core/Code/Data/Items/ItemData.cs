using HaroLibs;
using UnityEngine;

namespace SkillGame.Data {

    public abstract class ItemData : ScriptableObject {

        [field: SerializeField, AutoGUID] public string ID { get; private set; }
        [field: SerializeField] public string ItemName { get; private set; }
        [field: SerializeField, TextArea] public string Description { get; private set; }
        [field: SerializeField] public bool Stackable { get; private set; }
        [field: SerializeField, HideIf( nameof( Stackable ) )] public int MaxStack { get; private set; } = 99;
        [field: SerializeField] public bool ConsumedWhenUsed { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }

        public abstract bool Use( Inventory inventory );

    }

    public abstract class ItemData<T> : ItemData where T : IItemAction {
        [field: SerializeReference, SerializeReferenceDropdown] public T[] Actions { get; private set; }

    }

    public interface IItemAction {

        bool Use( Inventory source );

    }
    public interface IConsumableItem : IItemAction { }
    public interface IEquipableItem : IItemAction { }

}
