using Upgrade;
using System.Collections.Generic;
using Ship;
using System.Linq;

namespace UpgradesList.SecondEdition
{
    public class Juke : GenericUpgrade
    {
        public Juke() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Juke",
                UpgradeType.Elite,
                cost: 4,
                abilityType: typeof(Abilities.FirstEdition.JukeAbility),
                seImageNumber: 8
            );
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ShipInfo.BaseSize == BaseSize.Small || ship.ShipInfo.BaseSize == BaseSize.Medium;
        }
    }
}