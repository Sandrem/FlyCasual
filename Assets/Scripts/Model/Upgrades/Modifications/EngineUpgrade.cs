using Abilities;
using ActionsList;
using Upgrade;

namespace UpgradesList
{ 
	public class EngineUpgrade : GenericUpgrade
	{
		public EngineUpgrade() : base()
		{
            Types.Add(UpgradeType.Modification);
			Name = "Engine Upgrade";
			Cost = 4;

            UpgradeAbilities.Add(new EngineUpgradeAbility());
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

