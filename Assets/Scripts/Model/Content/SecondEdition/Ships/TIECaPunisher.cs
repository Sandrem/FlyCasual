using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Actions;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIECaPunisher
    {
        public class TIECaPunisher : FirstEdition.TIEPunisher.TIEPunisher, TIE
        {
            public TIECaPunisher() : base()
            {
                ShipInfo.ShipName = "TIE/ca Punisher";
                ShipInfo.BaseSize = BaseSize.Medium;

                ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Torpedo);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Gunner);

                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(ReloadAction)));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BarrelRollAction), ActionColor.Red));
                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BoostAction), typeof(TargetLockAction)));

                IconicPilots[Faction.Imperial] = typeof(CutlassSquadronPilot);

                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed0, ManeuverDirection.Stationary, ManeuverBearing.Stationary), MovementComplexity.Complex);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn), MovementComplexity.Normal);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn), MovementComplexity.Normal);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn), MovementComplexity.Complex);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn), MovementComplexity.Complex);
            }
        }
    }
}