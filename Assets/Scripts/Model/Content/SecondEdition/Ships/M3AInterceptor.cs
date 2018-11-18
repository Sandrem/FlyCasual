using Movement;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.M3AInterceptor
    {
        public class M3AInterceptor : FirstEdition.M3AInterceptor.M3AInterceptor
        {
            public M3AInterceptor() : base()
            {
                ShipInfo.Hull = 3;

                // TODOREVERT: Ability

                IconicPilots = new Dictionary<Faction, System.Type> {
                    { Faction.Scum, typeof(Inaldra) }
                };

                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Normal);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Normal);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.Straight), MovementComplexity.Normal);

            }
        }
    }
}
