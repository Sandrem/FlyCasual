using Ship;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class InertialDampeners : GenericUpgrade, IVariableCost
    {
        public InertialDampeners() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Inertial Dampeners",
                UpgradeType.Illicit,
                cost: 3,
                abilityType: typeof(Abilities.SecondEdition.InertialDampenersAbility),
                seImageNumber: 61
            );
        }

        public void UpdateCost(GenericShip ship)
        {
            Dictionary<int, int> initiativeToCost = new Dictionary<int, int>()
            {
                {0, 0},
                {1, 1},
                {2, 2},
                {3, 3},
                {4, 4},
                {5, 5},
                {6, 6}
            };

            UpgradeInfo.Cost = initiativeToCost[ship.PilotInfo.Initiative];
        }
    }
}

namespace Abilities.SecondEdition
{
    public class InertialDampenersAbility : FirstEdition.InertialDampenersAbility
    {
        protected override void CheckAbility(GenericShip ship)
        {
            if (HostShip.State.ShieldsCurrent > 0) RegisterTrigger();
        }

        protected override void FinishAbility()
        {
            HostShip.LoseShield();
            Triggers.FinishTrigger();
        }
    }
}