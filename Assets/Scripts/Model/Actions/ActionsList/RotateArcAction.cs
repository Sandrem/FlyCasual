using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Arcs;

namespace ActionsList
{

    public class RotateArcAction : GenericAction
    {
        public RotateArcAction() {
            Name = EffectName = "Rotate Arc";
        }

        public override void ActionTake()
        {
            Phases.StartTemporarySubPhase(
                "Rotate Arc decision",
                typeof(SubPhases.RotateArcDecisionSubPhase),
                Phases.CurrentSubPhase.CallBack
            );
        }

    }

}

namespace SubPhases
{

    public class RotateArcDecisionSubPhase : DecisionSubPhase
    {

        public override void PrepareDecision(System.Action callBack)
        {
            infoText = "Rotate Arc";

            AddDecision("Front", delegate { ChangeMobileArcFacing(ArcFacing.Front); });
            AddDecision("Left",  delegate { ChangeMobileArcFacing(ArcFacing.Left); });
            AddDecision("Right", delegate { ChangeMobileArcFacing(ArcFacing.Right); });
            AddDecision("Rear",  delegate { ChangeMobileArcFacing(ArcFacing.Rear); });

            defaultDecision = GetDefaultDecision();

            callBack();
        }

        //TODO: Update for AI
        private string GetDefaultDecision()
        {
            string result = "Rear";
            return result;
        }

        private void ChangeMobileArcFacing(ArcFacing facing)
        {
            (Selection.ThisShip.ArcInfo as ArcMobile).MobileArcFacing = facing;
            ConfirmDecision();
        }

        private void ConfirmDecision()
        {
            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }

    }

}
