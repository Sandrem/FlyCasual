using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ARC170Starfighter
    {
        public class OddBall : ARC170Starfighter
        {
            public OddBall() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Odd Ball\"",
                    "CC-2237",
                    Faction.Republic,
                    5,
                    5,
                    15,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.OddBallAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Torpedo,
                        UpgradeType.Torpedo,
                        UpgradeType.Astromech,
                        UpgradeType.Gunner,
                        UpgradeType.Gunner,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone
                    },
                    skinName: "Red"
                );

                PilotNameCanonical = "oddball-arc170starfighter";

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/ff/29/ff29970e-5ed7-416d-b5da-3918e226b3dc/swz33_odd-ball.png";
            }
        }
    }
}
