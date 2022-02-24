using Ship;
using SubPhases;
using System;
using System.Linq;
using Tokens;
using UnityEngine;
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

            Avatar = new AvatarInfo(
                Faction.Scum,
                new Vector2(369, 7)
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
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                UseOwnAbility,
                descriptionLong: "Do you want to spend a lock you have on the attacker to roll 1 attack die? (If you do, the attacker gains 1 Jam Token. Then, on a \"hit\" or \"crit\" result, gain 1 Jam Token)",
                imageHolder: HostUpgrade
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
                new JamToken(Combat.Attacker, HostShip.Owner),
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
                    new JamToken(HostShip, HostShip.Owner),
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
