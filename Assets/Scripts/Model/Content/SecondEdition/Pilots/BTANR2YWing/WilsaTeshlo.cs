using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTANR2YWing
    {
        public class WilsaTeshlo : BTANR2YWing
        {
            public WilsaTeshlo() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo
                (
                    "Wilsa Teshlo",
                    4,
                    33,
                    extraUpgradeIcon: UpgradeType.Talent,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.WilsaTeshloAbility)
                );

                ImageUrl = "https://i.imgur.com/m8nrMvg.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WilsaTeshloAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}
