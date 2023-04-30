using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ScavengedYT1300
    {
        public class ResistanceSympathizer : ScavengedYT1300
        {
            public ResistanceSympathizer() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Resistance Sympathizer",
                    "",
                    Faction.Resistance,
                    2,
                    6,
                    10,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Modification,
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Gunner,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Freighter,
                        Tags.YT1300
                    }
                );
            }
        }
    }
}