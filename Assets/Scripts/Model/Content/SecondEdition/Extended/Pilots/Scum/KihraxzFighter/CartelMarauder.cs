using Content;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.KihraxzFighter
    {
        public class CartelMarauder : KihraxzFighter
        {
            public CartelMarauder() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Cartel Marauder",
                    "",
                    Faction.Scum,
                    2,
                    4,
                    5,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Illicit
                    },
                    seImageNumber: 196,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}
