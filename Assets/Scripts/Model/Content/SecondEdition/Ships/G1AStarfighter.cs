using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;
using Upgrade;
using Actions;

namespace Ship
{
    namespace SecondEdition.G1AStarfighter
    {
        public class G1AStarfighter : FirstEdition.G1AStarfighter.G1AStarfighter
        {
            public G1AStarfighter() : base()
            {
                ShipInfo.BaseSize = BaseSize.Medium;
                ShipInfo.Hull = 5;
                ShipInfo.Shields = 4;

                IconicPilots[Faction.Scum] = typeof(GandFindsman);

                ShipInfo.ActionIcons.RemoveActions(typeof(EvadeAction));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(JamAction)));

                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed0, ManeuverDirection.Stationary, ManeuverBearing.Stationary), MovementComplexity.Complex);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn), MovementComplexity.Complex);
                DialInfo.RemoveManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn));
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight), MovementComplexity.Normal);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight), MovementComplexity.Complex);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/3/35/Maneuver_g1a_starfighter.png";
            }
        }
    }
}