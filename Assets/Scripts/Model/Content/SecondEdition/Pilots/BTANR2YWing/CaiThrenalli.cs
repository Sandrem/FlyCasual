using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTANR2YWing
    {
        public class CaiThrenalli : BTANR2YWing
        {
            public CaiThrenalli() : base()
            {
                PilotInfo = new PilotCardInfo
                (
                    "C'ai Threnalli",
                    2,
                    28,
                    extraUpgradeIcon: UpgradeType.Talent,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CaiThrenalliAbility)
                );

                PilotNameCanonical = "caithrenalli-btanr2ywing";

                ImageUrl = "https://i.imgur.com/quuZGXf.png";
            }
        }
    }
}
