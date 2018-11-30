using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class HullUpgrade : GenericUpgrade
    {
        public HullUpgrade() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Hull Upgrade",
                UpgradeType.Modification,
                cost: 3,
                abilityType: typeof(Abilities.FirstEdition.HullUpgradeAbility)
            );
        }
    }
}

namespace Abilities.FirstEdition
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