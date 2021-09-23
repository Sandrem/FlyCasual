using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESkStriker
    {
        public class PureSabacc : TIESkStriker
        {
            public PureSabacc() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Pure Sabacc\"",
                    4,
                    42,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.PureSabaccAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 119
                );
            }
        }
    }
}
