using Ship;
using Ship.Vcx100;
using Upgrade;
using System.Linq;
using System;
using UnityEngine;
using Abilities;
using UpgradesList;

namespace UpgradesList
{
    public class GhostV2 : GenericUpgrade
    {
        public GhostV2() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Ghost (Phantom II)";
            NameCanonical = "ghost-swx72";
            ImageUrl = ImageUrls.GetImageUrl(this, NameCanonical + ".png");
            Cost = 0;

            isUnique = true;

            UpgradeAbilities.Add(new GhostV2Ability());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is Vcx100;
        }
    }
}

namespace Abilities
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
                    if (upgrade.GetType() == typeof(PhantomII))
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
