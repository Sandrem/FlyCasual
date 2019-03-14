using Actions;
using ActionsList;
using BoardTools;
using RulesList;
using Ship;
using SubPhases;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using UnityEngine;

namespace ActionsList
{

    public class CoordinateAction : GenericAction
    {
        public CoordinateAction()
        {
            Name = DiceModificationName = "Coordinate";
        }

        public override void ActionTake()
        {
            CoordinateTargetSubPhase subphase = Phases.StartTemporarySubPhaseNew<CoordinateTargetSubPhase>(
                "Select target for Coordinate",
                Phases.CurrentSubPhase.CallBack
            );
            subphase.HostAction = this;
            subphase.Start();
        }

        public override void RevertActionOnFail(bool hasSecondChance = false)
        {
            if (hasSecondChance)
            {
                UI.ShowSkipButton();
                UI.HighlightSkipButton();
            }
            else
            {
                Phases.GoBack();
            }
        }
    }

}

namespace SubPhases
{

    public class CoordinateTargetSubPhase : SelectShipSubPhase
    {
        public GenericAction HostAction { get; set; }

        public override void Prepare()
        {
            PrepareByParameters(
                SelectCoordinateTarget,
                FilterCoordinateTargets,
                GetAiCoordinatePriority,
                Selection.ThisShip.Owner.PlayerNo,
                false,
                "Coordinate Action",
                "Select another ship.\nIt performs free action."
            );
        }

        protected override void CancelShipSelection()
        {
            Rules.Actions.ActionIsFailed(TheShip, HostAction, ActionFailReason.WrongRange, true);
        }

        public override void SkipButton()
        {
            Rules.Actions.ActionIsFailed(TheShip, HostAction, ActionFailReason.WrongRange, false);
        }

        public int GetAiCoordinatePriority(GenericShip ship)
        {
            int result = 0;

            result += NeedTokenPriority(ship);
            result += ship.PilotInfo.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.UpgradeInfo.Cost);

            return result;
        }

        private int NeedTokenPriority(GenericShip ship)
        {
            if (!ship.Tokens.HasToken(typeof(FocusToken))) return 100;
            if (ship.ActionBar.HasAction(typeof(EvadeAction)) && !ship.Tokens.HasToken(typeof(EvadeToken))) return 50;
            if (ship.ActionBar.HasAction(typeof(TargetLockAction)) && !ship.Tokens.HasToken(typeof(BlueTargetLockToken), '*')) return 50;
            return 0;
        }

        private bool FilterCoordinateTargets(GenericShip ship)
        {
            return ship.Owner.PlayerNo == Selection.ThisShip.Owner.PlayerNo
                && Board.CheckInRange(Selection.ThisShip, ship, 1, 2, RangeCheckReason.CoordinateAction);
        }

        public void SelectCoordinateTarget()
        {
            Selection.ThisShip.CallCoordinateTargetIsSelected(TargetShip, PerformCoordinateEffect);
        }

        private void PerformCoordinateEffect()
        {
            var coordinatingShip = Selection.ThisShip;
            Selection.ThisShip = TargetShip;
            GenericAction currentAction = ActionsHolder.CurrentAction;

            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = "Coordinate",
                    TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnFreeActionPlanned,
                    EventHandler = PerformFreeAction
                }
            );

            MovementTemplates.ReturnRangeRuler();

            Triggers.ResolveTriggers(TriggerTypes.OnFreeActionPlanned, (System.Action)delegate {
                Selection.ThisShip = coordinatingShip;
                ActionsHolder.CurrentAction = currentAction;
                Phases.FinishSubPhase(this.GetType());
                CallBack();
            });
        }

        public override void RevertSubPhase() { }

        protected virtual List<GenericAction> GetPossibleActions()
        {
            return Selection.ThisShip.GetAvailableActions();
        }

        protected virtual void PerformFreeAction(object sender, System.EventArgs e)
        {
            TargetShip.AskPerformFreeAction(GetPossibleActions(), Triggers.FinishTrigger);
        }

    }

}
