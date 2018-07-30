using Abilities;
using Movement;
using RuleSets;
using Ship;
using SubPhases;
using System;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList
{
    public class TrickShot : GenericUpgrade, ISecondEditionUpgrade
    {
        public TrickShot() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Trick Shot";
            Cost = 0;

            UpgradeAbilities.Add(new TrickShotAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 1;

            UpgradeAbilities.RemoveAll(ability => ability is Abilities.TrickShotAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.TrickShotAbilitySE);
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

        protected void RollExtraDie(ref int diceCount)
        {
            HostShip.AfterGotNumberOfAttackDice -= RollExtraDie;
            Messages.ShowInfo("Trick Shot: +1 attack die");
            diceCount++;         
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TrickShotAbilitySE : TrickShotAbility
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
            if (Combat.Attacker.ShipId == HostShip.ShipId && Combat.ShotInfo.IsObstructedByAsteroid)
            {
                HostShip.AfterGotNumberOfAttackDice += RollExtraDie;
            }
        }
    }
}