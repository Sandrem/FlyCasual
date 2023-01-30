using Content;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.EWing
    {
        public class RogueSquadronEscort : EWing
        {
            public RogueSquadronEscort() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Rogue Squadron Escort",
                    "",
                    Faction.Rebel,
                    4,
                    6,
                    12,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Sensor,
                        UpgradeType.Torpedo,
                        UpgradeType.Astromech,
                        UpgradeType.Modification
                    },
                    seImageNumber: 52,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}
