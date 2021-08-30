using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTANR2YWing
    {
        public class ShasaZard : BTANR2YWing
        {
            public ShasaZard() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo
                (
                    "Shasa Zard",
                    3,
                    31,
                    extraUpgradeIcon: UpgradeType.Talent,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ShasaZardAbility)
                );

                ImageUrl = "https://i.imgur.com/AL8m0H5.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ShasaZardAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}
