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
            Name = DiceModificationName = "Rotate Arc";
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

            if (Selection.ThisShip.ArcInfo.Arcs.Any(a => a is ArcMobile))
            {
                AddDecision("Front", delegate { ChangeMobileArcFacing(ArcFacing.Forward); });
                AddDecision("Left", delegate { ChangeMobileArcFacing(ArcFacing.Left); });
                AddDecision("Right", delegate { ChangeMobileArcFacing(ArcFacing.Right); });
                AddDecision("Rear", delegate { ChangeMobileArcFacing(ArcFacing.Rear); });
            }
            else if (Selection.ThisShip.ArcInfo.Arcs.Any(a => a is ArcMobileDualA))
            {
                AddDecision("Front-Rear", delegate { ChangeMobileDualArcFacing(ArcFacing.Forward); });
                AddDecision("Left-Right", delegate { ChangeMobileDualArcFacing(ArcFacing.Left); });
            }

            DefaultDecisionName = GetDefaultDecision();

            callBack();
        }

        //TODO: Update for AI
        private string GetDefaultDecision()
        {
            if (Selection.ThisShip.ArcInfo.Arcs.Any(a => a is ArcMobileDualA))
            {
                return "Front-Rear";
            }
            else if (Selection.ThisShip.ArcInfo.Arcs.Any(a => a is ArcPrimary))
            {
                return "Rear";
            }
            else
            {
                return "Front";
            }
        }

        private void ChangeMobileArcFacing(ArcFacing facing)
        {
            Selection.ThisShip.ArcInfo.GetArc<ArcMobile>().RotateArc(facing);
            ConfirmDecision();
        }

        private void ChangeMobileDualArcFacing(ArcFacing facing)
        {
            Selection.ThisShip.ArcInfo.GetArc<ArcMobileDualA>().RotateArc(facing);
            ConfirmDecision();
        }

    }

}
