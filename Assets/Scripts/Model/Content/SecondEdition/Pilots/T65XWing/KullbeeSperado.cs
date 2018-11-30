using Upgrade;
using Abilities.FirstEdition;
using System.Collections.Generic;

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
                    isLimited: true,
                    abilityType: typeof(KullbeeSperadoAbility),
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Elite, UpgradeType.Illicit },
                    seImageNumber: 6
                );

                ModelInfo.SkinName = "Partisan";
            }
        }
    }
}