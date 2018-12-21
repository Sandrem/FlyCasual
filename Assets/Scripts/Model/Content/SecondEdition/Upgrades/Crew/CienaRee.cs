using Ship;
using Upgrade;
using System.Linq;
using Actions;
using ActionsList;
using SubPhases;
using System;
using Tokens;

namespace UpgradesList.SecondEdition
{
    public class CienaRee : GenericUpgrade
    {
        public CienaRee() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ciena Ree",
                UpgradeType.Crew,
                cost: 10,
                isLimited: true,
                restrictions: new UpgradeCardRestrictions(
                    new FactionRestriction(Faction.Imperial),
                    new ActionBarRestriction(new ActionInfo(typeof(CoordinateAction)))
                ),
                abilityType: typeof(Abilities.SecondEdition.CienaReeCrewAbility),
                seImageNumber: 111
            );
        }
    }
}
namespace Abilities.SecondEdition
{
    public class CienaReeCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCoordinateTargetIsSelected += ApplyBonus;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCoordinateTargetIsSelected -= ApplyBonus;
        }

        private void ApplyBonus(GenericShip coordinatedShip)
        {
            coordinatedShip.OnActionIsPerformed += CheckBonus;
            coordinatedShip.OnActionIsSkipped += RemoveBonus;
        }

        private void CheckBonus(GenericAction action)
        {
            if (action is BoostAction || action is BarrelRollAction)
            {
                RemoveBonus(Selection.ThisShip);
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskToRotate);
            }
        }

        private void RemoveBonus(GenericShip ship)
        {
            ship.OnActionIsPerformed -= CheckBonus;
            ship.OnActionIsSkipped -= RemoveBonus;
        }

        private void AskToRotate(object sender, EventArgs e)
        {
            CienaReeRotationDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<CienaReeRotationDecisionSubphase>("Rotate the ship?", Triggers.FinishTrigger);

            subphase.InfoText = "Gain 1 Stress to rotate the ship?";

            subphase.AddDecision("90 Counterclockwise", Rotate90Counterclockwise);
            subphase.AddDecision("90 Clockwise", Rotate90Clockwise);
            subphase.AddDecision("No", delegate { DecisionSubPhase.ConfirmDecision(); }, isCentered: true);

            subphase.Start();
        }

        private void Rotate90Clockwise(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            Selection.ThisShip.Tokens.AssignToken(
                typeof(StressToken),
                delegate { Selection.ThisShip.Rotate90Clockwise(Triggers.FinishTrigger); }
            );
        }

        private void Rotate90Counterclockwise(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            Selection.ThisShip.Tokens.AssignToken(
                typeof(StressToken),
                delegate { HostShip.Rotate90Counterclockwise(Triggers.FinishTrigger); }
            );
        }

        private class CienaReeRotationDecisionSubphase : DecisionSubPhase { };
    }
}