using Ship;
using Ship.StarViper;
using Upgrade;
using UnityEngine;
using System.Collections.Generic;
using Abilities;
using System;

namespace UpgradesList
{
    public class StarViperMkII : GenericUpgradeSlotUpgrade
    {
        public StarViperMkII() : base()
        {
            Type = UpgradeType.Title;
            Name = "StarViper Mk.II";
            Cost = -3;
            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Title) { MustBeDifferent = true }
            };

            UpgradeAbilities.Add(new StarViperMkIIAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is StarViper;
        }
    }
}

namespace Abilities
{
    public class StarViperMkIIAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBarrelRollTemplates += ChangeBarrelRollTemplates;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBarrelRollTemplates -= ChangeBarrelRollTemplates;
        }

        private void ChangeBarrelRollTemplates(List<Actions.BarrelRollTemplates> availableTemplates)
        {
            if (availableTemplates.Contains(Actions.BarrelRollTemplates.Straight1))
            {
                availableTemplates.Remove(Actions.BarrelRollTemplates.Straight1);
                availableTemplates.Add(Actions.BarrelRollTemplates.Bank1);
            }
        }
    }
}
