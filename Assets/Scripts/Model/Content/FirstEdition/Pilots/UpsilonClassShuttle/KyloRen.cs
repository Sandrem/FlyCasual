using Conditions;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.UpsilonClassShuttle
    {
        public class KyloRen : UpsilonClassShuttle
        {
            public KyloRen() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Kylo Ren",
                    6,
                    34,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.KyloRenPilotAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class KyloRenPilotAbility : GenericAbility
    {
        public GenericDamageCard AssignedDamageCard;
        public GenericShip ShipWithCondition;

        public override void ActivateAbility()
        {
            HostShip.OnAttackHitAsDefender += CheckKyloRenAbility;
            Phases.Events.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackHitAsDefender -= CheckKyloRenAbility;
            Phases.Events.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        protected virtual void CheckKyloRenAbility()
        {
            if (!IsAbilityUsed)
            {
                IsAbilityUsed = true;
                RegisterAbilityTrigger(TriggerTypes.OnAttackHit, AssignConditionToAttacker);
            }
        }

        protected virtual void AssignConditionToAttacker(object sender, EventArgs e)
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

        protected virtual void ShowPilotCrits()
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

        protected void SelectDamageCard(GenericDamageCard damageCard)
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

        protected void AssignConditions(GenericShip ship)
        {
            ShipWithCondition = ship;

            ship.Tokens.AssignCondition(typeof(IllShowYouTheDarkSide));
            ship.Tokens.AssignCondition(new IllShowYouTheDarkSideDamageCard(ship) { Tooltip = AssignedDamageCard.ImageUrl });

            ship.OnSufferCriticalDamage += SufferAssignedCardInstead;
            ship.OnShipIsDestroyed += RemoveConditionsOnDestroyed;
        }

        protected void RemoveConditions(GenericShip ship)
        {
            ShipWithCondition = null;
            ship.Tokens.RemoveCondition(typeof(IllShowYouTheDarkSide));
            ship.Tokens.RemoveCondition(typeof(IllShowYouTheDarkSideDamageCard));

            ship.OnSufferCriticalDamage -= SufferAssignedCardInstead;
            ship.OnShipIsDestroyed -= RemoveConditionsOnDestroyed;
        }

        protected void SufferAssignedCardInstead(object sender, EventArgs e, ref bool isSkipSufferDamage)
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

        protected void RemoveConditionsOnDestroyed(GenericShip ship, bool isFled)
        {
            AssignedDamageCard = null;
            RemoveConditions(ship);
        }

        protected class SelectPilotCritDecision : DecisionSubPhase { };
    }
}

namespace Conditions
{
    public class IllShowYouTheDarkSide : GenericToken
    {
        public IllShowYouTheDarkSide(GenericShip host) : base(host)
        {
            Name = "I'll Show You The Dark Side Condition";
            Temporary = false;
            Tooltip = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/conditions/ill-show-you-the-dark-side.png";
        }
    }

    public class IllShowYouTheDarkSideDamageCard : GenericToken
    {
        public IllShowYouTheDarkSideDamageCard(GenericShip host) : base(host)
        {
            Name = "I'll Show You The Dark Side Damage Card Condition";
            Temporary = false;
        }
    }
}
