using ActionsList;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class EngineUpgrade : GenericUpgrade
    {
        public EngineUpgrade() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Engine Upgrade",
                UpgradeType.Modification,
                cost: 4,
                abilityType: typeof(Abilities.FirstEdition.EngineUpgradeAbility)
            );
        }
    }
}

namespace Abilities.FirstEdition
{
    public class EngineUpgradeAbility : GenericAbility
    {
        public override void ActivateAbility() { }
        public override void ActivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.AddGrantedAction(new BoostAction(), HostUpgrade);
        }

        public override void DeactivateAbility() { }
        public override void DeactivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.RemoveGrantedAction(typeof(BoostAction), HostUpgrade);
        }
    }
}