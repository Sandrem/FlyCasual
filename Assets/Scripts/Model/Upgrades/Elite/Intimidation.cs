using Upgrade;
using System.Linq;
using Ship;
using System;
using RuleSets;

namespace UpgradesList
{
    public class Intimidation : GenericUpgrade, ISecondEditionUpgrade
    {
        public Intimidation() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Intimidation";
            Cost = 3;

            UpgradeRuleType = typeof(SecondEdition);

            SEImageNumber = 7;

            UpgradeAbilities.Add(new Abilities.SecondEdition.IntimidationAbilitySE());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            // Not required
        }
    }

}

namespace Abilities.SecondEdition
{
    public class IntimidationAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.AfterGotNumberOfDefenceDiceGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.AfterGotNumberOfDefenceDiceGlobal -= CheckAbility;
        }

        private void CheckAbility(ref int count)
        {
            if (HostShip.ShipsBumped.Contains(Combat.Defender))
            {
                Messages.ShowInfo(HostUpgrade.Name + ": Defender rolls 1 fewer defense die");
                count--;
            }
        }
    }
}