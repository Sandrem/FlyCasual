using Ship;
using Upgrade;
using UnityEngine;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Conditions;

namespace UpgradesList.FirstEdition
{
    public class KyloRen : GenericUpgrade
    {
        public KyloRen() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Kylo Ren",
                UpgradeType.Crew,
                cost: 3,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Imperial),
                abilityType: typeof(Abilities.FirstEdition.KyloRenCrewAbility)
            );

            Avatar = new AvatarInfo(Faction.Imperial, new Vector2(36, 0));
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class KyloRenCrewAbility : GenericAbility
    {
        public GenericDamageCard AssignedDamageCard;
        public GenericShip ShipWithCondition;

        public override void ActivateAbility()
        {
            HostShip.OnGenerateActions += KyloRenCrewAddAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateActions -= KyloRenCrewAddAction;
        }

        private void KyloRenCrewAddAction(GenericShip host)
        {
            ActionsList.GenericAction action = new ActionsList.KyloRenCrewAction()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = HostShip,
                DoAction = DoKyloRenAction
            };
            host.AddAvailableAction(action);
        }

        private void DoKyloRenAction()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAbilityDirect, SelectShip);

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, Phases.CurrentSubPhase.CallBack);
        }

        private void SelectShip(object sender, EventArgs e)
        {
            // TODO: Skip/Wrong target - revert

            SelectTargetForAbility(
                AssignConditionToTarget,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostUpgrade.UpgradeInfo.Name,
                "Choose a ship to assign\n\"I'll Show You The Dark Side\" Condition",
                HostUpgrade
            );
        }

        private void AssignConditionToTarget()
        {
            Sounds.PlayShipSound("Ill-Show-You-The-Dark-Side");

            if (AssignedDamageCard == null)
            {
                // If condition is not in play - select card to assign
                SelectShipSubPhase.FinishSelectionNoCallback();
                ShowPilotCrits();
            }
            else
            {
                // If condition is in play - reassing only
                RemoveConditions(ShipWithCondition);
                AssignConditions(TargetShip);
                SelectShipSubPhase.FinishSelection();
            }
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.Enemy }) && FilterTargetsByRange(ship, 1, 3);
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.UpgradeInfo.Cost);
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

            selectPilotCritSubphase.DecisionViewType = DecisionViewTypes.ImagesDamageCard;

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
            DamageDeck opponentDamageDeck = DamageDecks.GetDamageDeck(TargetShip.Owner.PlayerNo);
            opponentDamageDeck.RemoveFromDamageDeck(damageCard);
            opponentDamageDeck.ReShuffleDeck();
            AssignConditions(TargetShip);

            DecisionSubPhase.ConfirmDecision();
        }

        private void AssignConditions(GenericShip ship)
        {
            ShipWithCondition = ship;

            ship.Tokens.AssignCondition(typeof(IllShowYouTheDarkSide));
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

namespace ActionsList
{
    public class KyloRenCrewAction : GenericAction
    {
        public KyloRenCrewAction()
        {
            Name = DiceModificationName = "Kylo Ren: Assign condition";
        }

        public override int GetActionPriority()
        {
            return 0;
        }
    }
}