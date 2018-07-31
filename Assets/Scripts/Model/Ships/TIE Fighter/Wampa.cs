using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System.Linq;
using Abilities;
using RuleSets;
using System;

namespace Ship
{
    namespace TIEFighter
    {
        // For now Wampa is SE only, sorry boys!
        public class Wampa : TIEFighter, ISecondEditionPilot
        {
            public Wampa() : base()
            {
                PilotName = "Wampa";
                PilotSkill = 1;
                Cost = 30;

                IsUnique = true;

                PilotRuleType = typeof(SecondEdition);
                UsesCharges = true;
                MaxCharges = 1;

                PilotAbilities.Add(new Abilities.SecondEdition.WampaAbilitySE());
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not required
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WampaAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += RegisterWampaAbility;
            HostShip.OnAttackFinishAsDefender += RegisterWampaDefendAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= RegisterWampaAbility;
            HostShip.OnAttackFinishAsDefender -= RegisterWampaDefendAbility;
        }

        private void RegisterWampaAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackStart, ShowDecision);
        }

        private void RegisterWampaDefendAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, RemoveCharge);
        }

        private void RemoveCharge(object sender, EventArgs e)
        {
            if (HostShip.Charges > 0)
            {
                Messages.ShowError("Wampa lost a charge after defending!");
                HostShip.RemoveCharge(Triggers.FinishTrigger);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void ShowDecision(object sender, System.EventArgs e)
        {
            // check if this card has a charge left
            if (HostShip.Charges > 0)
            {
                // give user the option to use ability
                AskToUseAbility(AlwaysUseByDefault, UseAbility);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice += WampaAddAttackDice;
            HostShip.RemoveCharge(SubPhases.DecisionSubPhase.ConfirmDecision);
        }

        private void WampaAddAttackDice(ref int value)
        {
            value++;
            HostShip.AfterGotNumberOfAttackDice -= WampaAddAttackDice;
        }
    }
}