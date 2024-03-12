using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class ElectroChaffMissiles : GenericUpgrade
    {
        public ElectroChaffMissiles() : base()
        {
            IsWIP = true;

            UpgradeInfo = new UpgradeCardInfo
            (
                "Electro-Chaff Missiles",
                types: new List<UpgradeType>()
                {
                    UpgradeType.Missile,
                    UpgradeType.Device
                },
                cost: 9,
                limited: 2,
                charges: 1,
                abilityType: typeof(Abilities.SecondEdition.ElectroChaffMissilesAbility)
            );

            ImageUrl = "https://i.imgur.com/Bsb3t8W.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ElectroChaffMissilesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}