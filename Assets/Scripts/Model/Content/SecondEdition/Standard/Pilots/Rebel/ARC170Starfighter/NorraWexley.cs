using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ARC170Starfighter
    {
        public class NorraWexley : ARC170Starfighter
        {
            public NorraWexley() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Norra Wexley",
                    "Gold Nine",
                    Faction.Rebel,
                    5,
                    6,
                    16,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.NorraWexleyAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Torpedo,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Astromech,
                        UpgradeType.Modification
                    },
                    seImageNumber: 65
                );
            }
        }
    }
}