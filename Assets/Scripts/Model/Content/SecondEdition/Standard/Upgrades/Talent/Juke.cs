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
                UpgradeType.Talent,
                cost: 6,
                abilityType: typeof(Abilities.FirstEdition.JukeAbility),
                restriction: new BaseSizeRestriction(BaseSize.Small, BaseSize.Medium),
                seImageNumber: 8
            );
        }
    }
}