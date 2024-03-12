﻿using Abilities.SecondEdition;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class JekPorkinsSSP : T65XWing
        {
            public JekPorkinsSSP() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Jek Porkins",
                    "Red Six",
                    Faction.Rebel,
                    4,
                    5,
                    0,
                    isLimited: true,
                    abilityType: typeof(JekPorkinsSSPAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Torpedo,
                        UpgradeType.Astromech
                    },
                    tags: new List<Tags>
                    {
                        Tags.XWing
                    },
                    skinName: "Jek Porkins",
                    isStandardLayout: true
                );

                MustHaveUpgrades.Add(typeof(Predator));
                MustHaveUpgrades.Add(typeof(ProtonTorpedoes));
                MustHaveUpgrades.Add(typeof(R5D8));

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/jekporkins-swz106.png";

                PilotNameCanonical = "jekporkins-swz106";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class JekPorkinsSSPAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTokenIsAssigned += CheckAbilityConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsAssigned -= CheckAbilityConditions;
        }

        private void CheckAbilityConditions(GenericShip ship, GenericToken token)
        {
            if (token is StressToken)
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, AskToUsePilotAbility);
            }
        }

        private void AskToUsePilotAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                ShouldUseAbility,
                RemoveStressAndRollDice,
                descriptionLong: "Do you want to remove Stress Token and to roll 1 attack die? (On a hit result, deal 1 Facedown Damage card to this ship)",
                imageHolder: HostShip
            );
        }

        private bool ShouldUseAbility()
        {
            return HostShip.State.HullCurrent > 1;
        }

        private void RemoveStressAndRollDice(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.Tokens.RemoveToken(typeof(StressToken), StartRollDiceSubphase);
        }

        private void StartRollDiceSubphase()
        {
            PerformDiceCheck(
                HostShip.PilotInfo.PilotName + ": Facedown damage card on hit",
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

        private void SufferNegativeEffect(Action callback)
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