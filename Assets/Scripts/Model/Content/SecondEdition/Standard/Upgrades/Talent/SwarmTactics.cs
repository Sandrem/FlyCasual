using Upgrade;
using System.Collections.Generic;
using Ship;
using System.Linq;

namespace UpgradesList.SecondEdition
{
    public class SwarmTactics : GenericUpgrade, IVariableCost
    {
        public SwarmTactics() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Swarm Tactics",
                UpgradeType.Talent,
                cost: 3,
                abilityType: typeof(Abilities.FirstEdition.SwarmTacticsAbility),
                seImageNumber: 17
            );
        }

        public void UpdateCost(GenericShip ship)
        {
            Dictionary<int, int> initiativeToCost = new Dictionary<int, int>()
            {
                {0, 3},
                {1, 3},
                {2, 3},
                {3, 3},
                {4, 3},
                {5, 4},
                {6, 5}
            };

            UpgradeInfo.Cost = initiativeToCost[ship.PilotInfo.Initiative];
        }
    }
}