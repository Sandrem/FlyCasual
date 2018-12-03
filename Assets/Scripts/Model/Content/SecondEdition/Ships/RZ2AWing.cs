using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Upgrade;
using Actions;
using Ship;
using Abilities.SecondEdition;
using System.Linq;
using Arcs;

namespace Ship
{
    namespace SecondEdition.RZ2AWing
    {
        public class RZ2AWing : FirstEdition.AWing.AWing
        {
            public RZ2AWing() : base()
            {
                ShipInfo.ShipName = "RZ-2 A-wing";

                ShipInfo.ArcInfo = new ShipArcsInfo(ArcType.SingleTurret, 2);

                ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Modification);

                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BarrelRollAction)));

                ShipInfo.DefaultShipFaction = Faction.Resistance;
                ShipInfo.FactionsAll = new List<Faction>() { Faction.Resistance };

                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.SegnorsLoop), MovementComplexity.Complex);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.SegnorsLoop), MovementComplexity.Complex);
                DialInfo.RemoveManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn));

                // ShipAbilities.Add(new VectoredThrusters());

                IconicPilots[Faction.Resistance] = typeof(TallissanLintra);

                // ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/b/b4/Maneuver_a-wing.png";
            }
        }
    }
}