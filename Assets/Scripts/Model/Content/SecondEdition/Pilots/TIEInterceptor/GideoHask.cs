using Abilities.SecondEdition;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEInterceptor
    {
        public class GideoHask : TIEInterceptor
        {
            public GideoHask() : base()
            {
                RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

                PilotInfo = new PilotCardInfo(
                    "Gideo Hask",
                    4,
                    42,
                    isLimited: true,
                    abilityType: typeof(GideoHaskTieInterceptorAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                PilotNameCanonical = "gideonhask-tieininterceptor";

                ModelInfo.SkinName = "Vult Skerris";

                ImageUrl = "https://i.imgur.com/AF5bPjw.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GideoHaskTieInterceptorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckGideoHaskAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckGideoHaskAbility;
        }

        private void CheckGideoHaskAbility(ref int value)
        {
            if (Combat.Defender.Damage.IsDamaged)
            {
                Messages.ShowInfo($"{Combat.Defender.PilotInfo.PilotName} is damaged, {HostShip.PilotInfo.PilotName} gains +1 attack die");
                value++;
            }
        }
    }
}