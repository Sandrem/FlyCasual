using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship
{
    namespace SecondEdition.TIEAdvancedX1
    {
        public class MaarekSteleSSP : TIEAdvancedX1
        {
            public MaarekSteleSSP() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Maarek Stele",
                    "Servant of the Empire",
                    Faction.Imperial,
                    5,
                    5,
                    0,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.MaarekSteleSSPAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    isStandardLayout: true
                );

                MustHaveUpgrades.Add(typeof(Elusive));
                MustHaveUpgrades.Add(typeof(Outmaneuver));
                MustHaveUpgrades.Add(typeof(AfterBurners));

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/maarekstele-swz105.png";

                PilotNameCanonical = "maarekstele-swz105";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MaarekSteleSSPAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnFaceupCritCardReadyToBeDealtGlobal += MaarekSteleSSPPilotAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnFaceupCritCardReadyToBeDealtGlobal -= MaarekSteleSSPPilotAbility;
        }

        private void MaarekSteleSSPPilotAbility(GenericShip ship, GenericDamageCard crit, EventArgs e)
        {
            if ((e as DamageSourceEventArgs) == null) return;

            GenericShip damageSourceShip = (e as DamageSourceEventArgs).Source as GenericShip;
            if (damageSourceShip == null) return;

            if (damageSourceShip.ShipId == HostShip.ShipId)
            {
                if ((e as DamageSourceEventArgs).DamageType == DamageTypes.ShipAttack)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnFaceupCritCardReadyToBeDealt, ShowDecision);
                }
            }
        }

        private void ShowDecision(object sender, EventArgs e)
        {
            SSPCritToDealDecisionSubPhase subphase = Phases.StartTemporarySubPhaseNew<SSPCritToDealDecisionSubPhase>(
                HostShip.PilotInfo.PilotName,
                Triggers.FinishTrigger
            );

            subphase.DescriptionShort = HostShip.PilotInfo.PilotName;
            subphase.DescriptionLong = "Select Critical Hit card to deal";
            subphase.ImageSource = HostShip;

            subphase.Start();
        }

    }
}

namespace SubPhases
{
    public class SSPCritToDealDecisionSubPhase : DecisionSubPhase
    {
        private List<GenericDamageCard> criticalHitCardsToChoose = new List<GenericDamageCard>();

        public override void PrepareDecision(Action callBack)
        {
            DescriptionLong = "Select Critical Hit card to deal";

            criticalHitCardsToChoose.Add(Combat.CurrentCriticalHitCard);
            for (int i = 0; i < 2; i++)
            {
                DamageDecks.GetDamageDeck(Combat.Attacker.Owner.PlayerNo).DrawDamageCard(
                    true,
                    AddToCriticalHitCardsToChoose,
                    new DamageSourceEventArgs()
                    {
                        Source = Combat.Attacker,
                        DamageType = DamageTypes.ShipAttack
                    },
                    delegate { }
                );
            }

            foreach (var critCard in criticalHitCardsToChoose)
            {
                AddDecision(
                    critCard.Name,
                    delegate { DealCard(critCard); }
                );
                AddTooltip(
                    critCard.Name,
                    critCard.ImageUrl
                );
            }

            DefaultDecisionName = Combat.CurrentCriticalHitCard.Name;

            DecisionViewType = DecisionViewTypes.ImagesDamageCard;

            callBack();
        }

        private void AddToCriticalHitCardsToChoose(EventArgs e, Action callback)
        {
            criticalHitCardsToChoose.Add(Combat.CurrentCriticalHitCard);
            callback();
        }

        private void DealCard(GenericDamageCard critCard)
        {
            Combat.CurrentCriticalHitCard = critCard;
            ConfirmDecision();
        }

    }
}
