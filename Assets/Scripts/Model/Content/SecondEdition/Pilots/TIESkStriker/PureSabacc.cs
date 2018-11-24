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
                    44,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.PureSabaccAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 119
                );
            }
        }
    }
}
