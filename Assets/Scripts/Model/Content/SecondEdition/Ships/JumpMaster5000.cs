using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Actions;

namespace Ship
{
    namespace SecondEdition.JumpMaster5000
    {
        public class JumpMaster5000 : FirstEdition.JumpMaster5000.JumpMaster5000
        {
            public JumpMaster5000() : base()
            {
                ShipInfo.Hull = 6;
                ShipInfo.Shields = 3;

                ShipInfo.ActionIcons.Actions.RemoveAll(a => a.ActionType == typeof(BarrelRollAction));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BarrelRollAction), ActionColor.Red));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(FocusAction), typeof(RotateArcAction)));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(TargetLockAction), typeof(RotateArcAction)));

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Torpedo);

                IconicPilots[Faction.Scum] = typeof(Dengar);

                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn), MovementComplexity.Normal);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn), MovementComplexity.Complex);

                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn), MovementComplexity.Normal);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn), MovementComplexity.Complex);
                DialInfo.RemoveManeuver(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.SegnorsLoop));
                DialInfo.RemoveManeuver(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.SegnorsLoop));

                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Easy);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight), MovementComplexity.Easy);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.SegnorsLoop), MovementComplexity.Complex);
            }
        }
    }
}