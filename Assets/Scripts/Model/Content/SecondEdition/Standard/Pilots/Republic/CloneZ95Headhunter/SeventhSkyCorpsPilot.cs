using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.CloneZ95Headhunter
    {
        public class SeventhSkyCorpsPilot : CloneZ95Headhunter
        {
            public SeventhSkyCorpsPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "7th Sky Corps Pilot",
                    "",
                    Faction.Republic,
                    3,
                    3,
                    3,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Sensor,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone
                    }
                );

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/reapersquadronscout.png";
            }
        }
    }
}