using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;
using RuleSets;
using Tokens;
using SubPhases;

namespace Ship
{
    namespace XWing
    {
        public class JekPorkins : XWing, ISecondEditionPilot
        {
            public JekPorkins() : base()
            {
                PilotName = "Jek Porkins";
                PilotSkill = 7;
                Cost = 26;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.JekPorkinsAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 4;
                Cost = 46;

                SEImageNumber = 5;

                PilotAbilities.RemoveAll(a => a is Abilities.JekPorkinsAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.JekPorkinsAbilitySE());
            }
        }
    }
}

namespace Abilities
{
    public class JekPorkinsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTokenIsAssigned += CheckAbilityConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsAssigned -= CheckAbilityConditions;
        }

        private void CheckAbilityConditions(GenericShip ship, Type tokenType)
        {
            if (tokenType == typeof(StressToken))
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, AskToUsePilotAbility);
            }
        }

        private void AskToUsePilotAbility(object sender, EventArgs e)
        {
            AskToUseAbility(ShouldUseAbility, RemoveStressAndRollDice);
        }

        private bool ShouldUseAbility()
        {
            return HostShip.Hull > 1;
        }

        private void RemoveStressAndRollDice(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.Tokens.RemoveToken(typeof(StressToken), StartRollDiceSubphase);
        }

        private void StartRollDiceSubphase()
        {
            PerformDiceCheck(
                "Jek Porkins: Facedown damage card on hit",
                DiceKind.Attack,
                1,
                FinishAction,
                Triggers.FinishTrigger
            );
        }

        private void FinishAction()
        {
            if (DiceCheckRoll.RegularSuccesses > 0)
            {
                SufferNegativeEffect(AbilityDiceCheck.ConfirmCheck);
            }
            else
            {
                AbilityDiceCheck.ConfirmCheck();
            }
        }

        protected virtual void SufferNegativeEffect(Action callback)
        {
            HostShip.Damage.SufferFacedownDamageCard(
                new DamageSourceEventArgs()
                {
                    Source = HostShip,
                    DamageType = DamageTypes.CardAbility
                },
                callback
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class JekPorkinsAbilitySE : JekPorkinsAbility
    {
        protected override void SufferNegativeEffect(Action callback)
        {
            DamageSourceEventArgs damageArgs = new DamageSourceEventArgs()
            {
                Source = HostShip,
                DamageType = DamageTypes.CardAbility
            };

            HostShip.Damage.TryResolveDamage(1, damageArgs, callback);
        }
    }
}