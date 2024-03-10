using Content;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship
{
    namespace SecondEdition.RZ1AWing
    {
        public class SharaBeySSP : RZ1AWing
        {
            public SharaBeySSP() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Shara Bey",
                    "Green Four",
                    Faction.Rebel,
                    4,
                    4,
                    0,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.SharaBeyAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile
                    },
                    tags: new List<Tags>
                    {
                        Tags.AWing
                    },
                    abilityText: "While you defend or perform a primary attack, you may spend 1 lock you have on the enemy ship to add 1 focus result to your dice results.",
                    skinName: "Red",
                    isStandardLayout: true
                );

                MustHaveUpgrades.Add(typeof(Hopeful));
                MustHaveUpgrades.Add(typeof(ConcussionMissiles));

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/sharabey-swz106.png";

                PilotNameCanonical = "sharabey-swz106";
            }
        }
    }
}