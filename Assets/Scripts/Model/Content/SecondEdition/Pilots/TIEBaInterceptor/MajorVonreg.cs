using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEBaInterceptor
    {
        public class MajorVonreg : TIEBaInterceptor
        {
            public MajorVonreg() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Major Vonreg",
                    6,
                    55,
                    isLimited: true,
                    extraUpgradeIcon: UpgradeType.Talent,
                    abilityType: typeof(Abilities.SecondEdition.MajorVonregAbility)
                );

                ImageUrl = "https://i.imgur.com/567HyC6.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MajorVonregAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}