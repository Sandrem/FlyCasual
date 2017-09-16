using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class R5Astromech : GenericUpgrade
    {

        public R5Astromech() : base()
        {
            Type = UpgradeType.Astromech;
            Name = ShortName = "R5 Astromech";
            Cost = 1;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            Host = host;
            base.AttachToShip(host);

            Phases.OnEndPhaseStart += RegisterR5AstromechAbility;
        }

        private void RegisterR5AstromechAbility()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "R5 Astromech: Flip facedown Ship Crit",
                TriggerOwner = Host.Owner.PlayerNo,
                TriggerType = TriggerTypes.OnEndPhaseStart,
                EventHandler = R5AstromechAbility
            });
        }

        private void R5AstromechAbility(object sender, System.EventArgs e)
        {
            Selection.ActiveShip = Host;

            List<CriticalHitCard.GenericCriticalHit> shipCritsList = Host.GetAssignedCritCards().Where(n => n.Type == CriticalCardType.Ship).ToList();

            if (shipCritsList.Count == 1)
            {
                Selection.ActiveShip.FlipFacedownFaceupDamageCard(shipCritsList.First());
                Triggers.FinishTrigger();
            }
            else if (shipCritsList.Count > 1)
            {
                Phases.StartTemporarySubPhase(
                    "R5 Astromech: Select faceup ship Crit",
                    typeof(SubPhases.R5AstromechDecisionSubPhase),
                    delegate () { Triggers.FinishTrigger(); }
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

    }

}

namespace SubPhases
{

    public class R5AstromechDecisionSubPhase : DecisionSubPhase
    {

        public override void Prepare()
        {
            infoText = "R5 Astromech: Select faceup ship Crit";

            foreach (var shipCrit in Selection.ActiveShip.GetAssignedCritCards().Where(n => n.Type == CriticalCardType.Ship).ToList())
            {
                //TODO: what if two same crits?
                AddDecision(shipCrit.Name, delegate { DiscardCrit(shipCrit); });
            }

            defaultDecision = GetDecisions().First().Key;
        }

        private void DiscardCrit(CriticalHitCard.GenericCriticalHit critCard)
        {
            Selection.ActiveShip.FlipFacedownFaceupDamageCard(critCard);
            Sounds.PlayShipSound("R2D2-Proud");
            ConfirmDecision();
        }

        private void ConfirmDecision()
        {
            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }

    }

}
