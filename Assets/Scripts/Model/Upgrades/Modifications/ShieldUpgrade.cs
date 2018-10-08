using RuleSets;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList
{
    public class ShieldUpgrade : GenericUpgrade, ISecondEditionUpgrade, IVariableCost
    {
        public ShieldUpgrade() : base()
        {
            Types.Add(UpgradeType.Modification);
            Name = "Shield Upgrade";
            Cost = 4;
        }

        public void AdaptUpgradeToSecondEdition()
        {
            SEImageNumber = 75;
        }

        public void UpdateCost(GenericShip ship)
        {
            Dictionary<int, int> agilityToCost = new Dictionary<int, int>()
            {
                {0, 3},
                {1, 4},
                {2, 6},
                {3, 8}
            };

            Cost = agilityToCost[ship.Agility];
        }

        public override void PreAttachToShip(GenericShip host)
        {
            base.PreAttachToShip(host);

            Host.MaxShields++;
        }

        public override void PreDettachFromShip()
        {
            base.PreDettachFromShip();

            Host.MaxShields--;
        }

    }
}
