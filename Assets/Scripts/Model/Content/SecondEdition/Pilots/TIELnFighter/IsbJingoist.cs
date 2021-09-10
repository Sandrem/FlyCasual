using System.Collections.Generic;
using Upgrade;
using System;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class IsbJingoist : TIELnFighter
        {
            public IsbJingoist() : base()
            {
                IsWIP = true;

                RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

                PilotInfo = new PilotCardInfo
                (
                    "ISB Jingoist",
                    4,
                    28,
                    limited: 2,
                    abilityType: typeof(Abilities.SecondEdition.IsbJingoistAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ImageUrl = "https://i.imgur.com/6rxtxtb.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class IsbJingoistAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}

