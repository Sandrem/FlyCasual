using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.CloneZ95Headhunter
    {
        public class ReaperSquadronScout : CloneZ95Headhunter
        {
            public ReaperSquadronScout() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Reaper Squadron Scout",
                    "",
                    Faction.Republic,
                    3,
                    3,
                    2,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone
                    }
                );

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/7thskycorpspilot.png";
            }
        }
    }
}