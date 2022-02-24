using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIERbHeavy
    {
        public class OnyxSquadronSentry : TIERbHeavy
        {
            public OnyxSquadronSentry() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Onyx Squadron Sentry",
                    "",
                    Faction.Imperial,
                    3,
                    5,
                    7,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/3d/7c/3d7ca1c1-9e57-4085-a9a6-e82a3d92c6df/swz67_onyx-sentry.png";
            }
        }
    }
}