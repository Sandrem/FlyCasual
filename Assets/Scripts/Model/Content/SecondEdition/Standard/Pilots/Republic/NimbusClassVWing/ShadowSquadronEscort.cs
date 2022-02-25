using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.NimbusClassVWing
    {
        public class ShadowSquadronEscort : NimbusClassVWing
        {
            public ShadowSquadronEscort() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Shadow Squadron Escort",
                    "",
                    Faction.Republic,
                    3,
                    3,
                    3,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie,
                        Tags.Clone
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/c0/b0/c0b03f12-cff6-43af-99df-6ddf61fd471a/swz80_ship_shadow-escort.png";
            }
        }
    }
}