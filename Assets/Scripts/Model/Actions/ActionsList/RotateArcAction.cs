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
            Phases.StartTemporarySubPhaseOld(
                "Rotate Arc decision",
                typeof(SubPhases.RotateArcDecisionSubPhase),
                Phases.CurrentSubPhase.CallBack
            );
        }

        public override int GetActionPriority()
        {
            return 1;
        }

    }

}

namespace SubPhases
{

    public class RotateArcDecisionSubPhase : DecisionSubPhase
    {

        public override void PrepareDecision(System.Action callBack)
        {
            InfoText = "Rotate Arc";

            AddDecision("Front", delegate { ChangeMobileArcFacing(ArcFacing.Forward); });
            AddDecision("Left",  delegate { ChangeMobileArcFacing(ArcFacing.Left); });
            AddDecision("Right", delegate { ChangeMobileArcFacing(ArcFacing.Right); });
            AddDecision("Rear",  delegate { ChangeMobileArcFacing(ArcFacing.Rear); });

            DefaultDecisionName = GetDefaultDecision();

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
            Selection.ThisShip.ArcInfo.GetArc<ArcMobile>().RotateArc(facing);
            ConfirmDecision();
        }

    }

}
