using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESaBomber
    {
        public class GammaSquadronAce : TIESaBomber
        {
            public GammaSquadronAce() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Gamma Squadron Ace",
                    "",
                    Faction.Imperial,
                    3,
                    4,
                    10,
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Torpedo,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    seImageNumber: 111,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );

                ModelInfo.SkinName = "Gamma Squadron";
            }
        }
    }
}
