using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionsList
{

    public class BarrelRollAction : GenericAction
    {
        public BarrelRollAction() {
            Name = "Barrel Roll";
        }

        public override void ActionTake()
        {
            Phases.StartBarrelRollSubPhase("Select target for Target Lock");
        }

    }

}
