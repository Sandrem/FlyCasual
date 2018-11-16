using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Actions;
using Upgrade;
using Ship;
using System;
using SubPhases;
using Abilities.FirstEdition;

namespace Ship
{
    namespace SecondEdition.LambdaClassT4AShuttle
    {
        public class LambdaClassT4AShuttle : FirstEdition.LambdaClassShuttle.LambdaClassShuttle
        {
            public LambdaClassT4AShuttle() : base()
            {
                ShipInfo.ShipName = "Lambda-class T-4a Shuttle";

                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(ReinforceAftAction)));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(CoordinateAction)));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(JamAction), ActionColor.Red));

                IconicPilots[Faction.Imperial] = typeof(CaptainKagi);
            }
        }
    }
}