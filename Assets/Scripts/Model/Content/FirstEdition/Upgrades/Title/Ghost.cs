using Ship;
using Upgrade;
using System.Collections.Generic;
using System.Linq;

namespace UpgradesList.FirstEdition
{
    public class Ghost : GenericUpgrade
    {
        public Ghost() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ghost",
                UpgradeType.Title,
                cost: 0,
                isLimited: true,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.VCX100.VCX100)),
                abilityType: typeof(Abilities.FirstEdition.GhostAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class GhostAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Rules.Docking.Dock(GetThisShip, FindPhantom);
        }

        public override void DeactivateAbility()
        {
            // No effect
        }

        private GenericShip GetThisShip()
        {
            return this.HostShip;
        }

        private GenericShip FindPhantom()
        {
            GenericShip result = null;

            foreach (var shipHolder in HostShip.Owner.Ships)
            {
                foreach (var upgrade in shipHolder.Value.UpgradeBar.GetUpgradesOnlyFaceup())
                {
                    if (upgrade.GetType() == typeof(UpgradesList.FirstEdition.Phantom))
                    {
                        result = upgrade.HostShip;
                        break;
                    }
                }
            }

            return result;
        }

    }
}