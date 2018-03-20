using Upgrade;
using Abilities;

namespace UpgradesList
{
    public class HullUpgrade : GenericUpgrade
    {
        public HullUpgrade() : base()
        {

            Types.Add(UpgradeType.Modification);
            Name = "Hull Upgrade";
            Cost = 3;
            UpgradeAbilities.Add(new HullUpgradeAbility());
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
