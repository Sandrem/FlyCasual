using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Actions;

namespace Ship
{
    namespace SecondEdition.Z95AF4Headhunter
    {
        public class Z95AF4Headhunter : FirstEdition.Z95Headhunter.Z95Headhunter
        {
            public Z95AF4Headhunter() : base()
            {
                ShipInfo.ShipName = "Z-95-AF4 Headhunter";

                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BarrelRollAction), ActionColor.Red));

                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight), MovementComplexity.Easy);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn), MovementComplexity.Complex);

                IconicPilots[Faction.Rebel] = typeof(LtBlount);
                IconicPilots[Faction.Scum] = typeof(NdruSuhlak);

                //TODO: ManeuversImageUrl
            }
        }
    }
}
