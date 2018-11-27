using Upgrade;
using System.Collections.Generic;
using ActionsList;
using Ship;
using System.Linq;
using SubPhases;
using Abilities;

namespace UpgradesList.SecondEdition
{
    public class ExpertHandling : GenericUpgrade, IVariableCost
    {
        public ExpertHandling() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Expert Handling",
                UpgradeType.Elite,
                cost: 2,
                seImageNumber: 5
            );

            UpgradeAbilities.Add(new GenericActionBarAbility<BarrelRollAction>());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ActionBar.HasAction(typeof(BarrelRollAction), isRed: true);
        }

        public void UpdateCost(GenericShip ship)
        {
            Dictionary<BaseSize, int> sizeToCost = new Dictionary<BaseSize, int>()
            {
                {BaseSize.Small, 2},
                {BaseSize.Medium, 4},
                {BaseSize.Large, 6},
            };

            UpgradeInfo.Cost = sizeToCost[ship.ShipInfo.BaseSize];
        }
    }
}