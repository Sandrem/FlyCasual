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

        public void SetUpgradeInfo(UpgradeType type, string Name, int Cost )
        {
            UpgradeInfo = new UpgradeCardInfo(Name, type: type, cost: Cost);
        }
    }
}