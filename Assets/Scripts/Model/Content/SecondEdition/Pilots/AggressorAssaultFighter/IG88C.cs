using ActionsList;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.AggressorAssaultFighter
    {
        public class IG88C : AggressorAssaultFighter
        {
            public IG88C() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "IG-88C",
                    4,
                    70,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.IG88CAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 199;
            }
        }
    }
}