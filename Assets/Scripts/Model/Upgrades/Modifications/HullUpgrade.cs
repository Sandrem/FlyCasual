using Upgrade;
using Abilities;
using RuleSets;
using Ship;
using System.Collections.Generic;

namespace UpgradesList
{
    public class HullUpgrade : GenericUpgrade, ISecondEditionUpgrade, IVariableCost
    {
        public HullUpgrade() : base()
        {

            Types.Add(UpgradeType.Modification);
            Name = "Hull Upgrade";
            Cost = 3;
            UpgradeAbilities.Add(new HullUpgradeAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            SEImageNumber = 73;
        }

        public void UpdateCost(GenericShip ship)
        {
            Dictionary<int, int> agilityToCost = new Dictionary<int, int>()
            {
                {0, 2},
                {1, 3},
                {2, 5},
                {3, 7}
            };

            Cost = agilityToCost[ship.Agility];
        }
    }
}

namespace Abilities
{
    public class HullUpgradeAbility : GenericAbility
    {
        protected int HullIncrease = 1;

        public HullUpgradeAbility()
        {
        }

        public HullUpgradeAbility(int hullIncrease)
        {
            HullIncrease = hullIncrease;
        }

        public override void ActivateAbility()
        {
            HostShip.AfterGetMaxHull += IncreaseMaxHull;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGetMaxHull -= IncreaseMaxHull;
        }

        private void IncreaseMaxHull(ref int maxHull)
        {
            maxHull += HullIncrease;
        }
    }
}
