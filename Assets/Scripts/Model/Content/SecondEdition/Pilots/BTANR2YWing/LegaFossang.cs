using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTANR2YWing
    {
        public class LegaFossang : BTANR2YWing
        {
            public LegaFossang() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo
                (
                    "Lega Fossang",
                    3,
                    31,
                    extraUpgradeIcon: UpgradeType.Talent,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LegaFossangAbility)
                );

                ImageUrl = "https://i.imgur.com/0uhUP03.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LegaFossangAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}
