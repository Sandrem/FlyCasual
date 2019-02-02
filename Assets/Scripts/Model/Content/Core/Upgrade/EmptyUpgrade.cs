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

        public void SetUpgradeInfo( List<UpgradeType> types, string Name, int Cost )
        {
            UpgradeInfo = new UpgradeCardInfo(Name, types: types, cost: Cost);
        }
    }
}