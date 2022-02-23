using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.Hwk290LightFreighter
{
    public class Tapusk : Hwk290LightFreighter
    {
        public Tapusk() : base()
        {
            IsWIP = true;

            PilotInfo = new PilotCardInfo
            (
                "Tapusk",
                5,
                36,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.TapuskAbility),
                extraUpgradeIcons: new List<UpgradeType>(){ UpgradeType.Talent, UpgradeType.Illicit },
                factionOverride: Faction.Scum,
                charges: 2,
                regensCharges: 1
            );

            ImageUrl = "https://i.imgur.com/oIZlcvg.png";

            ModelInfo.SkinName = "Black";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TapuskAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}
