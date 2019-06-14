using BoardTools;
using Movement;
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

        private void ChangeBarrelRollTemplates(List<ManeuverTemplate> availableTemplates)
        {
            if (availableTemplates.Any(n => n.Name == "Straight 1"))
            {
                availableTemplates.RemoveAll(n => n.Name == "Straight 1");
                availableTemplates.Add(new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Left, ManeuverSpeed.Speed1));
                availableTemplates.Add(new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Right, ManeuverSpeed.Speed1));
            }
        }
    }
}