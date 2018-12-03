using Arcs;
using System.Linq;

namespace Ship
{
    namespace SecondEdition.LancerClassPursuitCraft
    {
        public class LancerClassPursuitCraft : FirstEdition.LancerClassPursuitCraft.LancerClassPursuitCraft
        {
            public LancerClassPursuitCraft() : base()
            {
                ShipInfo.ArcInfo.Arcs.First(a => a.ArcType == ArcType.SingleTurret).Firepower = 2;

                ShipInfo.Hull = 8;
                ShipInfo.Shields = 2;

                IconicPilots[Faction.Scum] = typeof(ShadowportHunter);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/5/5b/Maneuver_lancer.png";
            }
        }
    }
}