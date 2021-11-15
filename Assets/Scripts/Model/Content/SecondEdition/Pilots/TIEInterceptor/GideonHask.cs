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
                PilotInfo = new PilotCardInfo(
                    "Gideon Hask",
                    4,
                    48,
                    isLimited: true,
                    abilityType: typeof(GideoHaskTieInterceptorAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                PilotNameCanonical = "gideonhask-tieininterceptor";

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