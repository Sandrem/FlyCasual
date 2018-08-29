using ActionsList;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;
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
            Phases.Events.CallBeforeActionSubPhaseTrigger();
            var ship = Selection.ThisShip;

            bool canPerformAction = !(ship.IsSkipsActionSubPhase  || ship.IsDestroyed
                                    || (ship.Tokens.HasToken(typeof(StressToken)) && !ship.CanPerformActionsWhileStressed));

            if (canPerformAction)
            {
                ship.GenerateAvailableActionsList();
                Triggers.RegisterTrigger(
                    new Trigger() {
                        Name = "Action",
                        TriggerOwner = Phases.CurrentPhasePlayer,
                        TriggerType = TriggerTypes.OnActionSubPhaseStart,
                        EventHandler = StartActionDecisionSubphase
                    }
                );                
            }

            Phases.Events.CallOnActionSubPhaseTrigger();
        }

        private void StartActionDecisionSubphase(object sender, System.EventArgs e)
        {
            Phases.StartTemporarySubPhaseOld(
                "Action Decision",
                typeof(ActionDecisonSubPhase),
                delegate {
                    Actions.TakeActionFinish(
                        delegate { Actions.EndActionDecisionSubhase(Finish); }
                    ); 
                }
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
        public bool ActionWasPerformed { get; private set; }

        public override void PrepareDecision(System.Action callBack)
        {
            InfoText = "Select action";
            ShowSkipButton = true;
            DefaultDecisionName = "Focus";

            List<ActionsList.GenericAction> availableActions = Selection.ThisShip.GetAvailableActions();

            if (availableActions.Count > 0)
            {
                GenerateActionButtons();
                callBack();
            }
            else
            {
                Messages.ShowErrorToHuman("Cannot perform any actions");
                Actions.CurrentAction = null;
                CallBack();
            }
        }

        public void GenerateActionButtons()
        {
            //TODO: Use more global way of fix
            HideDecisionWindowUI();

            List<ActionsList.GenericAction> availableActions = Selection.ThisShip.GetAvailableActions();
            foreach (var action in availableActions)
            {
                bool addedDecision = false;

                foreach(var kv in Selection.ThisShip.ActionBar.LinkedActions)
                {
                    Type actionType = kv.Key;
                    GenericAction linkedAction = kv.Value;

                    if (action.GetType() == actionType)
                    {
                        addedDecision = true;
                        string decisionName = action.Name + " > <color=red>" + linkedAction.Name + "</color>";

                        AddDecision(decisionName, delegate {
                            ActionWasPerformed = true;
                            Actions.TakeActionStart(action);
                        }, action.ImageUrl, -1, action.IsRed);
                    }
                }

                if(!addedDecision)
                {
                    AddDecision(action.Name, delegate {
                        ActionWasPerformed = true;
                        Actions.TakeActionStart(action);
                    }, action.ImageUrl, -1, action.IsRed);
                }
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
        public bool ActionWasPerformed { get; private set; }

        public override void PrepareDecision(System.Action callBack)
        {
            InfoText = "Select free action";
            DefaultDecisionName = "Focus";

            List<ActionsList.GenericAction> availableActions = Selection.ThisShip.GetAvailableFreeActions();

            if (availableActions.Count > 0)
            {
                GenerateFreeActionButtons();
                callBack();
            }
            else
            {
                Messages.ShowErrorToHuman("Cannot perform any free actions");
                Selection.ThisShip.IsFreeActionSkipped = true;
                Actions.CurrentAction = null;
                CallBack();
            }
        }

        public void GenerateFreeActionButtons()
		{
			Selection.ThisShip.IsFreeActionSkipped = false;
            List<ActionsList.GenericAction> availableActions = Selection.ThisShip.GetAvailableFreeActions();

            foreach (var action in availableActions)
            {
                bool addedDecision = false;

                foreach (var kv in Selection.ThisShip.ActionBar.LinkedActions)
                {
                    Type actionType = kv.Key;
                    GenericAction linkedAction = kv.Value;

                    if (action.GetType() == actionType)
                    {
                        addedDecision = true;
                        string decisionName = action.Name + " > <color=red>" + linkedAction.Name + "</color>";

                        AddDecision(
                            decisionName,
                            delegate {
                                ActionWasPerformed = true;
                                Selection.ThisShip.CallBeforeFreeActionIsPerformed(
                                    action,
                                    delegate { Actions.TakeActionStart(action); }
                                );
                            },
                            action.ImageUrl,
                            -1,
                            action.IsRed
                        );
                    }
                }

                if (!addedDecision)
                {
                    AddDecision(
                        action.Name,
                        delegate {
                            ActionWasPerformed = true;
                            Selection.ThisShip.CallBeforeFreeActionIsPerformed(
                                action,
                                delegate { Actions.TakeActionStart(action); }
                            );
                        },
                        action.ImageUrl,
                        -1,
                        action.IsRed
                    );
                }
            }
        }

        public override void Resume()
        {
            base.Resume();

            if (ShowSkipButton) UI.ShowSkipButton(); else UI.HideSkipButton();
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
