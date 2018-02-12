using Ship;
using Ship.Vcx100;
using Upgrade;
using System.Linq;
using System;
using UnityEngine;

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
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is Vcx100;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            Rules.Docking.Dock(GetHisShip, FindPhantom);
        }

        private GenericShip GetHisShip()
        {
            return this.Host;
        }

        private GenericShip FindPhantom()
        {
            GenericShip result = null;

            foreach (var shipHolder in Host.Owner.Ships)
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
