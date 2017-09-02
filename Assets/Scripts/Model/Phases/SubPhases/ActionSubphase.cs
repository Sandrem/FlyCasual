using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class ActionSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Name = "Action SubPhase";
            RequiredPilotSkill = PreviousSubPhase.RequiredPilotSkill;
            RequiredPlayer = PreviousSubPhase.RequiredPlayer;
            CanBePaused = true;
            UpdateHelpInfo();
        }

        public override void Initialize()
        {
            Phases.CallBeforeActionSubPhaseTrigger();

            if (!Selection.ThisShip.IsSkipsActionSubPhase)
            {
                if (!Selection.ThisShip.IsDestroyed)
                {
                    Selection.ThisShip.GenerateAvailableActionsList();
                    Triggers.RegisterTrigger(
                        new Trigger() {
                            Name = "Action",
                            TriggerOwner = Phases.CurrentPhasePlayer,
                            TriggerType = TriggerTypes.OnActionSubPhaseStart,
                            EventHandler = StartActionDecisionSubphase
                        }
                    );
                }
                else
                {
                    //Next();
                }
            }

            Phases.CallOnActionSubPhaseTrigger();
        }

        private void StartActionDecisionSubphase(object sender, System.EventArgs e)
        {
            Phases.StartTemporarySubPhase(
                "Action Decision",
                typeof(ActionDecisonSubPhase),
                EndActionDecisionSubhase
            );
        }

        private void EndActionDecisionSubhase()
        {
            Selection.ThisShip.CallOnActionDecisionSubphaseEnd();

            Triggers.ResolveTriggers(
                TriggerTypes.OnActionDecisionSubPhaseEnd,
                delegate {
                    Phases.FinishSubPhase(typeof(ActionDecisonSubPhase));
                    Triggers.FinishTrigger();
                });
        }

        public override void Next()
        {
            FinishPhase();
        }

        public override void Pause()
        {
            
        }

        public override void Resume()
        {
            
        }

        public override void FinishPhase()
        {
            GenericSubPhase activationSubPhase = new ActivationSubPhase();
            Phases.CurrentSubPhase = activationSubPhase;
            Phases.CurrentSubPhase.Start();
            Phases.CurrentSubPhase.RequiredPilotSkill = RequiredPilotSkill;
            Phases.CurrentSubPhase.RequiredPlayer = RequiredPlayer;

            Phases.CurrentSubPhase.Next();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            bool result = false;
            Messages.ShowErrorToHuman("Ship cannot be selected: Perform action first");
            return result;
        }

    }

}

namespace SubPhases
{

    public class ActionDecisonSubPhase : DecisionSubPhase
    {

        public override void Prepare()
        {
            infoText = "Select action";
            List<ActionsList.GenericAction> availableActions = Selection.ThisShip.GetAvailableActionsList();

            if (availableActions.Count > 0)
            {
                Roster.GetPlayer(Phases.CurrentPhasePlayer).PerformAction();
            }
            else
            {
                Messages.ShowErrorToHuman("Cannot perform any actions");
                CallBack();
            }
        }

        public void ShowActionDecisionPanel()
        {
            List<ActionsList.GenericAction> availableActions = Selection.ThisShip.GetAvailableActionsList();
            foreach (var action in availableActions)
            {
                AddDecision(action.Name, delegate {
                    Tooltips.EndTooltip();
                    UI.HideSkipButton();
                    Selection.ThisShip.AddAlreadyExecutedAction(action);
                    action.ActionTake();
                });
                AddTooltip(action.Name, action.ImageUrl);
            }
        }

        public override void Resume()
        {
            base.Resume();
            Initialize();

            UI.ShowSkipButton();
        }

        public override void SkipButton()
        {
            UI.HideSkipButton();
            CallBack();
        }

    }

}

namespace SubPhases
{

    public class FreeActionDecisonSubPhase : DecisionSubPhase
    {

        public override void Prepare()
        {
            infoText = "Select free action";
            List<ActionsList.GenericAction> availableActions = Selection.ThisShip.GetAvailableFreeActionsList();

            if (availableActions.Count > 0)
            {
                Roster.GetPlayer(Phases.CurrentPhasePlayer).PerformFreeAction();
            }
            else
            {
                Messages.ShowErrorToHuman("Cannot perform any actions");
                CallBack();
            }
        }

        public void ShowActionDecisionPanel()
        {
            List<ActionsList.GenericAction> availableActions = Selection.ThisShip.GetAvailableFreeActionsList();
            foreach (var action in availableActions)
            {
                AddDecision(action.Name, delegate {
                    Tooltips.EndTooltip();
                    UI.HideSkipButton();
                    Selection.ThisShip.AddAlreadyExecutedAction(action);
                    action.ActionTake();
                });
                AddTooltip(action.Name, action.ImageUrl);
            }
        }

        public override void Resume()
        {
            base.Resume();
            Initialize();
        }

        public override void SkipButton()
        {
            UI.HideSkipButton();
            CallBack();
        }

    }

}
