using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SubPhases;
namespace ActionsList
{

    public class BarrelRollAction : GenericAction
    {
        public BarrelRollAction()
        {
            Name = "Barrel Roll";
        }

        public override void ActionTake()
        {
            if (Selection.ThisShip.Owner.UsesHotacAiRules)
            {
                Phases.CurrentSubPhase.CallBack();
            }
            else
            {
                Phases.CurrentSubPhase.Pause();

                BarrelRollPlanningSubPhase subphase = Phases.StartTemporarySubPhaseNew<BarrelRollPlanningSubPhase>(
                    "Barrel Roll",
                    Phases.CurrentSubPhase.CallBack
                );
                subphase.HostAction = this;
                subphase.Start();
            }
        }

        public override void RevertActionOnFail(bool hasSecondChance = false)
        {
            Phases.GoBack();
        }

    }

}
