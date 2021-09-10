using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ST70AssaultShip
    {
        public class TheMandalorian : ST70AssaultShip
        {
            public TheMandalorian() : base()
            {
                IsWIP = true;

                RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

                PilotInfo = new PilotCardInfo
                (
                    "TheMandalorian",
                    5,
                    55,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.TheMandalorianAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ImageUrl = "https://i.imgur.com/Ncx2wka.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TheMandalorianAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}