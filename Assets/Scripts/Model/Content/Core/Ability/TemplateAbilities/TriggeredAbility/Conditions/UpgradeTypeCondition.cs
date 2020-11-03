using System;
using System.Collections.Generic;
using Upgrade;

namespace Abilities
{
    public class UpgradeTypeCondition : Condition
    {
        private UpgradeType UpgradeType;

        public UpgradeTypeCondition(UpgradeType upgradeType)
        {
            UpgradeType = upgradeType;
        }

        public override bool Passed(ConditionArgs args)
        {
            if (args.UpgradeToCheck == null)
            {
                Messages.ShowError("Ability Condition Error: upgrade is not set");
                return false;
            }

            return args.UpgradeToCheck.HasType(UpgradeType);
        }
    }
}
