using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class ActionSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            base.Start();

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
            Phases.StartTemporarySubPhaseOld(
                "Action Decision",
                typeof(ActionDecisonSubPhase),
                delegate { Actions.FinishAction(Finish); }
            );
        }

        private void Finish()
        {
            UI.HideSkipButton();
            Phases.FinishSubPhase(typeof(ActionDecisonSubPhase));
            Triggers.FinishTrigger();
        }

        public override void Next()
        {
            FinishPhase();
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

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship, int mouseKeyIsPressed)
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

        public override void PrepareDecision(System.Action callBack)
        {
            InfoText = "Select action";
            ShowSkipButton = true;
            DefaultDecisionName = "Focus";

            List<ActionsList.GenericAction> availableActions = Selection.ThisShip.GetAvailableActionsList();

            if (availableActions.Count > 0)
            {
                Roster.GetPlayer(Phases.CurrentPhasePlayer).PerformAction();
                callBack();
            }
            else
            {
                Messages.ShowErrorToHuman("Cannot perform any actions");
                Actions.CurrentAction = null;
                CallBack();
            }
        }

        public void ShowActionDecisionPanel()
        {
            //TODO: Use more global way of fix
            HideDecisionWindowUI();

            List<ActionsList.GenericAction> availableActions = Selection.ThisShip.GetAvailableActionsList();
            foreach (var action in availableActions)
            {
                AddDecision(action.Name, delegate { Actions.TakeAction(action); });
                AddTooltip(action.Name, action.ImageUrl);
            }
        }

        public override void Resume()
        {
            base.Resume();

            UI.ShowSkipButton();
        }

        public override void SkipButton()
        {
            Actions.CurrentAction = null;
            CallBack();
        }

    }

}

namespace SubPhases
{

    public class FreeActionDecisonSubPhase : DecisionSubPhase
    {

        public override void PrepareDecision(System.Action callBack)
        {
            InfoText = "Select free action";
            ShowSkipButton = true;
            DefaultDecisionName = "Focus";

            List<ActionsList.GenericAction> availableActions = Selection.ThisShip.GetAvailableFreeActionsList();

            if (availableActions.Count > 0)
            {
                Selection.ThisShip.Owner.PerformFreeAction();
                callBack();
            }
            else
            {
                Messages.ShowErrorToHuman("Cannot perform any actions");
                Actions.CurrentAction = null;
                CallBack();
            }
        }

        public void ShowActionDecisionPanel()
		{
			Selection.ThisShip.IsFreeActionSkipped = false;
            List<ActionsList.GenericAction> availableActions = Selection.ThisShip.GetAvailableFreeActionsList();
            foreach (var action in availableActions)
            {
                AddDecision(action.Name, delegate { Actions.TakeAction(action); });
                AddTooltip(action.Name, action.ImageUrl);
            }
        }

        public override void Resume()
        {
            base.Resume();

            UI.ShowSkipButton();
        }

        public override void SkipButton()
        {
            UI.HideSkipButton();
            Actions.CurrentAction = null;
            Selection.ThisShip.IsFreeActionSkipped = true;
            CallBack();
        }

    }

}
