using Upgrade;
using System.Collections.Generic;
using System.Linq;
using System;

namespace UpgradesList.SecondEdition
{
    public class TrickShot : GenericUpgrade
    {
        public TrickShot() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Trick Shot",
                UpgradeType.Talent,
                cost: 4,
                abilityType: typeof(Abilities.SecondEdition.TrickShotAbility),
                seImageNumber: 18
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    // When attacking, if the attack is obstructed, you may roll one additional attack die
    public class TrickShotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotStartAsAttacker += CheckAbilityAndAddDice;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotStartAsAttacker -= CheckAbilityAndAddDice;
        }

        private void CheckAbilityAndAddDice()
        {
            if (Combat.ShotInfo.IsObstructedByObstacle)
            {
                HostShip.AfterGotNumberOfAttackDice += RollExtraDie;
            }
        }

        protected void RollExtraDie(ref int diceCount)
        {
            HostShip.AfterGotNumberOfAttackDice -= RollExtraDie;
            Messages.ShowInfo("The attack is obstructed, Trick Shot causes " + HostShip.PilotInfo.PilotName + " to roll +1 attack die");
            diceCount++;
        }
    }
}