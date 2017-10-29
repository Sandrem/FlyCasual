using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEAdvanced
    {
        public class MaarekStele : TIEAdvanced
        {
            public MaarekStele() : base()
            {
                IsHidden = true;

                PilotName = "Maarek Stele";
                ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/4/41/Maarek_Stele.png";
                IsUnique = true;
                PilotSkill = 7;
                Cost = 27;
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }

            public override void InitializePilot()
            {
                base.InitializePilot();

                OnFaceupCritCardReadyToBeDealtGlobal += MaarekStelePilotAbility;
                OnDestroyed += RemoveMaarekSteleAbility;
            }

            private void MaarekStelePilotAbility(GenericShip ship, CriticalHitCard.GenericCriticalHit crit, EventArgs e)
            {
                if ((e as DamageSourceEventArgs) == null) return;
                else if ((((e as DamageSourceEventArgs).Source) as GenericShip) == this)
                {
                    if ((e as DamageSourceEventArgs).DamageType == DamageTypes.ShipAttack)
                    {
                        Triggers.RegisterTrigger(
                            new Trigger() {
                                Name = "Maarker Stele ability",
                                TriggerType = TriggerTypes.OnFaceupCritCardReadyToBeDealt,
                                TriggerOwner = ((e as DamageSourceEventArgs).Source as GenericShip).Owner.PlayerNo,
                                EventHandler = ShowDecision
                            }
                        );
                    }
                }
            }

            private static void ShowDecision(object sender, EventArgs e)
            {
                Phases.StartTemporarySubPhase(
                    "Ability of Maarek Stele",
                    typeof(SubPhases.CritToDealDecisionSubPhase),
                    Triggers.FinishTrigger
                );
            }

            private void RemoveMaarekSteleAbility(GenericShip ship)
            {
                OnFaceupCritCardReadyToBeDealtGlobal -= MaarekStelePilotAbility;
                OnDestroyed -= RemoveMaarekSteleAbility;
            }

        }
    }
}

namespace SubPhases
{

    public class CritToDealDecisionSubPhase : DecisionSubPhase
    {
        private List<CriticalHitCard.GenericCriticalHit> criticalHitCardsToChoose = new List<CriticalHitCard.GenericCriticalHit>();

        public override void PrepareDecision(Action callBack)
        {
            infoText = "Select Critical Hit card to deal";

            criticalHitCardsToChoose.Add(Combat.CurrentCriticalHitCard);
            for (int i = 0; i < 2; i++)
            {
                //criticalHitCardsToChoose.Add(CriticalHitsDeck.GetCritCard());
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

            defaultDecision = Combat.CurrentCriticalHitCard.Name;

            callBack();
        }

        private void DealCard(CriticalHitCard.GenericCriticalHit critCard)
        {
            Combat.CurrentCriticalHitCard = critCard;
            ConfirmDecision();
        }

        private void ConfirmDecision()
        {
            Tooltips.EndTooltip();

            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }

    }

}
