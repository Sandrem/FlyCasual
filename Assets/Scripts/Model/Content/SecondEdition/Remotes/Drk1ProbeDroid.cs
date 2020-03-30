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

        public override Dictionary<string, Vector3> BaseEdges
        {
            get
            {
                return new Dictionary<string, Vector3>()
                {
                    { "R0", new Vector3(-1.03f, 0f, -0.235f) },
                    { "R1", new Vector3(-1.795f, 0f, 0.318f) },
                    { "R2", new Vector3(-2.39f, 0f, 2.34f) },
                    { "R3", new Vector3(-2.126f, 0f, 3.123f) },
                    { "R4", new Vector3(-0.459f, 0f, 4.34f) },
                    { "R5", new Vector3(0.482f, 0f, 4.34f) },
                    { "R6", new Vector3(2.146f, 0f, 3.105f) },
                    { "R7", new Vector3(2.4f, 0f, 2.196f) },
                    { "R8", new Vector3(1.76f, 0f, 0.3f) },
                    { "R9", new Vector3(0.994f, 0f, -0.25f) }
                };
            }
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
            HostShip.OnCheckSystemsAbilityActivation += CheckAbility;
            HostShip.OnSystemsAbilityActivation += RegisterRepositionTrigger;
            GenericShip.OnPositionFinishGlobal += CheckRemoteOverlapping;
            RulesList.TargetLocksRule.OnCheckTargetLockIsAllowed += CanPerformTargetLock;
            RulesList.JamRule.OnCheckJamIsAllowed += CanPerformJam;
            HostShip.OnCheckFaceupCrit += FlipCrits;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation -= CheckAbility;
            HostShip.OnSystemsAbilityActivation -= RegisterRepositionTrigger;
            GenericShip.OnPositionFinishGlobal -= CheckRemoteOverlapping;
            RulesList.TargetLocksRule.OnCheckTargetLockIsAllowed -= CanPerformTargetLock;
            RulesList.JamRule.OnCheckJamIsAllowed -= CanPerformJam;
            HostShip.OnCheckFaceupCrit += FlipCrits;
        }

        private void CheckAbility(GenericShip ship, ref bool flag)
        {
            flag = true;
        }

        private void FlipCrits(ref bool result)
        {
            if (result == true)
            {
                result = false;
            }
        }

        private void CanPerformTargetLock(ref bool result, GenericShip ship, ITargetLockable defender)
        {
            if (ship.Owner.PlayerNo != HostShip.Owner.PlayerNo) return;

            if (defender.GetRangeToShip(HostShip) < 4)
            {
                result = true;
                return;
            }
        }

        private void CanPerformJam(ref bool result, GenericShip ship, ITargetLockable defender)
        {
            if (ship.Owner.PlayerNo != HostShip.Owner.PlayerNo) return;

            if (defender.GetRangeToShip(HostShip) < 2)
            {
                result = true;
                return;
            }
        }

        private void CheckRemoteOverlapping(GenericShip ship)
        {
            if (ship.RemotesOverlapped.Contains(HostShip))
            {
                RegisterAbilityTrigger(TriggerTypes.OnPositionFinish, RollForDestruction);
            }
        }

        private void RollForDestruction(object sender, EventArgs e)
        {
            PerformDiceCheck(
                "Roll for Remote's damage",
                DiceKind.Attack,
                1,
                CheckDamage,
                Triggers.FinishTrigger
            );
        }

        private void CheckDamage()
        {
            if (DiceCheckRoll.Focuses > 0)
            {
                Messages.ShowInfo("DRK-1 Probe Droid is destroyed");
                HostShip.Damage.TryResolveDamage(
                    1,
                    0,
                    new DamageSourceEventArgs() {
                        DamageType = DamageTypes.CardAbility,
                        Source = HostShip
                    },
                    AbilityDiceCheck.ConfirmCheck
                );
            }
            else
            {
                Messages.ShowInfo("DRK-1 Probe Droid suffers no damage");
                AbilityDiceCheck.ConfirmCheck();
            }
        }

        private void RegisterRepositionTrigger(GenericShip ship)
        {
            // Always register
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
                    },
                    isCentered: jointIndex == 5
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