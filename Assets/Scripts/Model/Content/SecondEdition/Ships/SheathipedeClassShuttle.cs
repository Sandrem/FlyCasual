using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;

namespace Ship
{
    namespace SecondEdition.SheathipedeClassShuttle
    {
        public class SheathipedeClassShuttle : FirstEdition.SheathipedeClassShuttle.SheathipedeClassShuttle
        {
            public SheathipedeClassShuttle() : base()
            {
                ShipInfo.ActionIcons.Actions.RemoveAll(a => a.ActionType == typeof(TargetLockAction));

                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Reverse), MovementComplexity.Complex);

                IconicPilots[Faction.Rebel] = typeof(FennRau);

                //TODO: ManeuversImageUrl
            }
        }
    }
}
