using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;

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
                PilotSkill = 7;
                Cost = 27;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.MaarekSteleAbility());
            }
        }
    }
}

namespace Abilities
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

        private void MaarekStelePilotAbility(GenericShip ship, CriticalHitCard.GenericCriticalHit crit, EventArgs e)
        {
            if ((e as DamageSourceEventArgs) == null) return;
            else if ((((e as DamageSourceEventArgs).Source) as GenericShip).ShipId == HostShip.ShipId)
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
                typeof(SubPhases.CritToDealDecisionSubPhase),
                Triggers.FinishTrigger
            );
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
            InfoText = "Select Critical Hit card to deal";

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

            DefaultDecision = Combat.CurrentCriticalHitCard.Name;

            callBack();
        }

        private void DealCard(CriticalHitCard.GenericCriticalHit critCard)
        {
            Combat.CurrentCriticalHitCard = critCard;
            ConfirmDecision();
        }

    }

}
