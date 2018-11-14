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

                ShipInfo.ActionIcons.Actions.Add(new ActionInfo(typeof(BoostAction)));
                ShipInfo.ActionIcons.Actions.Add(new ActionInfo(typeof(BoostAction), typeof(TargetLockAction)));
                ShipInfo.ActionIcons.Actions.Add(new ActionInfo(typeof(BarrelRollAction), typeof(TargetLockAction)));

                SetTargetLockRange(2, int.MaxValue);

                IconicPilots[Faction.Rebel] = typeof(KnaveSquadronEscort);

                Maneuvers.Add("1.L.T", MovementComplexity.Complex);
                Maneuvers["1.L.B"] = MovementComplexity.Easy;
                Maneuvers["1.R.B"] = MovementComplexity.Easy;
                Maneuvers.Add("1.R.T", MovementComplexity.Complex);
                Maneuvers["2.L.B"] = MovementComplexity.Normal;
                Maneuvers["2.R.B"] = MovementComplexity.Normal;
                Maneuvers.Remove("3.F.R");
                Maneuvers.Add("3.L.R", MovementComplexity.Complex);
                Maneuvers.Add("3.R.R", MovementComplexity.Complex);
                Maneuvers["4.F.S"] = MovementComplexity.Easy;

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

                //TODO: ManeuversImageUrl
            }
        }
    }
}
