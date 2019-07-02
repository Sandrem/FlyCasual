using Ship;
using Upgrade;
using System;
using System.Linq;

namespace UpgradesList.SecondEdition
{
    public class ChewbaccaRebel : GenericUpgrade
    {
        public ChewbaccaRebel() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Chewbacca",
                UpgradeType.Crew,
                cost: 4,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                charges: 2,
                regensCharges: true,
                abilityType: typeof(Abilities.SecondEdition.ChewbaccaRebelCrewAbility),
                seImageNumber: 82
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ChewbaccaRebelCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= CheckAbility;
        }

        private void CheckAbility()
        {
            if (HostUpgrade.State.Charges >= 2 && HostShip.Damage.HasFaceupCards)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskToUseOwnAbility);
            }
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                UseOwnAbility,
                descriptionLong: "Do you want to spend 2 Charge to repair 1 Faceup Damage Card?",
                imageHolder: HostUpgrade
            );
        }

        private void UseOwnAbility(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            HostUpgrade.State.SpendCharges(2);

            if (HostShip.Damage.GetFaceupCrits().Count == 1)
            {
                DoAutoRepair();
            }
            else
            {
                AskToSelectCrit();
            }
        }

        private void DoAutoRepair()
        {
            HostShip.Damage.FlipFaceupCritFacedown(HostShip.Damage.GetFaceupCrits().First());
            Sounds.PlayShipSound("Chewbacca");
            Triggers.FinishTrigger();
        }

        private void AskToSelectCrit()
        {
            Phases.StartTemporarySubPhaseOld(
                HostUpgrade.UpgradeInfo.Name + ": Select faceup damage card",
                typeof(SubPhases.ChewbaccaRebelCrewDecisionSubPhase),
                Triggers.FinishTrigger
            );
        }
    }
}

namespace SubPhases
{

    public class ChewbaccaRebelCrewDecisionSubPhase : DecisionSubPhase
    {

        public override void PrepareDecision(System.Action callBack)
        {
            DescriptionShort = "Chewbacca: Select faceup damage card";

            DecisionViewType = DecisionViewTypes.ImagesDamageCard;

            foreach (var faceupCrit in Selection.ThisShip.Damage.GetFaceupCrits().ToList())
            {
                AddDecision(faceupCrit.Name, delegate { Repair(faceupCrit); }, faceupCrit.ImageUrl);
            }

            DefaultDecisionName = GetDecisions().First().Name;

            callBack();
        }

        private void Repair(GenericDamageCard critCard)
        {
            Selection.ThisShip.Damage.FlipFaceupCritFacedown(critCard);
            Sounds.PlayShipSound("Chewbacca");
            ConfirmDecision();
        }

    }

}