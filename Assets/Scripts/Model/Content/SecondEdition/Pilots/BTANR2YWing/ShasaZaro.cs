using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTANR2YWing
    {
        public class ShasaZaro : BTANR2YWing
        {
            public ShasaZaro() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo
                (
                    "Shasa Zaro",
                    3,
                    33,
                    extraUpgradeIcon: UpgradeType.Talent,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ShasaZaroAbility)
                );

                ImageUrl = "https://i.imgur.com/AL8m0H5.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ShasaZaroAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}
