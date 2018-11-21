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
                    abilityType: typeof(Abilities.FirstEdition.PureSabaccAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 119;
            }
        }
    }
}
