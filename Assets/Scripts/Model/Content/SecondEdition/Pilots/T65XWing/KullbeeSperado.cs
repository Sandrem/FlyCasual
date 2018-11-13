using Upgrade;
using Abilities.FirstEdition;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class KullbeeSperado : T65XWing
        {
            public KullbeeSperado() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Kullbee Sperado",
                    4,
                    48,
                    limited: 1,
                    abilityType: typeof(KullbeeSperadoAbility)
                );

                ModelInfo.SkinName = "Partisan";

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Illicit);

                SEImageNumber = 6;
            }
        }
    }
}