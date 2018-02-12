using Ship;
using Ship.Vcx100;
using Upgrade;
using System.Linq;
using System;
using UnityEngine;

namespace UpgradesList
{
    public class Ghost : GenericUpgrade
    {
        public Ghost() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Ghost";
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
                    if (upgrade.GetType() == typeof(Phantom))
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
