using Content;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class GoldSquadronVeteran : BTLA4YWing
        {
            public GoldSquadronVeteran() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Gold Squadron Veteran",
                    "",
                    Faction.Rebel,
                    3,
                    4,
                    6,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Turret,
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.YWing
                    },
                    seImageNumber: 17
                );
            }
        }
    }
}
