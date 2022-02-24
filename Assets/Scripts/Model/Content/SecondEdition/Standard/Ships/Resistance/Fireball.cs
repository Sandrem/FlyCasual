using System;
using System.Collections.Generic;
using System.Linq;
using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship.CardInfo;
using SubPhases;
using Tokens;
using UnityEngine;

namespace Ship.SecondEdition.Fireball
{
    public class Fireball : GenericShip
    {
        public Fireball() : base()
        {
            ShipInfo = new ShipCardInfo25
            (
                "Fireball",
                BaseSize.Small,
                new FactionData
                (
                    new Dictionary<Faction, Type>
                    {
                        { Faction.Resistance, typeof(KazudaXiono) }
                    }
                ),
                new ShipArcsInfo(ArcType.Front, 2), 2, 6, 0,
                new ShipActionsInfo
                (
                    new ActionInfo(typeof(FocusAction)),
                    new ActionInfo(typeof(EvadeAction)),
                    new ActionInfo(typeof(BarrelRollAction)),
                    new ActionInfo(typeof(SlamAction))
                ),
                new ShipUpgradesInfo(),
                abilityText: "Setup: You are dealt 1 facedown damage card. After you perform a slam action, you may expose 1 damage card to remove 1 disarm token"
            );

            ShipAbilities.Add(new Abilities.SecondEdition.ExplosionWithWings());

            ModelInfo = new ShipModelInfo
            (
                "Fireball",
                "Fireball",
                new Vector3(-3.8f, 7.5f, 5.55f),
                1.6f
            );

            DialInfo = new ShipDialInfo
            (
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.TallonRoll, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.TallonRoll, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Complex)
            );

            SoundInfo = new ShipSoundInfo
            (
                new List<string>()
                {
                    "XWing-Fly1",
                    "XWing-Fly2",
                    "XWing-Fly3"
                },
                "XWing-Laser", 2
            );

            ShipIconLetter = '0';
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ExplosionWithWings : GenericAbility
    {
        public override string Name { get { return "Explosion with Wings"; } }

        public override void ActivateAbility()
        {
            Phases.Events.OnSetupStart += RegisterOwnTrigger;
            HostShip.OnActionIsPerformed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupStart -= RegisterOwnTrigger;
            HostShip.OnActionIsPerformed -= CheckAbility;
        }

        private void CheckAbility(GenericAction action)
        {
            if ((action is SlamAction) && (HostShip.Damage.GetFacedownCards().Count > 0))
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskToUseOwnAbility);
            }
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                "Explosion with Wings",
                NeverUseByDefault,
                StartExposeDamageCard,
                descriptionLong: "Do you want to expose your facedown damage card to remove 1 disarm token?",
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void StartExposeDamageCard(object sender, EventArgs e)
        {
            HostShip.Damage.ExposeRandomFacedownCard(RemoveDisarmToken);
        }

        private void RemoveDisarmToken()
        {
            if (HostShip.Tokens.HasToken<WeaponsDisabledToken>())
            {
                HostShip.Tokens.RemoveToken(typeof(WeaponsDisabledToken), DecisionSubPhase.ConfirmDecision);
            }
            else
            {
                DecisionSubPhase.ConfirmDecision();
            }
        }

        private void RegisterOwnTrigger()
        {
            RegisterAbilityTrigger(TriggerTypes.OnSetupStart, DealDamageToItself);
        }

        private void DealDamageToItself(object sender, EventArgs e)
        {
            if (!HostShip.UpgradeBar.HasUpgradeInstalled(typeof(UpgradesList.SecondEdition.KazsFireball)))
            {
                HostShip.SufferHullDamage(
                    false,
                    new DamageSourceEventArgs
                    {
                        Source = HostShip,
                        DamageType = DamageTypes.CardAbility
                    },
                    Triggers.FinishTrigger
                );
            }
            else
            {
                ShowShipCrits();
            }
        }

        protected void ShowShipCrits()
        {
            SelectShipCritDecision subphase = (SelectShipCritDecision)Phases.StartTemporarySubPhaseNew(
                "Select Damage Card",
                typeof(SelectShipCritDecision),
                Triggers.FinishTrigger
            );

            List<GenericDamageCard> ownDeck = DamageDecks.GetDamageDeck(HostShip.Owner.PlayerNo).Deck;
            foreach (var card in ownDeck.Where(n => n.Type == CriticalCardType.Ship))
            {
                Decision existingDecision = subphase.GetDecisions().Find(n => n.Name == card.Name);
                if (existingDecision == null)
                {
                    subphase.AddDecision(card.Name, delegate { SelectDamageCard(card); }, card.ImageUrl, 1);
                }
                else
                {
                    existingDecision.SetCount(existingDecision.Count + 1);
                }
            }

            subphase.DecisionViewType = DecisionViewTypes.ImagesDamageCard;

            subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;

            subphase.DescriptionShort = "Kaz's Fireball: Select Damage Card";

            subphase.RequiredPlayer = HostShip.Owner.PlayerNo;

            subphase.Start();
        }

        protected void SelectDamageCard(GenericDamageCard damageCard)
        {
            Messages.ShowInfo(damageCard.Name + " has been selected");

            DamageDeck ownDeck = DamageDecks.GetDamageDeck(HostShip.Owner.PlayerNo);
            ownDeck.RemoveFromDamageDeck(damageCard);
            ownDeck.ReShuffleDeck();

            Combat.CurrentCriticalHitCard = damageCard;
            HostShip.Damage.DealDrawnCard(DecisionSubPhase.ConfirmDecision);
        }

        protected class SelectShipCritDecision : DecisionSubPhase { };
    }
}