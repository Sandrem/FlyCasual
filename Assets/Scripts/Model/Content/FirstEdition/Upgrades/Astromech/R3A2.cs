using Upgrade;
using System;
using Tokens;

namespace UpgradesList.FirstEdition
{
    public class R3A2 : GenericUpgrade
    {
        public R3A2() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R3-A2",
                UpgradeType.Astromech,
                cost: 2,
                isLimited: true,
                abilityType: typeof(Abilities.FirstEdition.R3A2Ability)
            );
        }
    }
}

namespace Abilities.FirstEdition
{
    public class R3A2Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += CheckR3A2Ability;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= CheckR3A2Ability;
        }

        private void CheckR3A2Ability()
        {
            if (Combat.AttackStep == CombatStep.Attack && Combat.Attacker.ShipId == HostShip.ShipId)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskAssignStress);
            }
        }

        private void AskAssignStress(object sender, System.EventArgs e)
        {
            if (!alwaysUseAbility)
            {
                AskToUseAbility(AlwaysUseByDefault, AssignStressAndFinishDecision, null, null, true);
            }
            else
            {
                AssignStress(Triggers.FinishTrigger);
            }
        }

        private void AssignStressAndFinishDecision(object sender, System.EventArgs e)
        {
            AssignStress(SubPhases.DecisionSubPhase.ConfirmDecision);
        }

        private void AssignStress(Action callback)
        {
            Messages.ShowInfo(Name + " is used");
            Sounds.PlayShipSound("R2D2-Beeping-5");

            HostShip.Tokens.AssignToken(typeof(StressToken), delegate { AssignStressToDefender(callback); });
        }

        private void AssignStressToDefender(Action callback)
        {
            Combat.Defender.Tokens.AssignToken(typeof(StressToken), callback);
        }

    }
}