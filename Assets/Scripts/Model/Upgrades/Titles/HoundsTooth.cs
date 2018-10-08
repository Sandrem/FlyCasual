using Ship;
using Ship.YV666;
using Upgrade;
using System.Linq;
using System;
using UnityEngine;
using Abilities;
using UpgradesList;
using RuleSets;

namespace UpgradesList
{
    public class HoundsTooth : GenericUpgrade, ISecondEditionUpgrade
    {
        public HoundsTooth() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Hound's Tooth";
            Cost = 1;

            isUnique = true;

            UpgradeRuleType = typeof(SecondEdition);

            UpgradeAbilities.Add(new Abilities.SecondEdition.HoundsToothSE());

            SEImageNumber = 148;
        }

        public void AdaptUpgradeToSecondEdition()
        {
            
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is YV666;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class HoundsToothSE : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCanReleaseDockedShipRegular += DenyRelease;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCanReleaseDockedShipRegular -= DenyRelease;
        }

        private void DenyRelease(ref bool canRelease)
        {
            canRelease = false;
        }
    }
}