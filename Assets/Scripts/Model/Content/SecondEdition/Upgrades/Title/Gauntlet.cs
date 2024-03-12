using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Gauntlet : GenericUpgrade
    {
        public Gauntlet() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Gauntlet",
                types: new List<UpgradeType>()
                {
                    UpgradeType.Title,
                    UpgradeType.Modification
                },
                cost: 0,
                charges: 2,
                isLimited: true,
                restrictions: new UpgradeCardRestrictions(
                    new FactionRestriction(Faction.Republic, Faction.Separatists),
                    new ShipRestriction(typeof(Ship.SecondEdition.GauntletFighter.GauntletFighter))
                ),
                addSlot: new UpgradeSlot(UpgradeType.Crew),
                abilityType: typeof(Abilities.SecondEdition.GauntletAbility)
            );
            ImageUrl = "https://infinitearenas.com/xw2/images/upgrades/gauntlet.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class GauntletAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnSystemsPhaseStart += RegisterOwnAbilityTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnSystemsPhaseStart -= RegisterOwnAbilityTrigger;
        }

        private void RegisterOwnAbilityTrigger(GenericShip ship)
        {
            if (HostUpgrade.State.Charges>0 && HostShip.Damage.GetFaceupCrits(CriticalCardType.Ship).Any())
            {
                RegisterAbilityTrigger(TriggerTypes.OnSystemsPhaseStart, AskToUseOwnAbility);
            }
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                UseOwnAbility,
                descriptionLong: "Do you want to spend 1 Charge to repair 1 of your Faceup Ship Damage cards?",
                imageHolder: HostUpgrade
            );
        }

        private void UseOwnAbility(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            HostUpgrade.State.SpendCharge();
            RepairFaceupCards();
        }

        private void RepairFaceupCards()
        {
            if (HostShip.Damage.GetFaceupCrits(CriticalCardType.Ship).Count == 1)
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
            HostShip.Damage.FlipFaceupCritFacedown(HostShip.Damage.GetFaceupCrits(CriticalCardType.Ship).First());
            Triggers.FinishTrigger();
        }

        private void AskToSelectCrit()
        {
            Phases.StartTemporarySubPhaseOld(
                HostUpgrade.UpgradeInfo.Name + ": Select faceup ship damage card",
                typeof(SubPhases.GauntletRepairSubPhase),
                Triggers.FinishTrigger
            );
        }
    }
}

namespace SubPhases
{

    public class GauntletRepairSubPhase : DecisionSubPhase
    {

        public override void PrepareDecision(System.Action callBack)
        {
            DecisionViewType = DecisionViewTypes.ImagesDamageCard;

            foreach (var faceupCrit in Selection.ThisShip.Damage.GetFaceupCrits(CriticalCardType.Ship).ToList())
            {
                AddDecision(faceupCrit.Name, delegate { Repair(faceupCrit); }, faceupCrit.ImageUrl);
            }

            DefaultDecisionName = GetDecisions().First().Name;

            callBack();
        }

        private void Repair(GenericDamageCard critCard)
        {
            Selection.ThisShip.Damage.FlipFaceupCritFacedown(critCard);
            ConfirmDecision();
        }

    }

}