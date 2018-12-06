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

                IconicPilots[Faction.Scum] = typeof(MiningGuildSentry);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/6/62/Maneuver_tie_ln_fighter.png";
            }
        }
    }
}
