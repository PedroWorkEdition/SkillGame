using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HaroLibs {
    public sealed class ResolutionDropdownOptionCollectionProvider : CollectionProvider<Dropdown.OptionData> {

        public override Dropdown.OptionData[] Providers =>  Screen.resolutions.Select( res => CreateOption( res ) ).ToArray();
        Dropdown.OptionData CreateOption( Resolution res ) => new ( res.ToString(), null );

    }

}
