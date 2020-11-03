using System;
using System.Collections.Generic;
using Upgrade;

namespace Abilities
{
    public class HasUpgradeTypeInstalledCondition : Condition
    {
        private UpgradeType UpgradeType;

        public HasUpgradeTypeInstalledCondition(UpgradeType upgradeType)
        {
            UpgradeType = upgradeType;
        }

        public override bool Passed(ConditionArgs args)
        {
            if (args.ShipToCheck == null)
            {
                Messages.ShowError("Ability Condition Error: ship is not set");
                return false;
            }

            return args.ShipToCheck.UpgradeBar.HasUpgradeTypeInstalled(UpgradeType);
        }
    }
}
