using System;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList
{

    public class EmptyUpgrade : GenericUpgrade
    {
        public EmptyUpgrade( ) : base()
        {
            isPlaceholder = true;
        }
        public void set( List<UpgradeType> types, string Name, int Cost ) {
            // i starts at one here to skip the first upgrade slot (which has already been allocated). Fixes bug #708
            /*for (int i = 1; i < types.Count; i++) {
                this.Types.Add (types [i]);
            }*/

            // this.Name = Name;
            // this.Cost = Cost;
            // TODOREVERT
        }
    }
}