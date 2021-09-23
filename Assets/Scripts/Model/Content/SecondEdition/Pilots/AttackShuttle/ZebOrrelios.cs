using Upgrade;

namespace Ship
{
    namespace SecondEdition.AttackShuttle
    {
        public class ZebOrrelios : AttackShuttle
        {
            public ZebOrrelios() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Zeb\" Orrelios",
                    2,
                    33,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.ZebOrreliosPilotAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 37
                );
            }
        }
    }
}