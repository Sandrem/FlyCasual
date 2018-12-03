using System.Collections;
using System.Collections.Generic;
using ActionsList;
using Actions;
using Upgrade;
using Ship;
using Arcs;

namespace Ship
{
    namespace SecondEdition.LambdaClassT4AShuttle
    {
        public class LambdaClassT4AShuttle : FirstEdition.LambdaClassShuttle.LambdaClassShuttle
        {
            public LambdaClassT4AShuttle() : base()
            {
                ShipInfo.ShipName = "Lambda-class T-4a Shuttle";

                ShipInfo.ArcInfo.Arcs.Add(new ShipArcInfo(ArcType.Rear, 2));

                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(ReinforceAftAction)));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(CoordinateAction)));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(JamAction), ActionColor.Red));

                IconicPilots[Faction.Imperial] = typeof(CaptainKagi);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/d/d4/Maneuver_lambda_shuttle.png";
            }
        }
    }
}