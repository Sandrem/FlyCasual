using Movement;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.ModifiedTIELnFighter
    {
        public class ModifiedTIELnFighter : FirstEdition.TIEFighter.TIEFighter
        {
            public ModifiedTIELnFighter() : base()
            {
                ShipInfo.ShipName = "Modified TIE/ln Fighter";

                ShipInfo.DefaultShipFaction = Faction.Scum;
                ShipInfo.FactionsAll = new List<Faction>() { Faction.Scum };

                DialInfo.RemoveManeuver(new ManeuverHolder(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn));
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.Straight), MovementComplexity.Complex);

                IconicPilots[Faction.Scum] = typeof(MiningGuildSurveyor);

                ModelInfo = new ShipModelInfo(
                    "Modified TIE Fighter",
                    "Mining Guild"
                );

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/6/62/Maneuver_tie_ln_fighter.png";
            }
        }
    }
}
