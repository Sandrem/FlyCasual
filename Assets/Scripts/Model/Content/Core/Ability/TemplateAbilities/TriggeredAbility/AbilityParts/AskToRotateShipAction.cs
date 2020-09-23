using System;
using Abilities.Parameters;
using SubPhases;

namespace Abilities
{
    public class AskToRotateShipAction : AbilityPart
    {
        private TriggeredAbility Ability;
        private readonly AbilityDescription Description;
        private readonly bool Rotate90Allowed;
        private readonly bool Rotate180Allowed;
        private readonly bool Rotate0Allowed;
        private readonly AbilityPart AfterAction;

        public AskToRotateShipAction
        (
            AbilityDescription description,
            bool rotate90allowed = true,
            bool rotate180allowed = true,
            bool rotate0allowed = false,
            AbilityPart afterAction = null
        )
        {
            Description = description;
            Rotate90Allowed = rotate90allowed;
            Rotate180Allowed = rotate180allowed;
            Rotate0Allowed = rotate0allowed;
            AfterAction = afterAction;
        }

        public override void DoAction(TriggeredAbility ability)
        {
            Ability = ability;

            StartAskRotationSubphase();
        }

        private void StartAskRotationSubphase()
        {
            RotationDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<RotationDecisionSubphase>("Rotate the ship?", Triggers.FinishTrigger);

            subphase.DescriptionShort = Description.Name;
            subphase.DescriptionLong = Description.Description;
            subphase.ImageSource = Description.ImageSource;

            if (Rotate180Allowed) subphase.AddDecision("180", Rotate180, isCentered: true);

            if (Rotate90Allowed)
            {
                subphase.AddDecision("90 Counterclockwise", Rotate90Counterclockwise);
                subphase.AddDecision("90 Clockwise", Rotate90Clockwise);
            }

            if (Rotate0Allowed) subphase.AddDecision("No", delegate { DecisionSubPhase.ConfirmDecision(); }, isCentered: true);

            subphase.Start();
        }

        private void Rotate180(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            Ability.HostShip.Rotate180(FinishAbilityPart);
        }

        private void Rotate90Clockwise(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            Ability.HostShip.Rotate90Clockwise(FinishAbilityPart);
        }

        private void Rotate90Counterclockwise(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            Ability.HostShip.Rotate90Counterclockwise(FinishAbilityPart);
        }

        private void FinishAbilityPart()
        {
            if (AfterAction != null)
            {
                AfterAction.DoAction(Ability);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private class RotationDecisionSubphase : DecisionSubPhase { };
    }
}
