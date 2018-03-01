using Upgrade;

namespace UpgradesList
{ 
	public class EngineUpgrade : GenericActionBarUpgrade<ActionsList.BoostAction>
	{
		public EngineUpgrade() : base()
		{
            Types.Add(UpgradeType.Modification);
			Name = "Engine Upgrade";
			Cost = 4;
		}        
	}
}

