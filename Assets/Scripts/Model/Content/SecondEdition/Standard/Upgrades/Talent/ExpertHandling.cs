using Upgrade;
using System.Collections.Generic;
using ActionsList;
using Ship;
using Actions;

namespace UpgradesList.SecondEdition
{
    public class ExpertHandling : GenericUpgrade
    {
        public ExpertHandling() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Expert Handling",
                UpgradeType.Talent,
                cost: 2,
                restriction: new ActionBarRestriction(typeof(BarrelRollAction), ActionColor.Red),
                addAction: new ActionInfo(typeof(BarrelRollAction)),
                seImageNumber: 5
            );
        }
    }
}