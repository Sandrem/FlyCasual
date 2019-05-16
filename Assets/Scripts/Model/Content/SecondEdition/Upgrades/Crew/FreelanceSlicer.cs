using Ship;
using SubPhases;
using System;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class FreelanceSlicer : GenericUpgrade
    {
        public FreelanceSlicer() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Freelance Slicer",
                UpgradeType.Crew,
                cost: 3,
                abilityType: typeof(Abilities.SecondEdition.FreelanceSlicerCrewAbility),
                seImageNumber: 42
            );
        }
    }
}
namespace Abilities.SecondEdition
{
    public class FreelanceSlicerCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsDefender += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsDefender -= CheckAbility;
        }

        private void CheckAbility()
        {
            if (ActionsHolder.HasTargetLockOn(HostShip, Combat.Attacker))
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskToUseOwnAbility);
            }
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                AlwaysUseByDefault,
                UseOwnAbility,
                infoText: "Do you want to use Freelance Slicer's ability?"
            );
        }

        private void UseOwnAbility(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.Tokens.SpendToken(
                typeof(BlueTargetLockToken),
                AssignJamTokenToAttacker,
                ActionsHolder.GetTargetLocksLetterPairs(HostShip, Combat.Attacker).First()
            );
        }

        private void AssignJamTokenToAttacker()
        {
            Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": " + Combat.Attacker.PilotInfo.PilotName + " gains 1 Jam Token");

            Combat.Attacker.Tokens.AssignToken(
                typeof(JamToken),
                PerformCustomDiceCheck
            );
        }

        private void PerformCustomDiceCheck()
        {
            PerformDiceCheck(
                HostUpgrade.UpgradeInfo.Name + ": On hit or crit - gain 1 Jam Token",
                DiceKind.Attack,
                1,
                DiceCheckFinished,
                Triggers.FinishTrigger
            );
        }

        private void DiceCheckFinished()
        {
            if (DiceCheckRoll.Successes > 0)
            {
                Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + " Gain 1 Jam Token");
                HostShip.Tokens.AssignToken(
                    typeof(JamToken),
                    AbilityDiceCheck.ConfirmCheck
                );
            }
            else
            {
                AbilityDiceCheck.ConfirmCheck();
            }
        }
    }
}
