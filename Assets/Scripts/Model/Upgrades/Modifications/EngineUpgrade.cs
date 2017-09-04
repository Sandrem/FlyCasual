using Upgrade;

namespace UpgradesList
{ 
	public class EngineUpgrade : GenericActionBarUpgrade<ActionsList.BoostAction>
	{
		public EngineUpgrade() : base()
		{
			Type = UpgradeType.Modification;
			Name = ShortName = "Engine Upgrade";
			ImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/5/58/Swx53-vectored-thrusters.png";
			Cost = 4;
		}        
	}
}

