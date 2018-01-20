using RulesList;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ActionsList
{

    public class CoordinateAction : GenericAction
    {
        public CoordinateAction()
        {
            Name = EffectName = "Coordinate";
        }

        public override void ActionTake()
        {
            Phases.StartTemporarySubPhaseOld(
                "Select target for Squad Leader",
                typeof(SubPhases.CoordinateTargetSubPhase),
                Phases.CurrentSubPhase.CallBack
            );
        }

    }

}

namespace SubPhases
{

    public class CoordinateTargetSubPhase : SelectShipSubPhase
    {

        public override void Prepare()
        {
            targetsAllowed.Add(TargetTypes.OtherFriendly);
            maxRange = 2;
            finishAction = SelectCoordinateTarget;

            UI.ShowSkipButton();
        }

        private void SelectCoordinateTarget()
        {
            Selection.ThisShip.CallCoordinateTargetIsSelected(TargetShip, PerformCoordinateEffect);
        }

        private void PerformCoordinateEffect()
        {
            Selection.ThisShip = TargetShip;

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

            Triggers.ResolveTriggers(TriggerTypes.OnFreeActionPlanned, delegate {
                Phases.FinishSubPhase(typeof(CoordinateTargetSubPhase));
                CallBack();
            });
        }

        public override void RevertSubPhase() { }

        private void PerformFreeAction(object sender, System.EventArgs e)
        {
            Selection.ThisShip.GenerateAvailableActionsList();
            List<ActionsList.GenericAction> actions = Selection.ThisShip.GetAvailableActionsList();

            TargetShip.AskPerformFreeAction(actions, Triggers.FinishTrigger);
        }

        public override void SkipButton()
        {
            Phases.FinishSubPhase(typeof(CoordinateTargetSubPhase));
            CallBack();
        }

    }

}
