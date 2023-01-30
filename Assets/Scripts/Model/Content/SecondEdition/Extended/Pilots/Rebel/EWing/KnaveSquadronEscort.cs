using Content;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.EWing
    {
        public class KnaveSquadronEscort : EWing
        {
            public KnaveSquadronEscort() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Knave Squadron Escort",
                    "",
                    Faction.Rebel,
                    2,
                    6,
                    14,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Sensor,
                        UpgradeType.Tech,
                        UpgradeType.Astromech,
                        UpgradeType.Modification
                    },
                    seImageNumber: 53,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}
