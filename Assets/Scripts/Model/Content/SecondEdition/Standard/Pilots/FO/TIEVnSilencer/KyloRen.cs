using Conditions;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEVnSilencer
    {
        public class KyloRen : TIEVnSilencer
        {
            public KyloRen() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Kylo Ren",
                    "Tormented Apprentice",
                    Faction.FirstOrder,
                    5,
                    7,
                    16,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.KyloRenPilotAbility),
                    force: 2,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.ForcePower,
                        UpgradeType.Torpedo
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie,
                        Tags.DarkSide
                    }
                );

                ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/71dbde337b9ff5aab897781d40d8f653.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KyloRenPilotAbility : Abilities.FirstEdition.KyloRenPilotAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsDefender += CheckKyloRenAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsDefender -= CheckKyloRenAbility;
        }

        protected void CheckKyloRenAbility(GenericShip ship)
        {
            if (HostShip.State.Force > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskKyloRenAbility);
            }
        }

        protected void AskKyloRenAbility(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                AlwaysUseByDefault,
                AssignConditionToAttacker,
                descriptionLong: "Do you want to spend 1 Force to assign the \"I'll Show You The Dark Side\" condition to the attacker?",
                imageHolder: HostShip
            );
        }

        protected override void AssignConditionToAttacker(object sender, System.EventArgs e)
        {
            Sounds.PlayShipSound("Ill-Show-You-The-Dark-Side");

            if (AssignedDamageCard == null)
            {
                // If condition is not in play - select card to assign
                HostShip.State.SpendForce(1, ShowPilotCrits);
            }
            else
            {
                // If condition is in play - reassing only
                RemoveConditions(ShipWithCondition);
                AssignConditions(Combat.Attacker);
                HostShip.State.SpendForce(1, DecisionSubPhase.ConfirmDecision);
            }
        }

        protected override void ShowPilotCrits()
        {
            SelectPilotCritDecision selectPilotCritSubphase = (SelectPilotCritDecision)Phases.StartTemporarySubPhaseNew(
                "Select Damage Card",
                typeof(SelectPilotCritDecision),
                DecisionSubPhase.ConfirmDecision
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

            selectPilotCritSubphase.DescriptionShort = "Kylo Ren: Select Damage Card";

            selectPilotCritSubphase.RequiredPlayer = HostShip.Owner.PlayerNo;

            selectPilotCritSubphase.Start();
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

            selectPilotCritSubphase.DescriptionShort = "Kylo Ren";
            selectPilotCritSubphase.DescriptionLong = "Select a Damage Card to assign";
            selectPilotCritSubphase.ImageSource = HostShip;

            selectPilotCritSubphase.RequiredPlayer = HostShip.Owner.PlayerNo;

            selectPilotCritSubphase.Start();
        }

        protected void SelectDamageCard(GenericDamageCard damageCard)
        {
            Messages.ShowInfo(damageCard.Name + " has been selected");

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

                isSkipSufferDamage = true;

                GenericShip ship = ShipWithCondition;
                Messages.ShowInfo("Kylo Ren's premonition comes true: " + ship.PilotInfo.PilotName + " receives " + AssignedDamageCard.Name);
                Combat.CurrentCriticalHitCard = AssignedDamageCard;

                AssignedDamageCard = null;
                RemoveConditions(ship);

                ship.ProcessDrawnDamageCard(e, Triggers.FinishTrigger);
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
            Name = ImageName = "I'll Show You The Dark Side Condition";
            Temporary = false;
            Tooltip = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/conditions/ill-show-you-the-dark-side.png";
        }
    }

    public class IllShowYouTheDarkSideDamageCard : GenericToken
    {
        public IllShowYouTheDarkSideDamageCard(GenericShip host) : base(host)
        {
            Name = ImageName = "I'll Show You The Dark Side Damage Card Condition";
            Temporary = false;
        }
    }
}