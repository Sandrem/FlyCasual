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
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Freighter,
                        Tags.YT1300
                    }
                );

                ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/28411b84c1b15f0bfa9928f2206e44f5.png";
            }
        }
    }
}