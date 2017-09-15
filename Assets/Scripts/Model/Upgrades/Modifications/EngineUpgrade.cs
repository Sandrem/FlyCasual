using Upgrade;

namespace UpgradesList
{ 
	public class EngineUpgrade : GenericActionBarUpgrade<ActionsList.BoostAction>
	{
		public EngineUpgrade() : base()
		{
			Type = UpgradeType.Modification;
			Name = ShortName = "Engine Upgrade";
			Cost = 4;
		}        
	}
}

