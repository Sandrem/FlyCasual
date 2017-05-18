using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actions
{

    public class BarrelRollAction : GenericAction
    {
        public BarrelRollAction() {
            Name = "Barrel Roll";
        }

        public override void ActionTake()
        {
            Game.Phases.StartBarrelRollSubPhase("Select target for Target Lock");
        }

    }

}
