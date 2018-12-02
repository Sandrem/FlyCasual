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
                ShipInfo.ActionIcons.RemoveActions(typeof(TargetLockAction));

                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Reverse), MovementComplexity.Complex);

                IconicPilots[Faction.Rebel] = typeof(FennRau);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/0/03/Maneuver_sheathipede.png";
            }
        }
    }
}
