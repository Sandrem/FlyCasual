using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Abilities;
using System;
using SubPhases;
using DamageDeckCard;
using System.Linq;
using Ship;
using Conditions;

namespace Ship
{
    namespace UpsilonShuttle
    {
        public class KyloRenUpsilon : UpsilonShuttle
        {
            public KyloRenUpsilon() : base()
            {
                PilotName = "Kylo Ren";
                PilotSkill = 6;
                Cost = 34;

                IsUnique = true;

                UpgradeBar.AddSlot(UpgradeType.Elite);

                PilotAbilities.Add(new KyloRenPilotAbility());
            }
        }
    }
}

namespace Abilities
{
    public class KyloRenPilotAbility: GenericAbility
    {
        public GenericDamageCard AssignedDamageCard;
        public GenericShip ShipWithCondition;

        public override void ActivateAbility()
        {
            HostShip.OnAttackHitAsDefender += CheckKyloRenAbility;
            Phases.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackHitAsDefender -= CheckKyloRenAbility;
            Phases.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void CheckKyloRenAbility()
        {
            if (!IsAbilityUsed)
            {
                IsAbilityUsed = true;
                RegisterAbilityTrigger(TriggerTypes.OnAttackHit, AssignConditionToAttacker);
            }
        }

        private void AssignConditionToAttacker(object sender, EventArgs e)
        {
            Sounds.PlayShipSound("Ill-Show-You-The-Dark-Side");

            if (AssignedDamageCard == null)
            {
                // If condition is not in play - select card to assign
                ShowPilotCrits();
            }
            else
            {
                // If condition is in play - reassing only
                RemoveConditions(ShipWithCondition);
                AssignConditions(Combat.Attacker);
                Triggers.FinishTrigger();
            }
        }

        private void ShowPilotCrits()
        {
            SelectPilotCritDecision selectPilotCritSubphase = (SelectPilotCritDecision)Phases.StartTemporarySubPhaseNew(
                "Select Damage Card",
                typeof(SelectPilotCritDecision),
                Triggers.FinishTrigger
            );

            List<GenericDamageCard> opponentDeck = DamageDecks.GetDamageDeck(Roster.AnotherPlayer(HostShip.Owner.PlayerNo)).Deck;
            foreach (var card in opponentDeck.Where(n => n.Type == CriticalCardType.Pilot))
            {
                Decision existingDecision = selectPilotCritSubphase.GetDecisions().Find(n => n.Name == card.Name);
                if (existingDecision == null)
                {
                    selectPilotCritSubphase.AddDecision(card.Name, delegate { SelectDamageCard(card); }, card.ImageUrl, 1);
                }
                else
                {
                    existingDecision.SetCount(existingDecision.Count + 1);
                }
            }

            selectPilotCritSubphase.DecisionViewType = DecisionViewTypes.ImageButtons;

            selectPilotCritSubphase.DefaultDecisionName = selectPilotCritSubphase.GetDecisions().First().Name;

            selectPilotCritSubphase.InfoText = "Kylo Ren: Select Damage Card";

            selectPilotCritSubphase.RequiredPlayer = HostShip.Owner.PlayerNo;

            selectPilotCritSubphase.Start();
        }

        private void SelectDamageCard(GenericDamageCard damageCard)
        {
            Messages.ShowInfo("Card is selected: " + damageCard.Name);

            AssignedDamageCard = damageCard;
            AssignedDamageCard.IsFaceup = true;
            DamageDeck opponentDamageDeck = DamageDecks.GetDamageDeck(Combat.Attacker.Owner.PlayerNo);
            opponentDamageDeck.RemoveFromDamageDeck(damageCard);
            opponentDamageDeck.ReShuffleDeck();
            AssignConditions(Combat.Attacker);

            DecisionSubPhase.ConfirmDecision();
        }

        private void AssignConditions(GenericShip ship)
        {
            ShipWithCondition = ship;

            ship.Tokens.AssignCondition(new IllShowYouTheDarkSide(ship));
            ship.Tokens.AssignCondition(new IllShowYouTheDarkSideDamageCard(ship) { Tooltip = AssignedDamageCard.ImageUrl });

            ship.OnSufferCriticalDamage += SufferAssignedCardInstead;
            ship.OnShipIsDestroyed += RemoveConditionsOnDestroyed;
        }

        private void RemoveConditions(GenericShip ship)
        {
            ShipWithCondition = null;
            ship.Tokens.RemoveCondition(typeof(IllShowYouTheDarkSide));
            ship.Tokens.RemoveCondition(typeof(IllShowYouTheDarkSideDamageCard));

            ship.OnSufferCriticalDamage -= SufferAssignedCardInstead;
            ship.OnShipIsDestroyed -= RemoveConditionsOnDestroyed;
        }

        private void SufferAssignedCardInstead(object sender, EventArgs e, ref bool isSkipSufferDamage)
        {
            if ((e as DamageSourceEventArgs).DamageType == DamageTypes.ShipAttack)
            {
                Messages.ShowInfo("Kylo Ren: Assigned card is dealt instead");

                isSkipSufferDamage = true;

                GenericShip ship = ShipWithCondition;
                Combat.CurrentCriticalHitCard = AssignedDamageCard;

                AssignedDamageCard = null;
                RemoveConditions(ship);

                ship.ProcessDrawnDamageCard(e);
            }
        }

        private void RemoveConditionsOnDestroyed(GenericShip ship, bool isFled)
        {
            AssignedDamageCard = null;
            RemoveConditions(ship);
        }

        private class SelectPilotCritDecision : DecisionSubPhase { };
    }
}
