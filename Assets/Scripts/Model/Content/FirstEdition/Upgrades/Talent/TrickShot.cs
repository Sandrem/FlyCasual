using Upgrade;
using System.Collections.Generic;
using System.Linq;
using System;
using SubPhases;

namespace UpgradesList.FirstEdition
{
    public class TrickShot : GenericUpgrade
    {
        public TrickShot() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Trick Shot",
                UpgradeType.Talent,
                cost: 0,
                abilityType: typeof(Abilities.FirstEdition.TrickShotAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    // When attacking, if the attack is obstructed, you may roll one additional attack die
    public class TrickShotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotStartAsAttacker += CheckAbilityTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotStartAsAttacker -= CheckAbilityTrigger;
        }

        private void CheckAbilityTrigger()
        {
            if (Combat.ShotInfo.IsObstructedByAsteroid)
            {
                RegisterAbilityTrigger(TriggerTypes.OnShotStart, AskTrickShotAbility);
            }
        }

        private void AskTrickShotAbility(object sender, EventArgs e)
        {
            if (!alwaysUseAbility)
            {
                AskToUseAbility(AlwaysUseByDefault, UseAbilityDecision, null, null, true);
            }
            else
            {
                AllowRollAdditionalDie();
                Triggers.FinishTrigger();
            }
        }

        private void UseAbilityDecision(object sender, EventArgs e)
        {
            AllowRollAdditionalDie();
            DecisionSubPhase.ConfirmDecision();
        }

        private void AllowRollAdditionalDie()
        {
            HostShip.AfterGotNumberOfAttackDice += RollExtraDie;
        }

        protected void RollExtraDie(ref int diceCount)
        {
            HostShip.AfterGotNumberOfAttackDice -= RollExtraDie;
            Messages.ShowInfo("The attack is obstructed, Trick Shot causes " + HostShip.PilotInfo.PilotName + " to roll +1 attack die");
            diceCount++;
        }
    }
}