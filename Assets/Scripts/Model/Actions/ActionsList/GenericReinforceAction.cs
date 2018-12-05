using Arcs;
using Ship;
using System;
using System.Linq;
using UnityEngine;

namespace ActionsList
{

    public class GenericReinforceAction : GenericAction
    {
        public ArcFacing Facing;

        public GenericReinforceAction()
        {
            Name = DiceModificationName = "Reinforce (Generic)";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/ReinforceAction.png";
        }

        public override bool IsActionAvailable()
        {
            bool result = true;
            if (HostShip.IsAlreadyExecutedAction<ReinforceForeAction>() || HostShip.IsAlreadyExecutedAction<ReinforceAftAction>())
            {
                result = false;
            }
            return result;
        }

    }

}
