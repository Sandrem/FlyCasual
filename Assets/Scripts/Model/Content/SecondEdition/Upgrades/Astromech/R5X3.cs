using Ship;
using SubPhases;
using System;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class R5X3 : GenericUpgrade
    {
        public R5X3() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R5-X3",
                UpgradeType.Astromech,
                charges: 2,
                cost: 5,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Resistance),
                abilityType: typeof(Abilities.SecondEdition.R5X3)
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/aed4536536b67bae316b260ed151c22a.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //Before you activate or engage, you may spend 1 charge to ignore obstacles until the end of this phase.
    public class R5X3 : GenericAbility
    {
        private string AbilityDescription = "R5-X3: Spend 1 charge to ignore obstacles until the end of this phase?";

        public override void ActivateAbility()
        {
            HostShip.OnActivationPhaseStart += RegisterTriggerActivationPhase;
            HostShip.OnCombatActivation += RegisterTriggerCombatPhase;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActivationPhaseStart -= RegisterTriggerActivationPhase;
            HostShip.OnCombatActivation -= RegisterTriggerCombatPhase;
        }

        public void RegisterTriggerActivationPhase(GenericShip host)
        {
            if (HostUpgrade.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActivationPhaseStart, AskToUseAbilityActivationPhase);
            }
        }

        public void RegisterTriggerCombatPhase(GenericShip host)
        {
            if (HostUpgrade.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskToUseAbilityCombatPhase);
            }
        }

        private void AskToUseAbilityActivationPhase(object sender, EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, TurnOnIgnoreObstaclesActivationPhase, null, null, false, AbilityDescription);
        }

        private void AskToUseAbilityCombatPhase(object sender, EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, TurnOnIgnoreObstaclesCombatPhase, null, null, false, AbilityDescription);
        }

        private void TurnOnIgnoreObstaclesActivationPhase(object sender, EventArgs e)
        {
            HostUpgrade.State.SpendCharge();
            HostShip.IsIgnoreObstacles = true;
            HostShip.IsIgnoreObstacleObstructionWhenAttacking = true;
            Phases.Events.OnActivationPhaseEnd_NoTriggers += TurnOffIgnoreObstaclesActivationPhase;
            DecisionSubPhase.ConfirmDecision();
        }

        private void TurnOnIgnoreObstaclesCombatPhase(object sender, EventArgs e)
        {
            HostUpgrade.State.SpendCharge();
            HostShip.IsIgnoreObstacles = true;
            HostShip.IsIgnoreObstacleObstructionWhenAttacking = true;
            GenericShip.OnCanAttackWhileLandedOnObstacleGlobal += CanAttack;
            Phases.Events.OnCombatPhaseEnd_NoTriggers += TurnOffIgnoreObstaclesCombatPhase;
            DecisionSubPhase.ConfirmDecision();
        }

        private void TurnOffIgnoreObstaclesActivationPhase()
        {
            HostShip.IsIgnoreObstacles = false;
            HostShip.IsIgnoreObstacleObstructionWhenAttacking = false;
            Phases.Events.OnActivationPhaseEnd_NoTriggers -= TurnOffIgnoreObstaclesActivationPhase;
        }

        private void TurnOffIgnoreObstaclesCombatPhase()
        {
            HostShip.IsIgnoreObstacles = false;
            HostShip.IsIgnoreObstacleObstructionWhenAttacking = false;
            GenericShip.OnCanAttackWhileLandedOnObstacleGlobal -= CanAttack;
            Phases.Events.OnCombatPhaseEnd_NoTriggers -= TurnOffIgnoreObstaclesCombatPhase;
        }

        private void CanAttack(GenericShip ship, ref bool canAttack)
        {
            if (ship == HostShip) canAttack = true;
        }
    }
}