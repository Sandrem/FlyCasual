using Upgrade;
using System.Linq;
using System.Collections.Generic;
using SubPhases;

namespace UpgradesList.FirstEdition
{
    public class R5Astromech : GenericUpgrade
    {
        public R5Astromech() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R5 Astromech",
                UpgradeType.Astromech,
                cost: 1,
                abilityType: typeof(Abilities.FirstEdition.R5AstromechAbility)
            );
        }
    }
}

namespace Abilities.FirstEdition
{
    public class R5AstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers += RegisterR5AstromechAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers -= RegisterR5AstromechAbility;
        }

        private void RegisterR5AstromechAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, R5AstromechAbilityEffect);
        }

        private void R5AstromechAbilityEffect(object sender, System.EventArgs e)
        {
            Selection.ActiveShip = HostShip;

            List<GenericDamageCard> shipCritsList = HostShip.Damage.GetFaceupCrits(CriticalCardType.Ship);

            if (shipCritsList.Count == 1)
            {
                Selection.ActiveShip.Damage.FlipFaceupCritFacedown(shipCritsList.First());
                Sounds.PlayShipSound("R2D2-Proud");
                Triggers.FinishTrigger();
            }
            else if (shipCritsList.Count > 1)
            {
                R5AstromechDecisionSubPhase subphase = Phases.StartTemporarySubPhaseNew<R5AstromechDecisionSubPhase>(
                    "R5 Astromech: Select faceup ship Crit",
                    Triggers.FinishTrigger
                );
                subphase.DescriptionShort = "R5 Astromech";
                subphase.DescriptionLong = "Select a faceup ship Crit damage card to flip it facedown";
                subphase.ImageSource = HostUpgrade;
                subphase.Start();
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
            DecisionViewType = DecisionViewTypes.ImagesDamageCard;

            foreach (var shipCrit in Selection.ActiveShip.Damage.GetFaceupCrits(CriticalCardType.Ship).ToList())
            {
                AddDecision(shipCrit.Name, delegate { DiscardCrit(shipCrit); }, shipCrit.ImageUrl);
            }

            DefaultDecisionName = GetDecisions().First().Name;

            callBack();
        }

        private void DiscardCrit(GenericDamageCard critCard)
        {
            Selection.ActiveShip.Damage.FlipFaceupCritFacedown(critCard);
            Sounds.PlayShipSound("R2D2-Proud");
            ConfirmDecision();
        }

    }

}