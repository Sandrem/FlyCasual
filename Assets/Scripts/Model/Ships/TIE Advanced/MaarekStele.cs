using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PROBLEMS: 
// 1) Compare results and end combat are delayed
// 2) 2 damage - 2 windows at once

// TODO:
// 1) Deal Damage Card should wait for Select Crit Card

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
                AddUpgradeSlot(Upgrade.UpgradeSlot.Elite);
            }

            public override void InitializePilot()
            {
                base.InitializePilot();

                GenericShip.OnFaceupCritCardReadyToBeDealtGlobal += MaarekStelePilotAbility;
            }

            private void MaarekStelePilotAbility(GenericShip ship, ref CriticalHitCard.GenericCriticalHit crit, EventArgs e = null)
            {
                if (e == null) return;
                else if ((e as DamageSourceEventArgs) == null) return;
                else if ((((e as DamageSourceEventArgs).Source) as GenericShip) == null) return;
                else if ((((e as DamageSourceEventArgs).Source) as GenericShip).PilotName == this.PilotName)
                {
                    if ((e as DamageSourceEventArgs).DamageType == DamageTypes.ShipAttack)
                    {
                        //Debug.Log("+++ SUBSCRIBED!!!");
                        //OldTriggers.AddTrigger("Maarek Stele", TriggerTypes.OnFaceupCritCardReadyToBeDealt, ShowDecision, Combat.Defender, Combat.Attacker.Owner.PlayerNo);
                    }
                }
            }

            private static void ShowDecision(object sender, EventArgs e)
            {
                Debug.Log("+++ FIRED!!!");
                Phases.StartTemporarySubPhase("Ability of Maarek Stele", typeof(SubPhases.CritToDealDecisionSubPhase));
            }

        }
    }
}

namespace SubPhases
{

    public class CritToDealDecisionSubPhase : DecisionSubPhase
    {
        private List<CriticalHitCard.GenericCriticalHit> criticalHitCardsToChoose = new List<CriticalHitCard.GenericCriticalHit>();
        private List<EventHandler> delegatesToResolve = new List<EventHandler>();

        public override void Prepare()
        {
            infoText = "Select Critical Hit card to deal";

            criticalHitCardsToChoose.Add(Combat.CurrentCriticalHitCard);
            for (int i = 0; i < 2; i++)
            {
                criticalHitCardsToChoose.Add(CriticalHitsDeck.GetCritCard());
            }

            delegatesToResolve.Add(DealFirst);
            delegatesToResolve.Add(DealSecond);
            delegatesToResolve.Add(DealThird);

            for (int i = 0; i < 3; i++)
            {
                string newName = AddDecision(criticalHitCardsToChoose[i].Name, delegatesToResolve[i]);
                AddTooltip(newName, criticalHitCardsToChoose[i].ImageUrl);
            }

            defaultDecision = Combat.CurrentCriticalHitCard.Name;
        }

        private void ConfirmDecision()
        {
            Debug.Log("+++ CONFIRMED!!!");
            Phases.FinishSubPhase(this.GetType());
        }

        private void DealFirst(object sender, EventArgs e)
        {
            Combat.CurrentCriticalHitCard = criticalHitCardsToChoose[0];
            Phases.FinishSubPhase(this.GetType());
        }

        private void DealSecond(object sender, EventArgs e)
        {
            Combat.CurrentCriticalHitCard = criticalHitCardsToChoose[1];
            Phases.FinishSubPhase(this.GetType());
        }

        private void DealThird(object sender, EventArgs e)
        {
            Combat.CurrentCriticalHitCard = criticalHitCardsToChoose[2];
            Phases.FinishSubPhase(this.GetType());
        }

    }

}
