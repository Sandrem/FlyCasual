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
    namespace SecondEdition.UpsilonClassCommandShuttle
    {
        public class UpsilonClassCommandShuttle : FirstEdition.LambdaClassShuttle.LambdaClassShuttle
        {
            public UpsilonClassCommandShuttle() : base()
            {
                ShipInfo.ShipName = "Upsilon-class Command Shuttle";

                ShipInfo.DefaultShipFaction = Faction.FirstOrder;
                ShipInfo.FactionsAll = new List<Faction>() { Faction.FirstOrder };

                IconicPilots[Faction.FirstOrder] = typeof(LieutenantDormitz);

                // ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/d/d4/Maneuver_lambda_shuttle.png";
            }
        }
    }
}