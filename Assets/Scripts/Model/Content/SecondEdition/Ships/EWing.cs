using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Upgrade;
using Actions;

namespace Ship
{
    namespace SecondEdition.EWing
    {
        public class EWing : FirstEdition.EWing.EWing
        {
            public EWing() : base()
            {
                ShipInfo.Hull = 3;

                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BoostAction)));
                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BoostAction), typeof(TargetLockAction)));
                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BarrelRollAction), typeof(TargetLockAction)));

                SetTargetLockRange(2, int.MaxValue);

                IconicPilots[Faction.Rebel] = typeof(KnaveSquadronEscort);

                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn), MovementComplexity.Complex);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Easy);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Easy);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn), MovementComplexity.Complex);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Normal);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Normal);
                DialInfo.RemoveManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn));
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.SegnorsLoop), MovementComplexity.Complex);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.SegnorsLoop), MovementComplexity.Complex);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight), MovementComplexity.Easy);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/3/38/Maneuver_e-wing.png";
            }
        }
    }
}
