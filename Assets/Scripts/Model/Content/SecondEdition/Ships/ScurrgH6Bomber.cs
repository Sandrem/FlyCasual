using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Upgrade;
using Actions;

namespace Ship
{
    namespace SecondEdition.ScurrgH6Bomber
    {
        public class ScurrgH6Bomber : FirstEdition.ScurrgH6Bomber.ScurrgH6Bomber
        {
            public ScurrgH6Bomber() : base()
            {
                ShipInfo.FactionsAll.Remove(Faction.Rebel);

                ShipInfo.Hull = 6;
                ShipInfo.Shields = 4;
                ShipInfo.BaseSize = BaseSize.Medium;

                ShipInfo.ActionIcons.RemoveActions(typeof(BarrelRollAction));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BarrelRollAction), ActionColor.Red));

                ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Missile);
                ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Torpedo);

                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Easy);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Easy);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Normal);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Normal);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight), MovementComplexity.Normal);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight), MovementComplexity.Complex);
                DialInfo.RemoveManeuver(new ManeuverHolder(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.Straight));

                IconicPilots[Faction.Scum] = typeof(CaptainNym);

                //TODO: ManeuversImageUrl
            }
        }
    }
}
