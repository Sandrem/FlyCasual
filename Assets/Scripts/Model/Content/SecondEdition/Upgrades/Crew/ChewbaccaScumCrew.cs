using Ship;
using Upgrade;
using System;
using Tokens;
using System.Linq;

namespace UpgradesList.SecondEdition
{
    public class ChewbaccaScum : GenericUpgrade
    {
        public ChewbaccaScum() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Chewbacca (Scum)",
                UpgradeType.Crew,
                cost: 4,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum),
                abilityType: typeof(Abilities.SecondEdition.ChewbaccaScumCrewAbility),
                seImageNumber: 157
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ChewbaccaScumCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers -= CheckAbility;
        }

        private void CheckAbility()
        {
            if (HostShip.Tokens.HasToken<FocusToken>() && HostShip.Damage.HasFaceupCards)
            {
                RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, AskToUseOwnAbility);
            }
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);
            AskToUseAbility(
                AlwaysUseByDefault,
                UseOwnAbility,
                infoText: "Do you want to use Chewbacca's ability?"
            );
        }

        private void UseOwnAbility(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.Tokens.SpendToken(typeof(FocusToken), RepairFaceupCards);
        }

        private void RepairFaceupCards()
        {
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