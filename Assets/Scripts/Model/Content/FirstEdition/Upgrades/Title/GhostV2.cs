using Ship;
using Upgrade;
using System.Collections.Generic;
using System.Linq;

namespace UpgradesList.FirstEdition
{
    public class GhostV2 : GenericUpgrade
    {
        public GhostV2() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ghost (Phantom II)",
                UpgradeType.Title,
                cost: 0,
                isLimited: true,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.VCX100.VCX100)),
                abilityType: typeof(Abilities.FirstEdition.GhostV2Ability)
            );

            // TODOREVERT
            // NameCanonical = "ghost-swx72";
            // ImageUrl = ImageUrls.GetImageUrl(this, NameCanonical + ".png");
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class GhostV2Ability : GenericAbility
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
                    if (upgrade.GetType() == typeof(UpgradesList.FirstEdition.PhantomII))
                    {
                        result = upgrade.Host;
                        break;
                    }
                }
            }

            return result;
        }

    }
}