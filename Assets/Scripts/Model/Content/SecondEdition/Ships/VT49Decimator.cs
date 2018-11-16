using System.Collections;
using System.Collections.Generic;
using Actions;
using ActionsList;
using Movement;
using Upgrade;

namespace Ship.SecondEdition.VT49Decimator
{
    public class VT49Decimator : FirstEdition.VT49Decimator.VT49Decimator
    {
        public VT49Decimator() : base()
        {
            ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Crew);
            ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Gunner);

            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(ReinforceAftAction)));
            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(RotateArcAction)));
            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(CoordinateAction), ActionColor.Red));

            Maneuvers.Add("1.L.T", MovementComplexity.Complex);
            Maneuvers["1.L.B"] = MovementComplexity.Easy;
            Maneuvers["1.F.S"] = MovementComplexity.Easy;
            Maneuvers["1.R.B"] = MovementComplexity.Easy;
            Maneuvers.Add("1.R.T", MovementComplexity.Complex);
            Maneuvers["2.L.B"] = MovementComplexity.Normal;
            Maneuvers["2.R.B"] = MovementComplexity.Normal;
            Maneuvers["3.F.S"] = MovementComplexity.Normal;

            DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn), MovementComplexity.Complex);
            DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Easy);
            DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight), MovementComplexity.Easy);
            DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Easy);
            DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn), MovementComplexity.Complex);
            DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Normal);
            DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Normal);
            DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight), MovementComplexity.Normal);

            IconicPilots[Faction.Imperial] = typeof(CaptainOicunn);

            // ManeuversImageUrl

            /* HotacManeuverTable = new AI.XWingTable(); */
        }
    }
}
