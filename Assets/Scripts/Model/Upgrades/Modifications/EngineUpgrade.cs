using Abilities;
using ActionsList;
using RuleSets;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList
{ 
	public class EngineUpgrade : GenericUpgrade, ISecondEditionUpgrade, IVariableCost
    {
		public EngineUpgrade() : base()
		{
            Types.Add(UpgradeType.Modification);
			Name = "Engine Upgrade";
			Cost = 4;

            UpgradeAbilities.Add(new EngineUpgradeAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            UpgradeAbilities.RemoveAll(a => a is EngineUpgradeAbility);
            UpgradeAbilities.Add(new GenericActionBarAbility<BoostAction>());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            if (RuleSet.Instance is SecondEdition)
            {
                return ship.ActionBar.HasAction(typeof(BoostAction), isRed: true);
            }
            else
            {
                return true;
            }
        }

        public void UpdateCost(GenericShip ship)
        {
            Dictionary<BaseSize, int> sizeToCost = new Dictionary<BaseSize, int>()
            {
                {BaseSize.Small, 3},
                {BaseSize.Medium, 6},
                {BaseSize.Large, 9},
            };

            Cost = sizeToCost[ship.ShipBaseSize];
        }
    }
}

namespace Abilities
{
    public class EngineUpgradeAbility : GenericAbility
    {
        public override void ActivateAbility() {}
        public override void ActivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.AddGrantedAction(new BoostAction(), HostUpgrade);
        }

        public override void DeactivateAbility(){}
        public override void DeactivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.RemoveGrantedAction(typeof(BoostAction), HostUpgrade);
        }
    }
}

