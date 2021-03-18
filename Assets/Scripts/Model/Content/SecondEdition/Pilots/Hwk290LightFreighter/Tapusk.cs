using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.Hwk290LightFreighter
{
    public class Tapusk : Hwk290LightFreighter
    {
        public Tapusk() : base()
        {
            RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

            PilotInfo = new PilotCardInfo(
                "Tapusk",
                5,
                38,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.TapuskAbility),
                extraUpgradeIcon: UpgradeType.Talent,
                factionOverride: Faction.Scum,
                charges: 2,
                regensCharges: 1
            );

            ImageUrl = "https://i.imgur.com/oIZlcvg.png";

            ModelInfo.SkinName = "Black";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TapuskAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}
