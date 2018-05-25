using Upgrade;
using UnityEngine;
using Ship;
using Abilities;
using RuleSets;

namespace UpgradesList
{
    public class InstinctiveAim : GenericUpgrade
    {
        public InstinctiveAim() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Instinctive Aim";
            Cost = 1;

            ImageUrl = "https://i.imgur.com/HWa4OmI.png";

            UpgradeRuleType = typeof(SecondEdition);

            UpgradeAbilities.Add(new DeadeyeAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ShipBaseSize == BaseSize.Small;
        }
    }
}