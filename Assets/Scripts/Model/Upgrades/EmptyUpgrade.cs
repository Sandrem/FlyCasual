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
            for (int i = 0; i < types.Count; i++) {
                this.Types.Add (types [i]);
            }

            this.Name = Name;
            this.Cost = Cost;
        }
    }
}