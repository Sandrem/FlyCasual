using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class StarViperMkII : GenericUpgrade
    {
        public StarViperMkII() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "StarViper Mk.II",
                UpgradeType.Title,
                cost: -3,
                addSlot: new UpgradeSlot(UpgradeType.Title) { MustBeDifferent = true },
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.StarViper.StarViper)),
                abilityType: typeof(Abilities.FirstEdition.StarViperMkIIAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
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

        private void ChangeBarrelRollTemplates(List<ActionsHolder.BarrelRollTemplates> availableTemplates)
        {
            if (availableTemplates.Contains(ActionsHolder.BarrelRollTemplates.Straight1))
            {
                availableTemplates.Remove(ActionsHolder.BarrelRollTemplates.Straight1);
                availableTemplates.Add(ActionsHolder.BarrelRollTemplates.Bank1);
            }
        }
    }
}