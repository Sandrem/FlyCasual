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
            UpgradeAbilities.Add(new GenericActionBarAbility<BoostAction>());
        }        
	}
}

