using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESaBomber
    {
        public class ScimitarSquadronPilot : TIESaBomber
        {
            public ScimitarSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Scimitar Squadron Pilot",
                    "",
                    Faction.Imperial,
                    2,
                    4,
                    6,
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Missile,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    seImageNumber: 112,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}
