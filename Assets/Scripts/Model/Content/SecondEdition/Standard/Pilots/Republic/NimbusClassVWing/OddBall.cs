using System.Collections.Generic;
using Content;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.NimbusClassVWing
    {
        public class OddBall : NimbusClassVWing
        {
            public OddBall() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Odd Ball\"",
                    "CC-2237",
                    Faction.Republic,
                    5,
                    4,
                    12,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.OddBallAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Torpedo,
                        UpgradeType.Astromech,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone,
                        Tags.Tie
                    }
                );

                PilotNameCanonical = "oddball-nimbusclassvwing";

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/8e/70/8e70bbf1-d0ca-4367-9e3a-4ad0186af71f/swz80_ship_odd-ball.png";
            }
        }
    }
}