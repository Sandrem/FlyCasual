using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Upgrade;
using Actions;

namespace Ship
{
    namespace SecondEdition.KihraxzFighter
    {
        public class KihraxzFighter : FirstEdition.KihraxzFighter.KihraxzFighter
        {
            public KihraxzFighter() : base()
            {
                ShipInfo.Hull = 5;

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Modification);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Modification);

                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BarrelRollAction)));

                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight), MovementComplexity.Easy);
                DialInfo.RemoveManeuver(new ManeuverHolder(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn));
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.TallonRoll), MovementComplexity.Complex);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.TallonRoll), MovementComplexity.Complex);

                IconicPilots[Faction.Scum] = typeof(TalonbaneCobra);

                //TODO: ManeuversImageUrl
            }
        }
    }
}
