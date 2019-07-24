using BoardTools;
using Movement;
using Players;
using Remote;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Remote
{
    public class Drk1ProbeDroid : GenericRemote
    {
        public Drk1ProbeDroid(GenericPlayer owner) : base(owner)
        {
            RemoteInfo = new RemoteInfo(
                "DRK-1 Probe Droid",
                0, 3, 1,
                "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/c/c9/Swz30_probe-card.png",
                typeof(Abilities.SecondEdition.Drk1ProbeDroidAbility)
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class Drk1ProbeDroidAbility : GenericAbility
    {
        private int SelectedJointIndex = 1;
        private ManeuverTemplate SelectedManeuverTemplate;

        public override void ActivateAbility()
        {
            HostShip.OnSystemsAbilityActivation += RegisterRepositionTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnSystemsAbilityActivation -= RegisterRepositionTrigger;
        }

        private void RegisterRepositionTrigger(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, AskToPerformReposition);
        }

        private void AskToPerformReposition(object sender, EventArgs e)
        {
            AskToUseAbility(
                "Do you want to relocate",
                NeverUseByDefault,
                RelocateStart,
                descriptionLong: "You may relocate using a 2 straight or bank template",
                imageHolder: HostShip,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void RelocateStart(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            JointDecisionSubphase jointDecisionSubphase = Phases.StartTemporarySubPhaseNew<JointDecisionSubphase>(
                "Direction decision",
                AskSelectTemplate
            );

            jointDecisionSubphase.DescriptionShort = "Select direction";
            jointDecisionSubphase.DecisionOwner = HostShip.Owner;

            jointDecisionSubphase.ShowSkipButton = false;

            for (int i = 1; i < 6; i++)
            {
                int jointIndex = i;
                jointDecisionSubphase.AddDecision(
                    "Direction " + jointIndex,
                    delegate {
                        SelectDirection(jointIndex);
                        PrepareNextButton();
                    }
                );
            }

            jointDecisionSubphase.OnNextButtonIsPressed = CallSubphaseCallback;

            jointDecisionSubphase.DefaultDecisionName = "Direction 1";
            SelectDirection(1, isSilent: true);

            jointDecisionSubphase.Start();   
        }

        private void CallSubphaseCallback()
        {
            DecisionSubPhase.ConfirmDecision();
        }

        private void SelectDirection(int jointIndex, bool isSilent = false)
        {
            (HostShip as GenericRemote).ToggleJointArrow(SelectedJointIndex, isVisible: false);
            SelectedJointIndex = jointIndex;
            if (!isSilent) (HostShip as GenericRemote).ToggleJointArrow(SelectedJointIndex, isVisible: true);
        }

        private void PrepareNextButton()
        {
            (Phases.CurrentSubPhase as JointDecisionSubphase).WasDecisionButtonPressed = false;
            (Phases.CurrentSubPhase as JointDecisionSubphase).IsReadyForCommands = true;

            UI.ShowNextButton();
        }

        private void AskSelectTemplate()
        {
            (HostShip as GenericRemote).ToggleJointArrow(SelectedJointIndex, isVisible: false);

            TemplateDecisionSubphase templateDecisionSubphase = Phases.StartTemporarySubPhaseNew<TemplateDecisionSubphase>(
                "Template decision",
                StartReposition
            );

            templateDecisionSubphase.DescriptionShort = "Select template";
            templateDecisionSubphase.DecisionOwner = HostShip.Owner;

            templateDecisionSubphase.ShowSkipButton = false;

            templateDecisionSubphase.AddDecision(
                "Straight 2",
                delegate { SelectTemplate(new ManeuverTemplate(ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed2)); },
                isCentered: true
            );

            templateDecisionSubphase.AddDecision(
                "Bank 2 Left",
                delegate { SelectTemplate(new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Left, ManeuverSpeed.Speed2)); }
            );

            templateDecisionSubphase.AddDecision(
                "Bank 2 Right",
                delegate { SelectTemplate(new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Right, ManeuverSpeed.Speed2)); }
            );

            templateDecisionSubphase.DefaultDecisionName = "Straight 2";

            templateDecisionSubphase.Start();
        }

        private void SelectTemplate(ManeuverTemplate maneuverTemplate)
        {
            SelectedManeuverTemplate = maneuverTemplate;
            DecisionSubPhase.ConfirmDecision();
        }

        private void StartReposition()
        {
            GenericRemote remote = HostShip as GenericRemote;
            SelectedManeuverTemplate.ApplyTemplate(remote, SelectedJointIndex);

            GameManagerScript.Wait(1, ContinueReposition);
        }

        private void ContinueReposition()
        {
            HostShip.SetPosition(SelectedManeuverTemplate.GetFinalPosition());
            HostShip.SetAngles(SelectedManeuverTemplate.GetFinalAngles());

            GameManagerScript.Wait(1, FinishReposition);
        }

        private void FinishReposition()
        {
            SelectedManeuverTemplate.DestroyTemplate();
            Triggers.FinishTrigger();
        }

        private class JointDecisionSubphase : DecisionSubPhase { }
        private class TemplateDecisionSubphase : DecisionSubPhase { }
    }

}