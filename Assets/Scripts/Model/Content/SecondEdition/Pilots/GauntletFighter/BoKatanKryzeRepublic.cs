using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.GauntletFighter
    {
        public class BoKatanKryzeRepublic : GauntletFighter
        {
            public BoKatanKryzeRepublic() : base()
            {
                RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

                PilotInfo = new PilotCardInfo
                (
                    "Bo-Katan Kryze",
                    4,
                    66,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.BoKatanKryzeRepublicAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ImageUrl = "https://i.imgur.com/QCUreef.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BoKatanKryzeRepublicAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}