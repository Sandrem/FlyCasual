using Upgrade;
using Ship;
using ActionsList;
using Ship.T70XWing;
using System;
using Abilities;

namespace UpgradesList
{
    public class BlackOne : GenericUpgradeSlotUpgrade
    {
        public BlackOne() : base()
        {
            Type = UpgradeType.Title;
            Name = "Black One";
            Cost = 1;

            isUnique = true;

            UpgradeAbilities.Add(new BlackOneAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is T70XWing;
        }
    }
}

namespace Abilities
{
    public class BlackOneAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += RegisterBlackOneAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= RegisterBlackOneAbility;
        }

        private void RegisterBlackOneAbility(GenericAction shipAction)
        {
            if (shipAction.Host.)
        }
    }
}