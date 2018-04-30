using Ship;
using Ship.LambdaShuttle;
using Upgrade;
using Abilities;

namespace UpgradesList
{
    public class ST321 : GenericUpgrade
    {
        public ST321() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "ST-321";
            Cost = 3;
            isUnique = true;

            UpgradeAbilities.Add(new ST321Ability());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is LambdaShuttle;
        }
    }
}

namespace Abilities
{
    public class ST321Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.SetTargetLockRange(1, int.MaxValue);
        }

        public override void DeactivateAbility()
        {
            HostShip.SetTargetLockRange(1, 3);
        }
    }
}
