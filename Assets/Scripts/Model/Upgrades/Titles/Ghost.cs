using Ship;
using Ship.Vcx100;
using Upgrade;
using System.Linq;
using System;
using UnityEngine;
using Abilities;
using UpgradesList;
using RuleSets;

namespace UpgradesList
{
    public class Ghost : GenericUpgrade, ISecondEditionUpgrade
    {
        public Ghost() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Ghost";
            Cost = 0;

            isUnique = true;

            UpgradeAbilities.Add(new GhostAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            //Nothing to do for now
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is Vcx100;
        }
    }
}

namespace Abilities
{
    public class GhostAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Rules.Docking.Dock(GetHisShip, FindPhantom);
        }

        public override void DeactivateAbility()
        {
            // No effect
        }

        private GenericShip GetHisShip()
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
