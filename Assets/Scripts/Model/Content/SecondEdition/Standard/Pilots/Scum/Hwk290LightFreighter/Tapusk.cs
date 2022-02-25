using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.Hwk290LightFreighter
{
    public class Tapusk : Hwk290LightFreighter
    {
        public Tapusk() : base()
        {
            IsWIP = true;

            PilotInfo = new PilotCardInfo25
            (
                "Tapusk",
                "Order 66 Informant",
                Faction.Scum,
                5,
                4,
                10,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.TapuskAbility),
                charges: 2,
                regensCharges: 1,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Crew,
                    UpgradeType.Device,
                    UpgradeType.Illicit,
                    UpgradeType.Modification,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Freighter
                },
                skinName: "Black"
            );

            ImageUrl = "https://i.imgur.com/oIZlcvg.png";
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
