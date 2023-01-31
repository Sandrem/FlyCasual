using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.G1AStarfighter
    {
        public class GandFindsman : G1AStarfighter
        {
            public GandFindsman() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Gand Findsman",
                    "",
                    Faction.Scum,
                    1,
                    5,
                    3,
                    tags: new List<Tags>
                    {
                        Tags.BountyHunter,
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Illicit
                    },
                    seImageNumber: 203,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}