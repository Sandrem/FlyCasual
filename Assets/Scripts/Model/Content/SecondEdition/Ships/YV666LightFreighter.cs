using System.Collections;
using System.Collections.Generic;
using ActionsList;
using Actions;
using Arcs;

namespace Ship
{
    namespace SecondEdition.YV666LightFreighter
    {
        public class YV666LightFreighter : FirstEdition.YV666.YV666
        {
            public YV666LightFreighter() : base()
            {
                ShipInfo.ShipName = "YV-666 Light Freighter";
                ShipInfo.ArcInfo = new ShipArcsInfo(ArcType.FullFront, 3);
                ShipInfo.Hull = 9;
                ShipInfo.Shields = 3;

                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(ReinforceAftAction)));

                IconicPilots[Faction.Scum] = typeof(Bossk);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/2/2f/Maneuver_yv-666.png";
            }
        }
    }
}