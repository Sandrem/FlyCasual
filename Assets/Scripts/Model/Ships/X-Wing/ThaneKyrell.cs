using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using ActionsList.SecondEdition;
using System;
using SubPhases;
using RuleSets;

namespace Ship
{
    namespace XWing
    {
        public class ThaneKyrell : XWing, ISecondEditionPilot
        {
            public ThaneKyrell() : base()
            {
                PilotName = "Thane Kyrell";
                PilotSkill = 5;
                Cost = 48;

                IsUnique = true;
                PilotRuleType = typeof(SecondEdition);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
                PilotAbilities.Add(new Abilities.SecondEdition.ThaneKyrellAbilitySE());
            }

            public void AdaptPilotToSecondEdition()
            {
            }
        }
    }
}
namespace Abilities.SecondEdition
{
    public class ThaneKyrellAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddThaneKyrellAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddThaneKyrellAbility;
        }

        protected virtual void AddThaneKyrellAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new ThaneKyrellDiceModificationSE() { Host = HostShip });
        }
    }
}

namespace ActionsList.SecondEdition
{
    public class ThaneKyrellDiceModificationSE : GenericAction
    {
        public ThaneKyrellDiceModificationSE()
        {
            Name = DiceModificationName = "Thane Kyrell's ability";
        }

        public override bool IsDiceModificationAvailable()
        {
            if (Combat.AttackStep != CombatStep.Attack) return false;
            if (Combat.DiceRollAttack.CriticalSuccesses == 0 && Combat.DiceRollAttack.Successes == 0 && Combat.DiceRollAttack.Focuses == 0) return false;

            return true;
        }

        public override void ActionEffect(Action callback)
        {
            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = "Thane Kyrell's ability",
                    TriggerOwner = Host.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnAbilityDirect,
                    EventHandler = StartSubphase
                }
            );

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, callback);
        }

        private void StartSubphase(object sender, System.EventArgs e)
        {
            var spendDiceSubPhase = Phases.StartTemporarySubPhaseNew<ThaneKyrellDecisionSubPhase>(Name, Triggers.FinishTrigger);
            spendDiceSubPhase.ShowSkipButton = true;
            spendDiceSubPhase.OnSkipButtonIsPressed = DecisionSubPhase.ConfirmDecision;
            spendDiceSubPhase.Start();
        }
    }
}

namespace SubPhases
{
    public class ThaneKyrellDecisionSubPhase : SpendDiceResultDecisionSubPhase
    {
        protected override void PrepareDiceResultEffects()
        {
            InfoText = Selection.ActiveShip.PilotName + ": " + "Select a result to spend.";
            DecisionOwner = Selection.ActiveShip.Owner;

            AddSpendDiceResultEffect(DieSide.Crit, "Spend a critical result to flip a damage card.", delegate { SpendResultToFlip(DieSide.Crit); });
            AddSpendDiceResultEffect(DieSide.Success, "Spend a hit result to flip a damage card.", delegate { SpendResultToFlip(DieSide.Success); });
            AddSpendDiceResultEffect(DieSide.Focus, "Spend a focus result to flip a damage card.", delegate { SpendResultToFlip(DieSide.Focus); });
        }

        private void SpendResultToFlip(DieSide side)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            Combat.DiceRollAttack.RemoveType(side);
            //Combat.DiceRollAttack.OrganizeDicePositions();

            var damageCardSubPhase = Phases.StartTemporarySubPhaseNew<SelectDamageCardDecisionSubPhase>(Name, Triggers.FinishTrigger);
            damageCardSubPhase.RegisterDamageCardHandler(HandleDamageCard);
            damageCardSubPhase.Start();
        }

        private void HandleDamageCard(GenericDamageCard card)
        {
            card.Expose(DecisionSubPhase.ConfirmDecision);
        }
    }
}