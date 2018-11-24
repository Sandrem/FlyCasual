using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Actions;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FiresprayClassPatrolCraft
    {
        public class FiresprayClassPatrolCraft : FirstEdition.Firespray31.Firespray31
        {
            public FiresprayClassPatrolCraft() : base()
            {
                ShipInfo.ShipName = "Firespray-class Patrol Craft";
                ShipInfo.BaseSize = BaseSize.Medium;

                ShipInfo.FactionsAll.Remove(Faction.Imperial);

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Illicit);

                ShipInfo.ActionIcons.RemoveActions(typeof(EvadeAction));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BoostAction)));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(ReinforceAftAction), ActionColor.Red));

                IconicPilots[Faction.Scum] = typeof(KrassisTrelix);

                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn), MovementComplexity.Normal);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn), MovementComplexity.Normal);
                DialInfo.RemoveManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn));
                DialInfo.RemoveManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn));
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.TallonRoll), MovementComplexity.Complex);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.TallonRoll), MovementComplexity.Complex);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight), MovementComplexity.Easy);
                DialInfo.RemoveManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn));
            }
        }
    }
}