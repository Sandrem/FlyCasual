using Mods.ModsList;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ1AWing
    {
        public class AhsokaTano : RZ1AWing
        {
            public AhsokaTano() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Ahsoka Tano",
                    5,
                    45,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.AhsokaTanoAbility),
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.ForcePower, UpgradeType.Talent },
                    force: 2,
                    abilityText: "After you fully execute a maneuver, you may choose a friendly ship at range 0-1 and spend 1 Force. That ship may perform an action, even if it is stressed."
                );

                RequiredMods = new List<Type>() { typeof(UnreleasedContentMod) };
                PilotNameCanonical = "ahsokatano-rz1awing";

                ModelInfo.SkinName = "Blue";

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/f2/84/f284aa2b-9e09-4c3c-968b-935360a65edc/swz83_pilot_ahsokatano.png";
            }
        }
    }
}