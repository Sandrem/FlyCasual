using Actions;
using ActionsList;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class TacticalScrambler : GenericUpgrade
    {
        public TacticalScrambler() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Tactical Scrambler",
                UpgradeType.Modification,
                cost: 2,
                restriction: new BaseSizeRestriction(BaseSize.Medium, BaseSize.Large),
                abilityType: typeof(Abilities.SecondEdition.TacticalScramblerAbility),
                seImageNumber: 78
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TacticalScramblerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotObstructedByMe += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotObstructedByMe -= CheckAbility;
        }

        private void CheckAbility(GenericShip attacker, ref int count)
        {
            if (attacker.Owner.PlayerNo != HostShip.Owner.PlayerNo)
            {
                Messages.ShowInfo(string.Format("{0}: The attack is obstructed by {1}, giving the defender +1 defense die", HostUpgrade.UpgradeInfo.Name, HostShip.PilotInfo.PilotName));
                count++;
            }
        }
    }
}