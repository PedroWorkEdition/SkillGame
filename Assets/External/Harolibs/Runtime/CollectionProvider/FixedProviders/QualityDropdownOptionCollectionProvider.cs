using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HaroLibs {
    public sealed class QualityDropdownOptionCollectionProvider : CollectionProvider<Dropdown.OptionData> {

        public override Dropdown.OptionData[] Providers => QualitySettings.names.Select( quality => CreateOption( quality ) ).ToArray();
        Dropdown.OptionData CreateOption( string quality ) => new( quality, null );
    }

}
