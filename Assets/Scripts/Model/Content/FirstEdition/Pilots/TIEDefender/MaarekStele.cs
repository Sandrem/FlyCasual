using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEDefender
    {
        public class MaarekStele : TIEDefender
        {
            public MaarekStele() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Maarek Stele",
                    7,
                    35,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.MaarekSteleAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class MaarekSteleAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnFaceupCritCardReadyToBeDealtGlobal += MaarekStelePilotAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnFaceupCritCardReadyToBeDealtGlobal -= MaarekStelePilotAbility;
        }

        private void MaarekStelePilotAbility(GenericShip ship, GenericDamageCard crit, EventArgs e)
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

        private static void ShowDecision(object sender, EventArgs e)
        {
            Phases.StartTemporarySubPhaseOld(
                "Ability of Maarek Stele",
                typeof(CritToDealDecisionSubPhase),
                Triggers.FinishTrigger
            );
        }

    }
}

namespace SubPhases
{
    public class CritToDealDecisionSubPhase : DecisionSubPhase
    {
        private List<GenericDamageCard> criticalHitCardsToChoose = new List<GenericDamageCard>();

        public override void PrepareDecision(Action callBack)
        {
            InfoText = "Select Critical Hit card to deal";

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
                    }
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

        private void AddToCriticalHitCardsToChoose(EventArgs e)
        {
            criticalHitCardsToChoose.Add(Combat.CurrentCriticalHitCard);
        }

        private void DealCard(GenericDamageCard critCard)
        {
            Combat.CurrentCriticalHitCard = critCard;
            ConfirmDecision();
        }

    }
}