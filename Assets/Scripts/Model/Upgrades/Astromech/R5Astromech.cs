using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Abilities;

namespace UpgradesList
{

    public class R5Astromech : GenericUpgrade
    {
        public R5Astromech() : base()
        {
            Types.Add(UpgradeType.Astromech);
            Name = "R5 Astromech";
            Cost = 1;

            UpgradeAbilities.Add(new R5AstromechAbility());
        }
    }

}

namespace Abilities
{
    public class R5AstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.OnEndPhaseStart += RegisterR5AstromechAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.OnEndPhaseStart -= RegisterR5AstromechAbility;
        }

        private void RegisterR5AstromechAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, R5AstromechAbilityEffect);
        }

        private void R5AstromechAbilityEffect(object sender, System.EventArgs e)
        {
            Selection.ActiveShip = HostShip;

            List<CriticalHitCard.GenericCriticalHit> shipCritsList = HostShip.GetAssignedCritCards().Where(n => n.Type == CriticalCardType.Ship).ToList();

            if (shipCritsList.Count == 1)
            {
                Selection.ActiveShip.FlipFacedownFaceupDamageCard(shipCritsList.First());
                Sounds.PlayShipSound("R2D2-Proud");
                Triggers.FinishTrigger();
            }
            else if (shipCritsList.Count > 1)
            {
                Phases.StartTemporarySubPhaseOld(
                    "R5 Astromech: Select faceup ship Crit",
                    typeof(SubPhases.R5AstromechDecisionSubPhase),
                    Triggers.FinishTrigger
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

        public override void PrepareDecision(System.Action callBack)
        {
            InfoText = "R5 Astromech: Select faceup ship Crit";

            foreach (var shipCrit in Selection.ActiveShip.GetAssignedCritCards().Where(n => n.Type == CriticalCardType.Ship).ToList())
            {
                //TODO: what if two same crits?
                AddDecision(shipCrit.Name, delegate { DiscardCrit(shipCrit); });
            }

            DefaultDecision = GetDecisions().First().Key;

            callBack();
        }

        private void DiscardCrit(CriticalHitCard.GenericCriticalHit critCard)
        {
            Selection.ActiveShip.FlipFacedownFaceupDamageCard(critCard);
            Sounds.PlayShipSound("R2D2-Proud");
            ConfirmDecision();
        }

    }

}
