using Upgrade;

namespace UpgradesList
{ 
	public class EngineUpgrade : GenericActionBarUpgrade<ActionsList.BoostAction>
	{
		public EngineUpgrade() : base()
		{
			Type = UpgradeType.Modification;
			Name = ShortName = "Engine Upgrade";
			ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/upgrades/Modification/engine-upgrade.png";
			Cost = 4;
		}        
	}
}

