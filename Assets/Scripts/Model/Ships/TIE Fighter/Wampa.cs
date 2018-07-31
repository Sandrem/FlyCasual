using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;
using Tokens;
using RuleSets;

namespace Ship
{
    namespace TIEFighter
    {
        public class Wampa : TIEFighter, ISecondEditionPilot
        {
            public Wampa() : base()
            {
                PilotName = "\"Wampa\"";
                PilotSkill = 1;
                Cost = 30;

                IsUnique = true;

                UsesCharges = true;
                MaxCharges = 1;

                PilotRuleType = typeof(SecondEdition);
                PilotAbilities.Add(new Abilities.WampaAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not required
            }
        }
    }
}

namespace Abilities
{
    public class WampaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += RegisterWampaAbility;
            HostShip.OnAttackFinishAsDefender += RegisterWampaDefenseAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= RegisterWampaAbility;
            HostShip.OnAttackFinishAsDefender -= RegisterWampaDefenseAbility;
        }

        // Offensive portion
        private void RegisterWampaAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackStart, ShowDecision);
        }

        private void ShowDecision(object sender, System.EventArgs e)
        {
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
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice -= WampaAddAttackDice;
        }

        // defensive portion
        private void RegisterWampaDefenseAbility(GenericShip ship)
        {
            if(HostShip.Charges > 0)
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, delegate
                {
                    Messages.ShowError("Wampa lost a charge after defending!");
                    HostShip.RemoveCharge(Triggers.FinishTrigger);
                });
        }
    }
}