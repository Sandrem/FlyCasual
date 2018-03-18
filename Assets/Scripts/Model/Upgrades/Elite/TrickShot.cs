using Abilities;
using Movement;
using Ship;
using SubPhases;
using System;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList
{
    public class TrickShot : GenericUpgrade
    {
        public TrickShot() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Trick Shot";
            Cost = 0;

            UpgradeAbilities.Add(new TrickShotAbility());
        }
    }
}

namespace Abilities
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
            if (Combat.Attacker.ShipId == HostShip.ShipId && Combat.ShotInfo.IsObstructedByAsteroid)
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

        private void RollExtraDie(ref int diceCount)
        {
            HostShip.AfterGotNumberOfAttackDice -= RollExtraDie;
            Messages.ShowInfo("Trick Shot: +1 attack die");
            diceCount++;         
        }
    }
}